using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCinema
{
    internal class Promitani
    {

        private string nazev;
        private Film movie;
        private Sedadlo sedadlo;
        private List<Sedadlo> sedadla;

        public Sedadlo Sedadlo
        {
            get { return sedadlo; }
            set { sedadlo = value;}
        }

        public string Nazev
        {
            get { return nazev; }
            set { nazev = value; }
        }

        public Film Movie
        {
            get { return movie; }
            set { movie = value; }
        }

        public List<Sedadlo> Sedadla
        {
            get { return sedadla; }
            set
            {
                sedadla = value;
            }
        }

        public Promitani(string n, Film f)
        {
            Nazev = n;
            Movie = f;
            Sedadla = new List<Sedadlo> { new Sedadlo(1), new Sedadlo(2), new Sedadlo(3), new Sedadlo(4), new Sedadlo(5)};
        }

        public Promitani()
        {
        }


        public Promitani(string n, Film f, Sedadlo s)
        {
            Nazev = n;
            Movie = f;
            Sedadlo = s;
        }

        public void RemoveSedadlo(Sedadlo s)
        {
            for (int i = 0; i < Sedadla.Count; i++)
            {
                if(s.Cislo == Sedadla[i].Cislo)
                {
                    Sedadla.RemoveAt(i);
                }
            }
        }

        public string Vypis()
        {
            return Nazev + ",\n " + Movie.ToString() + "\nSedadlo: "+Sedadlo;
        }

        public override string ToString()
        {
            string s = "";
            foreach(Sedadlo sed in sedadla)
            {
                s += sed.ToString()+", ";
            }
            return Nazev + ",\n" + Movie.ToString() + "\nSedadla: " + s;
        }






    }
}
