// File: FileManager.cs
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Banca;

namespace ConsoleBanca
{
    public static class FileManager
    {
        // ===================== GESTIONE PASSWORD ADMIN =====================
        private static string cartellaConti;
        private static string percorsoAdmin;

        static FileManager()
        {
            // Inizializza cartellaConti e percorsoAdmin nel costruttore statico
            cartellaConti = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Conti");
            Directory.CreateDirectory(cartellaConti); // Assicura che la cartella esista
            percorsoAdmin = Path.Combine(cartellaConti, "Admin.txt");
        }

        // Restituisce la password admin, o "admin123" se non esiste ancora
        public static string LeggiPasswordAdmin()
        {
            try
            {
                if (!File.Exists(percorsoAdmin))
                {
                    string defaultPwd = "admin123";
                    File.WriteAllText(percorsoAdmin, defaultPwd);
                    return defaultPwd;
                }
                string pwd = File.ReadAllText(percorsoAdmin).Trim();
                return pwd;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la lettura della password admin: {ex.Message}");
                throw;
            }
        }

        // Cambia la password admin
        public static void CambiaPasswordAdmin(string nuovaPwd)
        {
            if (string.IsNullOrWhiteSpace(nuovaPwd))
                throw new ArgumentException("La nuova password non può essere vuota.");
            File.WriteAllText(percorsoAdmin, nuovaPwd.Trim());
        }

        // Crea o ottiene il percorso della cartella personale del cliente
        private static string GetCartellaCliente(string intestatario)
        {
            string path = Path.Combine(cartellaConti, intestatario);
            Directory.CreateDirectory(path);
            return path;
        }

        // Salva un conto bancario su file (in cartella personale)
        public static void SalvaConto(ContoBancario conto)
        {
            Directory.CreateDirectory(cartellaConti);
            string cartellaCliente = GetCartellaCliente(conto._intestatario);
            string percorso = Path.Combine(cartellaCliente, conto._numeroConto + ".txt");

            using (StreamWriter sw = new StreamWriter(percorso))
            {
                sw.WriteLine(conto._intestatario);
                sw.WriteLine(conto._numeroConto);
                sw.WriteLine(conto.Password);

                foreach (var t in conto._transazioni)
                    sw.WriteLine($"{t._data.Ticks}|{t._importo.ToString(CultureInfo.InvariantCulture)}|{t._descrizione}");
            }
        }

        public static ContoBancario CreaNuovoConto(string intestatario, string passwordIniziale)
        {
            // Crea numero conto casuale
            string numeroConto = "IT" + new Random().Next(100000000, 999999999);

            // Crea oggetto conto
            ContoBancario conto = new ContoBancario(intestatario, numeroConto, passwordIniziale);

            // Salva su file
            SalvaConto(conto);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Conto creato con successo per {intestatario}");
            Console.WriteLine($"🏦 Numero conto: {numeroConto}");
            Console.WriteLine($"🔐 Password iniziale: {passwordIniziale}");
            Console.ResetColor();

            return conto;
        }


        // Carica un conto bancario da file
        public static ContoBancario CaricaConto(string intestatario, string numeroConto, string password)
        {
            string percorso = Path.Combine(GetCartellaCliente(intestatario), numeroConto + ".txt");
            if (!File.Exists(percorso))
                return null;

            string[] righe = File.ReadAllLines(percorso);
            if (righe.Length < 3)
                return null;

            string pwd = righe[2];
            if (pwd != password)
                return null;

            ContoBancario conto = new ContoBancario(righe[0], righe[1], pwd);

            for (int i = 3; i < righe.Length; i++)
            {
                string[] parti = righe[i].Split('|');
                if (parti.Length == 3)
                {
                    DateTime data = new DateTime(long.Parse(parti[0]));
                    double importo = double.Parse(parti[1], CultureInfo.InvariantCulture);
                    string descrizione = parti[2];
                    conto.AggiungiTransazioneDaFile(importo, data, descrizione);
                }
            }

            return conto;
        }

        // Trova tutti i conti di un intestatario
        public static List<string> TrovaContiPerIntestatario(string intestatario)
        {
            string cartellaCliente = Path.Combine(cartellaConti, intestatario);
            List<string> conti = new List<string>();

            if (!Directory.Exists(cartellaCliente))
                return conti;

            foreach (var file in Directory.GetFiles(cartellaCliente, "IT*.txt"))
                conti.Add(Path.GetFileNameWithoutExtension(file));

            return conti;
        }

        // Salva una richiesta di prestito nel file personale del cliente
        public static void SalvaRichiestaPrestito(Prestito prestito, string intestatario)
        {
            string cartellaCliente = GetCartellaCliente(intestatario);
            string percorso = Path.Combine(cartellaCliente, "Prestiti.txt");

            using (StreamWriter sw = File.AppendText(percorso))
                sw.WriteLine($"{prestito._numeroConto}|{prestito._importo.ToString(CultureInfo.InvariantCulture)}|{prestito._approvato}");
        }

        // L’amministratore può visualizzare e approvare prestiti
        public static void GestisciPrestiti()
        {
            Console.Clear();
            Console.WriteLine("📋 Elenco richieste prestiti trovate:");

            string[] cartelleClienti = Directory.GetDirectories(cartellaConti);
            List<(string percorso, Prestito prestito, string intestatario)> tutteRichieste = new();

            // Legge tutti i file Prestiti.txt nelle varie cartelle
            foreach (var cartella in cartelleClienti)
            {
                string percorso = Path.Combine(cartella, "Prestiti.txt");
                if (!File.Exists(percorso)) continue;

                string[] righe = File.ReadAllLines(percorso);
                foreach (string r in righe)
                {
                    var parti = r.Split('|');
                    if (parti.Length >= 3 && bool.Parse(parti[2]) == false)
                    {
                        tutteRichieste.Add((percorso, new Prestito(parti[0], double.Parse(parti[1], CultureInfo.InvariantCulture), false), Path.GetFileName(cartella)));
                    }
                }
            }

            if (tutteRichieste.Count == 0)
            {
                Console.WriteLine("❌ Nessuna richiesta di prestito trovata.");
                return;
            }

            for (int i = 0; i < tutteRichieste.Count; i++)
            {
                var (percorso, p, nome) = tutteRichieste[i];
                Console.WriteLine($"{i + 1}. {nome} - Conto {p._numeroConto} - {p._importo} €");
            }

            Console.Write("\n👉 Inserisci i numeri dei prestiti da approvare (es: 1,3,5): ");
            string input = Console.ReadLine();
            var daApprovare = new HashSet<int>(Array.ConvertAll(input.Split(','), int.Parse));

            foreach (int idx in daApprovare)
            {
                var (percorso, prestito, intestatario) = tutteRichieste[idx - 1];
                string pwd = File.ReadAllLines(Path.Combine(cartellaConti, intestatario, prestito._numeroConto + ".txt"))[2];

                ContoBancario conto = CaricaConto(intestatario, prestito._numeroConto, pwd);
                conto.Deposita(prestito._importo, "Prestito approvato");
                SalvaConto(conto);

                // Rimuove la richiesta approvata dal file
                var linee = new List<string>(File.ReadAllLines(percorso));
                linee.Remove($"{prestito._numeroConto}|{prestito._importo.ToString(CultureInfo.InvariantCulture)}|False");
                File.WriteAllLines(percorso, linee);

                Console.WriteLine($"✅ Prestito approvato per {intestatario} ({prestito._numeroConto}): {prestito._importo} €");
            }
        }
    }
}
