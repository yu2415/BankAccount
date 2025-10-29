using System;   // Importa il namespace System (DateTime, ecc.)

namespace Banca
{
    public class Transazione   // Definisce la classe Transazione
    {
        public double _importo;          // Importo della transazione (positivo o negativo)
        public DateTime _data;           // Data e ora della transazione
        public string _descrizione;      // Descrizione della transazione

        // ================================================================
        // COSTRUTTORE
        // ================================================================
        public Transazione(double importo, DateTime data, string descrizione)
        {
            _importo = importo;           // Imposta l'importo
            _data = data;                 // Imposta la data
            _descrizione = descrizione;   // Imposta la descrizione
        }

        // ================================================================
        // METODO → Visualizza la transazione come stringa
        // ================================================================
        public override string ToString()
        {
            // Restituisce una stringa leggibile con importo, data e descrizione
            return $"{_data:yyyy-MM-dd HH:mm:ss} | {_importo} € | {_descrizione}";
        }
    }
}
