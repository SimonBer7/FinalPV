using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCinema
{
    internal class Sedadlo
    {

        private int cislo;

        public int Cislo
        {
            get { return cislo; }
            set { cislo = value; }
        }

        public Sedadlo(int c)
        {
            Cislo = c;
        }

        public override string ToString()
        {
            return ""+Cislo; 
        }

    }
}
