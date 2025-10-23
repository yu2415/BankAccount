using System;                         

namespace Banca                     
{
    public class Transazione         
    {
        public double _importo { get; }         // Proprietà pubblica per l'importo, sola lettura
        public DateTime _data { get; }          // Proprietà per la data della transazione
        public string _descrizione { get; }     // Proprietà per la descrizione dell'operazione

        
        public Transazione(double importo, DateTime data, string descrizione)
        {
            _importo = importo;           
            _data = data;                 
            _descrizione = descrizione;   
        }
    }
}
