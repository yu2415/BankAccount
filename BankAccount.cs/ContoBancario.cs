using System;                         
using System.Collections.Generic;     
using System.Text;                    

namespace Banca                     
{
    public class ContoBancario        
    {
        private List<Transazione> _transazioni = new List<Transazione>();  // Lista per memorizzare tutte le transazioni

        public string _intestatario { get; private set; }  // Proprietà del nome del titolare, lettura pubblica e scrittura privata
        public string _numeroConto { get; private set; }   // Proprietà del numero del conto, lettura pubblica e scrittura privata

        private static int _contatoreConti = 1000000000;        // Contatore statico per generare numeri di conto univoci



        public double _saldo                              // Proprietà calcolata per il saldo corrente
        {
            get
            {
                double saldo = 0;                       // Variabile per accumulare il saldo

                foreach (var t in _transazioni)        // Per ogni transazione nella lista
                {
                    saldo += t._importo;                 // Aggiungi o sottrai l'importo al saldo
                }

                return saldo;                           
            }
        }



        public ContoBancario(string intestatario)       
        {
            _intestatario = intestatario;                // Imposta il nome del titolare
            _numeroConto = $"IT{_contatoreConti++}";    // Genera un numero conto unico incrementando il contatore
        }



        public void Deposita(double importo, string descrizione)   // Metodo per aggiungere un deposito
        {
            if (importo <= 0)                                // Controlla che l'importo sia positivo
                throw new ArgumentOutOfRangeException(nameof(importo), "L'importo deve essere maggiore di zero.");

            _transazioni.Add(new Transazione(importo, DateTime.Now, descrizione)); // Aggiunge la transazione positiva alla lista
        }




        public void Preleva(double importo, string descrizione)    // Metodo per fare un prelievo
        {
            if (importo <= 0)                                  // Controlla che l'importo sia positivo
                throw new ArgumentOutOfRangeException(nameof(importo), "L'importo deve essere maggiore di zero.");

            if (_saldo - importo < 0)                           // Controlla che ci siano fondi sufficienti
                throw new InvalidOperationException("Fondi insufficienti.");

            _transazioni.Add(new Transazione(-importo, DateTime.Now, descrizione)); // Aggiunge la transazione negativa (prelievo)
        }



        public string StoricoOperazioni()                       // Metodo che restituisce una stringa con tutte le operazioni
        {
            var sb = new StringBuilder();                       // Creo uno StringBuilder per costruire la stringa in modo efficiente
            sb.AppendLine("Data\t\tImporto\tDescrizione");     // Aggiungo intestazioni per le colonne

            foreach (var t in _transazioni)                     // Ciclo su tutte le transazioni
            {
                // StringBuilder è una classe che permette di costruire stringhe in modo più efficiente rispetto alla semplice concatenazione
                sb.AppendLine($"{t._data:dd/MM/yyyy}\t{t._importo}\t{t._descrizione}"); // Aggiungo una riga per ogni transazione
            }

            sb.AppendLine($"\nSaldo attuale: {_saldo} euro");   // Aggiungo il saldo finale
            return sb.ToString();                               // Ritorno la stringa completa
        }
    }
}
