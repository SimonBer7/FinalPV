using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCinema
{
    internal class Zakaznik
    {

        private string jmeno;
        private string prijmeni;
        private string email;
        private string heslo;

        public string Jmeno
        {
            get { return jmeno; }
            set { jmeno = value; }
        }

        public string Prijmeni
        {
            get { return prijmeni; }
            set { prijmeni = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Heslo
        {
            get { return heslo; }
            set { heslo = value; }
        }

        public Zakaznik(string j, string p, string e, string h)
        {
            Jmeno = j;
            Prijmeni = p;
            Email = e;
            Heslo = h;
        }


        public string HashingSimulator()
        {
            string hash = "";
            for(int i = 0; i < Heslo.Length; i++)
            {
                hash += "*";
            }
            return hash;
        }

        public override string ToString()
        {
            return "Zakaznik "+Jmeno+" "+Prijmeni+", email: "+Email+", heslo: " + HashingSimulator();
        }
    }
}
