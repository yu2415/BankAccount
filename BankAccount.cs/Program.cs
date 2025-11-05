using System;
using System.IO;
using Banca;

namespace ConsoleBanca
{
    class Program
    {
        static void Main()
        {
            Directory.CreateDirectory(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Conti")
            );

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("═════════════════════════════════════════════════════════════════════");
                Console.WriteLine("🏦 BENVENUTO NELLA BANCA DIGITALE 🏦");
                Console.WriteLine("═════════════════════════════════════════════════════════════════════");
                Console.ResetColor();

                Console.Write("\n👤 Inserisci il tuo nome e cognome: ");
                string nome = Console.ReadLine().Trim();

                if (string.Equals(nome, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    MenuAdmin();
                }
                else
                {
                    MenuUtente(nome);
                }
            }
        }

        // -------------------- MENU ADMIN --------------------
        static void MenuAdmin()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                     👑  MODALITÀ ADMINISTRATORE  👑");
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.ResetColor();

            Console.Write("Inserisci password admin: ");
            string pwd = Console.ReadLine();
            string passwordAdmin = FileManager.LeggiPasswordAdmin();

            if (pwd != passwordAdmin)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Password errata!");
                Console.ResetColor();
                return;
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- MENU ADMIN ---");
                Console.ResetColor();

                Console.WriteLine("1️⃣  Crea nuovo conto");
                Console.WriteLine("2️⃣  Approva richieste prestiti");
                Console.WriteLine("3️⃣  Cambia password admin");
                Console.WriteLine("4️⃣  Torna al login");

                Console.Write("\n👉 Scelta: ");
                string scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        Console.Write("👤 Inserisci nome e cognome del nuovo cliente: ");
                        string nomeCliente = Console.ReadLine();
                        Console.Write("🔐 Inserisci password iniziale: ");
                        string pwdCliente = Console.ReadLine();
                        FileManager.CreaNuovoConto(nomeCliente, pwdCliente);
                        break;

                    case "2":
                        FileManager.GestisciPrestiti();
                        break;

                    case "3":
                        Console.Write("Inserisci la nuova password admin: ");
                        string nuovaPwd = Console.ReadLine();
                        FileManager.CambiaPasswordAdmin(nuovaPwd);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✅ Password admin cambiata con successo!");
                        Console.ResetColor();
                        break;

                    case "4":
                        Console.WriteLine("👋 Uscita dalla modalità admin...");
                        return;

                    default:
                        Console.WriteLine("⚠️ Scelta non valida.");
                        break;
                }
            }
        }

        // -------------------- MENU UTENTE --------------------
        static void MenuUtente(string nomeCompleto)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                         💳  MODALITÀ BANCA UTENTE  💳");
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.ResetColor();

            var conti = FileManager.TrovaContiPerIntestatario(nomeCompleto);
            if (conti.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Nessun conto trovato per questo nome. Chiedi all’admin di crearne uno.");
                Console.ResetColor();
                return;
            }

            string numeroConto = conti[0];
            if (conti.Count > 1)
            {
                Console.WriteLine("Trovati più conti. Scegli quale aprire:");
                for (int i = 0; i < conti.Count; i++)
                    Console.WriteLine($"{i + 1}. {conti[i]}");
                Console.Write("👉 Scelta: ");
                int scelta = Convert.ToInt32(Console.ReadLine());
                numeroConto = conti[scelta - 1];
            }

            Console.Write("Inserisci la password: ");
            string password = Console.ReadLine();

            ContoBancario conto = FileManager.CaricaConto(nomeCompleto, numeroConto, password);
            if (conto == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Credenziali errate o conto non trovato.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n👋 Benvenuto {conto._intestatario}!");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- MENU UTENTE ---");
                Console.ResetColor();

                Console.WriteLine("1. Visualizza saldo");
                Console.WriteLine("2. Deposita denaro");
                Console.WriteLine("3. Preleva denaro");
                Console.WriteLine("4. Storico operazioni");
                Console.WriteLine("5. Richiedi prestito");
                Console.WriteLine("6. Modifica password");
                Console.WriteLine("7. Torna al login");

                Console.Write("👉 Scelta: ");
                string scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        Console.WriteLine($"💰 Saldo attuale: {conto._saldo} €");
                        break;
                    case "2":
                        Console.Write("Importo da depositare: ");
                        double dep = Convert.ToDouble(Console.ReadLine());
                        Console.Write("Descrizione: ");
                        conto.Deposita(dep, Console.ReadLine());
                        FileManager.SalvaConto(conto);
                        Console.WriteLine("✅ Deposito registrato.");
                        break;
                    case "3":
                        Console.Write("Importo da prelevare: ");
                        double pre = Convert.ToDouble(Console.ReadLine());
                        Console.Write("Descrizione: ");
                        conto.Preleva(pre, Console.ReadLine());
                        FileManager.SalvaConto(conto);
                        Console.WriteLine("✅ Prelievo registrato.");
                        break;
                    case "4":
                        Console.WriteLine(conto.StoricoOperazioni());
                        break;
                    case "5":
                        Console.Write("Importo del prestito richiesto: ");
                        double importo = Convert.ToDouble(Console.ReadLine());
                        Prestito p = new Prestito(conto._numeroConto, importo, false);
                        FileManager.SalvaRichiestaPrestito(p, conto._intestatario);
                        Console.WriteLine("📨 Richiesta prestito inviata all’admin.");
                        break;
                    case "6":
                        Console.Write("Nuova password: ");
                        conto.CambiaPassword(Console.ReadLine());
                        FileManager.SalvaConto(conto);
                        Console.WriteLine("✅ Password modificata.");
                        break;
                    case "7":
                        Console.WriteLine("👋 Torno al login...");
                        return;
                    default:
                        Console.WriteLine("⚠️ Scelta non valida.");
                        break;
                }
            }
        }
    }
}
