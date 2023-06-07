using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCinema
{
    internal class Film
    {

        private string nazev;
        private string cena;
        private string delka;

        public string Nazev
        {
            get { return nazev; }
            set { nazev = value; }
        }

        public string Cena
        {
            get { return cena; }
            set { cena = value; }
        }

        public string Delka
        {
            get { return delka; }
            set { delka = value; }
        }

        public Film(string n, string c, string d)
        {
            Nazev = n;
            Cena = c;
            Delka = d;
        }

        public override string ToString()
        {
            return "Film " + Nazev + ", cena: " + Cena + "Kc, delka: " + Delka + " minut";
        }
    }
}
