using System;

namespace Banca
{
    public class Prestito
    {
        
            public string _numeroConto { get; private set; }
            public double _importo { get; private set; }
            public bool _approvato { get; set; }

            public Prestito(string numeroConto, double importo, bool approvato)
            {
                _numeroConto = numeroConto;
                _importo = importo;
                _approvato = approvato;
            }


        public override string ToString()
        {
            return $"{_numeroConto}|{_importo}|{_approvato}";
        }

        public static Prestito Parse(string line)
        {
            var p = line.Split('|');
            return new Prestito(p[0], double.Parse(p[1]), bool.Parse(p[2]));
        }
    }
}
