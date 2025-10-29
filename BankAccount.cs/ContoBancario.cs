using System;                       // Importa il namespace System (necessario per DateTime, eccezioni, ecc.)
using System.Collections.Generic;    // Importa il supporto per liste e collezioni generiche
using System.Text;                   // Importa la classe StringBuilder per gestire stringhe in modo efficiente

namespace Banca                      // Definisce il namespace principale del progetto (Banca)
{
    public class ContoBancario       // Definisce la classe pubblica ContoBancario (rappresenta un singolo conto bancario)
    {
        public string _intestatario { get; private set; }  // Nome e cognome del titolare del conto (solo lettura pubblica)
        public string _numeroConto { get; private set; }   // Numero identificativo univoco del conto
        public string Password { get; private set; }       // Password associata al conto (solo lettura pubblica)

        public List<Transazione> _transazioni { get; private set; } = new List<Transazione>();
        // Lista di tutte le transazioni effettuate (depositi e prelievi)

        private static int _contatore = 1000000000;
        // Contatore statico usato per generare nuovi numeri di conto univoci (inizia da IT1000000000)

        public double _saldo
        {
            get
            {
                double s = 0;                              // Variabile temporanea per sommare le transazioni
                foreach (var t in _transazioni)            // Cicla su tutte le transazioni
                    s += t._importo;                       // Somma (positivi o negativi) gli importi
                return s;                                  // Restituisce il saldo totale
            }
        }

        // ================================================================
        // COSTRUTTORE → usato quando si crea un NUOVO conto
        // ================================================================
        public ContoBancario(string intestatario, string password)
        {
            _intestatario = intestatario;                  // Salva il nome e cognome del titolare
            Password = password;                           // Imposta la password scelta
            _numeroConto = $"IT{_contatore++}";            // Genera un nuovo numero di conto univoco
        }

        // ================================================================
        // COSTRUTTORE → usato per CARICARE da file un conto esistente
        // ================================================================
        public ContoBancario(string intestatario, string numero, string password)
        {
            _intestatario = intestatario;                  // Imposta l’intestatario letto dal file
            _numeroConto = numero;                         // Imposta il numero di conto letto dal file
            Password = password;                           // Imposta la password letta dal file
        }

        // ================================================================
        // METODO → Deposita denaro sul conto
        // ================================================================
        public void Deposita(double importo, string descrizione)
        {
            if (importo <= 0)                              // Se l’importo è negativo o zero...
                throw new ArgumentException("Importo non valido."); // ...genera un’eccezione

            _transazioni.Add(new Transazione(importo, DateTime.Now, descrizione));
            // Aggiunge una nuova transazione positiva (deposito)
        }

        // ================================================================
        // METODO → Preleva denaro dal conto
        // ================================================================
        public void Preleva(double importo, string descrizione)
        {
            if (importo <= 0)                              // Controlla che l’importo sia positivo
                throw new ArgumentException("Importo non valido.");

            if (_saldo < importo)                          // Se il saldo è insufficiente...
                throw new InvalidOperationException("Fondi insufficienti."); // ...blocca l’operazione

            _transazioni.Add(new Transazione(-importo, DateTime.Now, descrizione));
            // Aggiunge una transazione negativa (prelievo)
        }

        // ================================================================
        // METODO → Restituisce lo storico di tutte le operazioni del conto
        // ================================================================
        public string StoricoOperazioni()
        {
            var sb = new StringBuilder();                  // Crea un oggetto StringBuilder per comporre il testo
            sb.AppendLine($"Conto: {_numeroConto} - {_intestatario}");
            // Aggiunge intestazione con numero conto e nome
            sb.AppendLine("Data\t\tImporto\tDescrizione");  // Aggiunge l’intestazione delle colonne

            foreach (var t in _transazioni)                // Scorre tutte le transazioni
                sb.AppendLine($"{t._data:dd/MM/yyyy}\t{t._importo}\t{t._descrizione}");
            // Aggiunge una riga per ogni transazione (data, importo, descrizione)

            sb.AppendLine($"\nSaldo attuale: {_saldo} euro"); // Mostra saldo finale
            return sb.ToString();                            // Restituisce la stringa completa
        }

        // ================================================================
        // METODO → Cambia la password del conto in modo sicuro
        // ================================================================
        public void CambiaPassword(string nuovaPassword)
        {
            if (string.IsNullOrWhiteSpace(nuovaPassword))   // Verifica che non sia vuota o spazi bianchi
                throw new ArgumentException("La nuova password non può essere vuota.");

            Password = nuovaPassword;                       // Aggiorna la password del conto
        }
        public void AggiungiTransazioneDaFile(double importo, DateTime data, string descrizione)
        {
            _transazioni.Add(new Transazione(importo, data, descrizione));
        }

    } // Fine classe ContoBancario
}     // Fine namespace Banca
