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

                Console.WriteLine("\n1️⃣  Accesso UTENTE");
                Console.WriteLine("2️⃣  Accesso ADMIN");
                Console.WriteLine("3️⃣  Esci dal programma");
                Console.Write("\n👉 Scelta: ");
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
                        Console.WriteLine("\n👋 Uscita dal programma...");
                        return; // Termina il programma
                    default:
                        Console.WriteLine("⚠️ Scelta non valida. Riprova.");
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
            if (pwd != "admin123")
            {
                Console.WriteLine("❌ Password errata!");
                return;
            }

            // 🔁 Menu Admin
            while (true)
            {
                Console.WriteLine("\n--- MENU ADMIN ---");
                Console.WriteLine("1. Crea nuovo conto");
                Console.WriteLine("2. Approva richieste prestiti");
                Console.WriteLine("3. Torna al menu principale");
                Console.Write("Scelta: ");
                string scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        Console.Write("Nome: ");
                        string nome = Console.ReadLine();
                        Console.Write("Cognome: ");
                        string cognome = Console.ReadLine();
                        Console.Write("Imposta password per il conto: ");
                        string password = Console.ReadLine();

                        // 🆕 Crea nuovo conto bancario
                        ContoBancario nuovo = new ContoBancario($"{nome} {cognome}", password);
                        FileManager.SalvaConto(nuovo); // Salva il conto su Desktop
                        Console.WriteLine($"✅ Conto creato: {nuovo._numeroConto}");
                        break;

                    case "2":
                        FileManager.ApprovaPrestiti(); // Gestisce prestiti pendenti
                        break;

                    case "3":
                        Console.WriteLine("👋 Uscita dalla modalità admin...");
                        return; // Torna al menu principale

                    default:
                        Console.WriteLine("⚠️ Scelta non valida.");
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

            Console.Write("Inserisci il numero del conto (es. IT1000000000): ");
            string numero = Console.ReadLine();
            Console.Write("Inserisci la password: ");
            string password = Console.ReadLine();

            // 🔍 Carica il conto da file
            ContoBancario conto = FileManager.CaricaConto(numero, password);
            if (conto == null)
            {
                Console.WriteLine("❌ Credenziali errate o conto non trovato.");
                return;
            }

            Console.WriteLine($"\n👋 Benvenuto {conto._intestatario}!");

            // 🔁 Menu utente
            while (true)
            {
                Console.WriteLine("\n--- MENU UTENTE ---");
                Console.WriteLine("1. Visualizza saldo");
                Console.WriteLine("2. Deposita denaro");
                Console.WriteLine("3. Preleva denaro");
                Console.WriteLine("4. Storico operazioni");
                Console.WriteLine("5. Richiedi prestito");
                Console.WriteLine("6. Modifica password");
                Console.WriteLine("7. Torna al menu principale");
                Console.Write("Scelta: ");

                string scelta = Console.ReadLine();

                try
                {
                    switch (scelta)
                    {
                        case "1":
                            Console.WriteLine($"💰 Saldo attuale: {conto._saldo} euro");
                            break;

                        case "2":
                            Console.Write("Importo da depositare: ");
                            double dep = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Descrizione: ");
                            string descDep = Console.ReadLine();
                            conto.Deposita(dep, descDep);
                            FileManager.SalvaConto(conto);
                            Console.WriteLine("✅ Deposito salvato.");
                            break;

                        case "3":
                            Console.Write("Importo da prelevare: ");
                            double pre = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Descrizione: ");
                            string descPre = Console.ReadLine();
                            conto.Preleva(pre, descPre);
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
                            FileManager.SalvaRichiestaPrestito(p);
                            Console.WriteLine("📨 Richiesta prestito inviata all’admin.");
                            break;

                        case "6":
                            Console.Write("🔐 Inserisci la nuova password: ");
                            string nuovaPwd = Console.ReadLine();
                            conto.CambiaPassword(nuovaPwd);
                            FileManager.SalvaConto(conto);
                            Console.WriteLine("✅ Password modificata con successo.");
                            break;

                        case "7":
                            Console.WriteLine("👋 Torno al menu principale...");
                            return;

                        default:
                            Console.WriteLine("⚠️ Opzione non valida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Errore: {ex.Message}");
                }
            }
        }
    }
}
