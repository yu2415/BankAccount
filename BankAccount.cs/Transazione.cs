using System;

namespace Banca
{
    public class Transazione
    {
        public double _importo { get; }
        public DateTime _data { get; }
        public string _descrizione { get; }

        public Transazione(double importo, DateTime data, string descrizione)
        {
            _importo = importo;
            _data = data;
            _descrizione = descrizione;
        }

        public override string ToString()
        {
            return $"{_data:dd/MM/yyyy}|{_importo}|{_descrizione}";
        }

        public static Transazione Parse(string line)
        {
            var p = line.Split('|');
            double importo = double.Parse(p[1]);
            DateTime data = DateTime.Parse(p[0]);
            string descr = p.Length > 2 ? p[2] : "";
            return new Transazione(importo, data, descr);
        }
    }
}
