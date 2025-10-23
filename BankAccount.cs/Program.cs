using System;                  
using Banca;                   

namespace ConsoleBanca         
{
    class Program              
    {
        static void Main(string[] args)   
        {
            Console.WriteLine("Benvenuto in EsempioBanca!\nQuesto programma è un esempio di un conto bancario.");
            Console.WriteLine("!ATTENZIONE!\nQuesto programma rappresenta un conto bancario in cui l'utente può inserire");
            Console.WriteLine("Programma di Lorenzo Baravelli e Simone Raffoni\n\n\n");

            Console.WriteLine("🏦 Benvenuto nella tua Banca Digitale!");   
            Console.Write("👉 Inserisci il tuo nome per creare un nuovo conto: ");  // Chiedo il nome all'utente

            string nome = Console.ReadLine();

            ContoBancario conto = new ContoBancario(nome);   // Creo un nuovo conto per l'utente grazie al costruttore della classe ContoBancario
            Console.WriteLine($"\n✅ Conto creato per {nome}! Numero conto: {conto._numeroConto}"); //Visualizzo conferma creazione conto

            while (true)     // Ciclo infinito per mostrare il menu continuamente
            {
                Console.WriteLine("\n--- MENU ---");              
                Console.WriteLine("1. Visualizza saldo");          
                Console.WriteLine("2. Deposita denaro");           
                Console.WriteLine("3. Preleva denaro");            
                Console.WriteLine("4. Visualizza storico operazioni");
                Console.WriteLine("5. Esci");                       
                Console.Write("Scegli un'opzione: ");              // Chiedo la scelta all'utente

                string scelta = Console.ReadLine();                 

                try    
                {
                    switch (scelta)    // Controllo la scelta
                    {
                        case "1":      // Se vuole vedere il saldo
                            Console.WriteLine($"\n💰 Saldo attuale: {conto._saldo} euro");  // Stampo saldo
                            break;

                        case "2":      // Se vuole fare un deposito
                            Console.Write("👉 Importo da depositare: ");                   
                            double importoDeposito = Convert.ToDouble(Console.ReadLine()); 

                            Console.Write("📝 Descrizione: ");                             
                            string descrDeposito = Console.ReadLine();

                            conto.Deposita(importoDeposito, descrDeposito);               // Chiamo metodo deposito della classe ContoBancario
                            Console.WriteLine("✅ Deposito effettuato con successo.");    
                            break;

                        case "3":      // Se vuole prelevare soldi
                            Console.Write("👉 Importo da prelevare: ");                   
                            double importoPrelievo = Convert.ToDouble(Console.ReadLine()); 

                            Console.Write("📝 Descrizione: ");                             
                            string descrPrelievo = Console.ReadLine();

                            conto.Preleva(importoPrelievo, descrPrelievo);                 // Chiamo metodo prelievo della classe ContoBancario
                            Console.WriteLine("✅ Prelievo effettuato con successo.");    
                            break;

                        case "4":      // Se vuole vedere lo storico delle operazioni
                            Console.WriteLine("\n📜 Storico operazioni:");                 
                            Console.WriteLine(conto.StoricoOperazioni());                  // Stampo lo storico
                            break;

                        case "5":      // Se vuole uscire
                            Console.WriteLine("👋 Grazie per aver usato la banca. Arrivederci!");  // Messaggio di uscita
                            return;          // Esco dal programma

                        default:       // Se scrive un'opzione non valida
                            Console.WriteLine("⚠️ Opzione non valida. Riprova.");          // Messaggio di errore
                            break;
                    }
                }
                catch (Exception ex)      // Se c'è qualche errore (input sbagliato, fondi mancanti)
                {
                    Console.WriteLine($"❌ Errore: {ex.Message}");   // Lo stampo per capire cosa è successo
                }
            }
        }
    }
}
