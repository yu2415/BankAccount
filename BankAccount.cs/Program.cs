using System;
using System.IO;
using Banca;

namespace ConsoleBanca
{
    /*
     Ogni conto ha una password (salvata nel file .txt del conto).

    Si può accedere solo inserendo numero conto + password corretta.

    L’admin ha la sua password fissa (es. admin123).

    Tutto viene salvato e caricato tramite StreamReader/StreamWriter in .txt.

    Nessun errore di parametri o costruttori: tutto compila.
     */

    /*
     Program.cs → gestisce login admin/utente

    ContoBancario.cs → rappresenta un conto (ora ha anche Password)

    Transazione.cs → rappresenta una singola transazione

    Prestito.cs → rappresenta una richiesta di prestito

    FileManager.cs → salva/carica i conti e i prestiti (.txt)
     */
    class Program
    {
        static void Main()
        {
            Directory.CreateDirectory("Conti"); // cartella per i file .txt

            Console.WriteLine("🏦 Benvenuto nella Banca Digitale!");
            Console.Write("Sei un (1) Utente o (2) Admin? ");
            string ruolo = Console.ReadLine();

            if (ruolo == "2")
                MenuAdmin();
            else
                MenuUtente();
        }

        // -------------------- MENU ADMIN --------------------
        static void MenuAdmin()
        {
            Console.Write("Inserisci password admin: ");
            string pwd = Console.ReadLine();
            if (pwd != "admin123")
            {
                Console.WriteLine("❌ Password errata!");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n--- MENU ADMIN ---");
                Console.WriteLine("1. Crea nuovo conto");
                Console.WriteLine("2. Approva richieste prestiti");
                Console.WriteLine("3. Esci");
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

                        ContoBancario nuovo = new ContoBancario($"{nome} {cognome}", password);
                        FileManager.SalvaConto(nuovo);
                        Console.WriteLine($"✅ Conto creato: {nuovo._numeroConto}");
                        break;

                    case "2":
                        FileManager.ApprovaPrestiti();
                        break;

                    case "3":
                        Console.WriteLine("👋 Logout Admin.");
                        return;

                    default:
                        Console.WriteLine("❌ Scelta non valida.");
                        break;
                }
            }
        }

        // -------------------- MENU UTENTE --------------------
        static void MenuUtente()
        {
            Console.Write("Inserisci il numero del conto (es. IT1000000000): ");
            string numero = Console.ReadLine();
            Console.Write("Inserisci la password: ");
            string password = Console.ReadLine();

            ContoBancario conto = FileManager.CaricaConto(numero, password);
            if (conto == null)
            {
                Console.WriteLine("❌ Credenziali errate o conto non trovato.");
                return;
            }

            Console.WriteLine($"\nBenvenuto {conto._intestatario}!");

            while (true)
            {
                Console.WriteLine("\n--- MENU UTENTE ---");
                Console.WriteLine("1. Visualizza saldo");
                Console.WriteLine("2. Deposita denaro");
                Console.WriteLine("3. Preleva denaro");
                Console.WriteLine("4. Storico operazioni");
                Console.WriteLine("5. Richiedi prestito");
                Console.WriteLine("6. Esci");
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
                            Console.WriteLine("👋 Logout utente.");
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
