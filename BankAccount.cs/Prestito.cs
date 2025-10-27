using System;

namespace Banca
{
    public class Prestito
    {
        public string NumeroConto { get; set; }
        public double Importo { get; set; }
        public bool Approvato { get; set; }

        public Prestito(string numeroConto, double importo, bool approvato)
        {
            NumeroConto = numeroConto;
            Importo = importo;
            Approvato = approvato;
        }

        public override string ToString()
        {
            return $"{NumeroConto}|{Importo}|{Approvato}";
        }

        public static Prestito Parse(string line)
        {
            var p = line.Split('|');
            return new Prestito(p[0], double.Parse(p[1]), bool.Parse(p[2]));
        }
    }
}
