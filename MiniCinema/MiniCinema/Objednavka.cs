using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCinema
{
    internal class Objednavka
    {

        private int cisloObj;
        private Zakaznik zakaznik;
        private Promitani promitani;

        public int CisloObj
        {
            get { return cisloObj; }
            set { cisloObj = value; }
        }

        public Zakaznik Zakaznik
        {
            get { return zakaznik; }
            set { zakaznik = value; }
        }

        public Promitani Promitani
        {
            get { return promitani; }
            set { promitani = value; }
        }


        public Objednavka(Zakaznik z , Promitani p)
        {
            CisloObj = new Random().Next(1000)+1;
            Zakaznik = z;
            Promitani = p;
        }

        public Objednavka(int c, Zakaznik z, Promitani p)
        {
            CisloObj = c;
            Zakaznik = z;
            Promitani = p;
        }


        
        public override string ToString() 
        {
            return "Objednavka c." + CisloObj + "\n------------------\n" + Zakaznik.ToString() + "\n" + Promitani.Vypis();   
        }


    }
}
