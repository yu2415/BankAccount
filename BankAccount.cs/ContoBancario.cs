using System;
using System.Collections.Generic;
using System.Text;

namespace Banca
{
    public class ContoBancario
    {
        public string _intestatario { get; private set; }
        public string _numeroConto { get; private set; }
        public string Password { get; private set; }

        public List<Transazione> _transazioni { get; private set; } = new List<Transazione>();
        private static int _contatore = 1000000000;

        public double _saldo
        {
            get
            {
                double s = 0;
                foreach (var t in _transazioni)
                    s += t._importo;
                return s;
            }
        }

        // costruttore per nuovo conto
        public ContoBancario(string intestatario, string password)
        {
            _intestatario = intestatario;
            Password = password;
            _numeroConto = $"IT{_contatore++}";
        }

        // costruttore per caricamento da file
        public ContoBancario(string intestatario, string numero, string password)
        {
            _intestatario = intestatario;
            _numeroConto = numero;
            Password = password;
        }

        public void Deposita(double importo, string descrizione)
        {
            if (importo <= 0)
                throw new ArgumentException("Importo non valido.");
            _transazioni.Add(new Transazione(importo, DateTime.Now, descrizione));
        }

        public void Preleva(double importo, string descrizione)
        {
            if (importo <= 0)
                throw new ArgumentException("Importo non valido.");
            if (_saldo < importo)
                throw new InvalidOperationException("Fondi insufficienti.");
            _transazioni.Add(new Transazione(-importo, DateTime.Now, descrizione));
        }

        public string StoricoOperazioni()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Conto: {_numeroConto} - {_intestatario}");
            sb.AppendLine("Data\t\tImporto\tDescrizione");
            foreach (var t in _transazioni)
                sb.AppendLine($"{t._data:dd/MM/yyyy}\t{t._importo}\t{t._descrizione}");
            sb.AppendLine($"\nSaldo attuale: {_saldo} euro");
            return sb.ToString();
        }
    }
}
