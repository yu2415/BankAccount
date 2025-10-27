using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Banca
{
    public static class FileManager
    {
        private static string folderPath = "Conti";
        private static string filePrestiti = "RichiestePrestiti.txt";

        // Salva conto in file .txt
        public static void SalvaConto(ContoBancario conto)
        {
            string path = Path.Combine(folderPath, $"Conto_{conto._numeroConto}.txt");

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine($"Intestatario:{conto._intestatario}");
                writer.WriteLine($"NumeroConto:{conto._numeroConto}");
                writer.WriteLine($"Password:{conto.Password}");
                writer.WriteLine("TRANSAZIONI_START");

                foreach (var t in conto._transazioni)
                    writer.WriteLine(t.ToString());

                writer.WriteLine("TRANSAZIONI_END");
            }
        }

        // Carica conto se numero e password corrispondono
        public static ContoBancario CaricaConto(string numeroConto, string password)
        {
            string path = Path.Combine(folderPath, $"Conto_{numeroConto}.txt");
            if (!File.Exists(path))
                return null;

            string intestatario = "";
            string storedPwd = "";
            var transazioni = new List<Transazione>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("Intestatario:"))
                        intestatario = line.Substring("Intestatario:".Length);
                    else if (line.StartsWith("NumeroConto:"))
                        numeroConto = line.Substring("NumeroConto:".Length);
                    else if (line.StartsWith("Password:"))
                        storedPwd = line.Substring("Password:".Length);
                    else if (line == "TRANSAZIONI_START")
                        continue;
                    else if (line == "TRANSAZIONI_END")
                        break;
                    else if (!string.IsNullOrWhiteSpace(line))
                        transazioni.Add(Transazione.Parse(line));
                }
            }

            if (storedPwd != password)
                return null;

            ContoBancario conto = new ContoBancario(intestatario, numeroConto, storedPwd);
            conto._transazioni.AddRange(transazioni);
            return conto;
        }

        public static void SalvaRichiestaPrestito(Prestito p)
        {
            using (StreamWriter writer = new StreamWriter(filePrestiti, append: true))
                writer.WriteLine(p.ToString());
        }

        public static void ApprovaPrestiti()
        {
            if (!File.Exists(filePrestiti))
            {
                Console.WriteLine("📂 Nessuna richiesta trovata.");
                return;
            }

            var prestiti = File.ReadAllLines(filePrestiti)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(Prestito.Parse)
                .ToList();

            foreach (var p in prestiti)
            {
                if (!p.Approvato)
                {
                    Console.WriteLine($"Richiesta conto {p.NumeroConto}: {p.Importo} euro");
                    Console.Write("Approvare? (s/n): ");
                    string risposta = Console.ReadLine();

                    if (risposta.ToLower() == "s")
                    {
                        p.Approvato = true;
                        ContoBancario c = CaricaConto(p.NumeroConto, CaricaPassword(p.NumeroConto));
                        if (c != null)
                        {
                            c.Deposita(p.Importo, "Prestito approvato");
                            SalvaConto(c);
                            Console.WriteLine("✅ Prestito approvato e fondi aggiunti.");
                        }
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(filePrestiti, false))
            {
                foreach (var p in prestiti)
                    writer.WriteLine(p.ToString());
            }
        }

        // metodo di supporto per leggere la password da file conto
        private static string CaricaPassword(string numeroConto)
        {
            string path = Path.Combine(folderPath, $"Conto_{numeroConto}.txt");
            if (!File.Exists(path)) return "";

            foreach (var line in File.ReadAllLines(path))
                if (line.StartsWith("Password:"))
                    return line.Substring("Password:".Length);

            return "";
        }
    }
}
