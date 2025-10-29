using System;
using System.IO;
using System.Collections.Generic;
using Banca;
using System.Globalization;

namespace ConsoleBanca
{
    public static class FileManager
    {
        static string cartellaConti = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Conti");

        // Salva un conto bancario su file
        public static void SalvaConto(ContoBancario conto)
        {
            Directory.CreateDirectory(cartellaConti); // Assicurati che la cartella esista

            string percorso = Path.Combine(cartellaConti, conto._numeroConto + ".txt");
            using (StreamWriter sw = new StreamWriter(percorso))
            {
                sw.WriteLine(conto._intestatario);
                sw.WriteLine(conto._numeroConto);
                sw.WriteLine(conto.Password);

                // Salva le transazioni
                foreach (var t in conto._transazioni)
                {
                    sw.WriteLine($"{t._data.Ticks}|{t._importo.ToString(CultureInfo.InvariantCulture)}|{t._descrizione}");
                }
            }
        }

        // Carica un conto bancario da file
        public static ContoBancario CaricaConto(string numeroConto, string password)
        {
            string percorso = Path.Combine(cartellaConti, numeroConto + ".txt");
            if (!File.Exists(percorso))
                return null;

            string[] righe = File.ReadAllLines(percorso);
            if (righe.Length < 3)
                return null;

            string intestatario = righe[0];
            string numero = righe[1];
            string pwd = righe[2];

            if (pwd != password)
                return null;

            ContoBancario conto = new ContoBancario(intestatario, numero, pwd);

            // Carica tutte le transazioni
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

        // Salva una richiesta di prestito su file
        public static void SalvaRichiestaPrestito(Prestito prestito)
        {
            Directory.CreateDirectory(cartellaConti);

            string percorso = Path.Combine(cartellaConti, "Prestiti.txt");
            using (StreamWriter sw = File.AppendText(percorso))
            {
                sw.WriteLine($"{prestito._numeroConto}|{prestito._importo.ToString(CultureInfo.InvariantCulture)}|{prestito._approvato}");
            }
        }

        // Approva tutte le richieste di prestito pendenti
        public static void ApprovaPrestiti()
        {
            string percorso = Path.Combine(cartellaConti, "Prestiti.txt");
            if (!File.Exists(percorso))
            {
                Console.WriteLine("Nessuna richiesta di prestito trovata.");
                return;
            }

            string[] righe = File.ReadAllLines(percorso);
            File.Delete(percorso); // Cancella richieste dopo approvazione

            foreach (string riga in righe)
            {
                string[] parti = riga.Split('|');
                if (parti.Length < 2) continue;

                string numeroConto = parti[0];
                double importo = double.Parse(parti[1], CultureInfo.InvariantCulture);

                // Leggi la password direttamente dal file
                string pwd = File.ReadAllLines(Path.Combine(cartellaConti, numeroConto + ".txt"))[2];
                ContoBancario conto = CaricaConto(numeroConto, pwd);

                if (conto != null)
                {
                    conto.Deposita(importo, "Prestito approvato");
                    SalvaConto(conto);
                    Console.WriteLine($"✅ Prestito approvato per conto {numeroConto}: {importo} euro");
                }
            }
        }
    }
}
