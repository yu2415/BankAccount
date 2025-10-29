using System;
using System.IO;
using Banca;

namespace ConsoleBanca
{
    class Program
    {
        static void Main()
        {
            // 📁 Crea la cartella "Conti" sul Desktop se non esiste
            Directory.CreateDirectory(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Conti")
            );

            // 🔁 Ciclo infinito che permette di passare da Admin a Utente senza chiudere la console
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("═════════════════════════════════════════════════════════════════════");
                Console.WriteLine("🏦 BENVENUTO NELLA BANCA DIGITALE 🏦");
                Console.WriteLine("═════════════════════════════════════════════════════════════════════");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n1️⃣  Accesso UTENTE");
                Console.WriteLine("2️⃣  Accesso ADMIN");
                Console.WriteLine("3️⃣  Esci dal programma");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("\n👉 Scelta: ");
                Console.ResetColor();
                string scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        MenuUtente(); // Vai al menu utente
                        break;
                    case "2":
                        MenuAdmin(); // Vai al menu admin
                        break;
                    case "3":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n👋 Uscita dal programma...");
                        Console.ResetColor();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Scelta non valida. Riprova.");
                        Console.ResetColor();
                        break;
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

            // 🔐 Verifica password admin
            Console.Write("Inserisci password admin: ");
            string pwd = Console.ReadLine();

            string passwordAdmin = FileManager.LeggiPasswordAdmin();
            Console.WriteLine($"DEBUG: Password admin letta = '{passwordAdmin}'");  // solo per debug

            if (pwd != passwordAdmin)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Password errata!");
                Console.ResetColor();
                return;
            }


            // 🔁 Menu Admin
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- MENU ADMIN ---");
                Console.ResetColor();

                Console.WriteLine("1. Crea nuovo conto");
                Console.WriteLine("2. Approva richieste prestiti");
                Console.WriteLine("3. Cambia password admin");
                Console.WriteLine("4. Torna al menu principale");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("👉 Scelta: ");
                Console.ResetColor();
                string scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("🧾 Creazione conto non ancora implementata...");
                        Console.ResetColor();
                        break;

                    case "2":
                        FileManager.GestisciPrestiti();
                        break;

                    case "3":
                        Console.Write("Inserisci la nuova password admin: ");
                        string nuovaPwd = Console.ReadLine();
                        try
                        {
                            FileManager.CambiaPasswordAdmin(nuovaPwd);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Password admin cambiata con successo!");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Errore: {ex.Message}");
                        }
                        Console.ResetColor();
                        break;

                    case "4":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("👋 Uscita dalla modalità admin...");
                        Console.ResetColor();
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Scelta non valida.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // -------------------- MENU UTENTE --------------------
        static void MenuUtente()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                         💳  MODALITÀ BANCA UTENTE  💳");
            Console.WriteLine("═════════════════════════════════════════════════════════════════════");
            Console.ResetColor();

            Console.Write("Inserisci il tuo nome e cognome: ");
            string nomeCompleto = Console.ReadLine();

            var conti = FileManager.TrovaContiPerIntestatario(nomeCompleto);

            if (conti.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Nessun conto trovato per questo intestatario.");
                Console.ResetColor();
                return;
            }

            string numeroConto;
            if (conti.Count == 1)
            {
                numeroConto = conti[0];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Trovato un conto: {numeroConto}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Trovati più conti. Scegli quale aprire:");
                Console.ResetColor();
                for (int i = 0; i < conti.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {conti[i]}");
                }

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

            // 🔁 Menu utente
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
                Console.WriteLine("7. Torna al menu principale");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Scelta: ");
                Console.ResetColor();

                string scelta = Console.ReadLine();

                try
                {
                    switch (scelta)
                    {
                        case "1":
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"💰 Saldo attuale: {conto._saldo} euro");
                            break;

                        case "2":
                            Console.Write("Importo da depositare: ");
                            double dep = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Descrizione: ");
                            string descDep = Console.ReadLine();
                            conto.Deposita(dep, descDep);
                            FileManager.SalvaConto(conto);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Deposito salvato.");
                            break;

                        case "3":
                            Console.Write("Importo da prelevare: ");
                            double pre = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Descrizione: ");
                            string descPre = Console.ReadLine();
                            conto.Preleva(pre, descPre);
                            FileManager.SalvaConto(conto);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Prelievo registrato.");
                            break;

                        case "4":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(conto.StoricoOperazioni());
                            break;

                        case "5":
                            Console.Write("Importo del prestito richiesto: ");
                            double importo = Convert.ToDouble(Console.ReadLine());
                            Prestito p = new Prestito(conto._numeroConto, importo, false);
                            FileManager.SalvaRichiestaPrestito(p, conto._intestatario);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("📨 Richiesta prestito inviata all’admin.");
                            break;

                        case "6":
                            Console.Write("🔐 Inserisci la nuova password: ");
                            string nuovaPwd = Console.ReadLine();
                            conto.CambiaPassword(nuovaPwd);
                            FileManager.SalvaConto(conto);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Password modificata con successo.");
                            break;

                        case "7":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("👋 Torno al menu principale...");
                            Console.ResetColor();
                            return;

                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("⚠️ Opzione non valida.");
                            break;
                    }
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Errore: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
