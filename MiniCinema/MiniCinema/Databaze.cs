﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.IdentityModel.Protocols;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;

namespace MiniCinema
{
    internal class Databaze
    {

        private static SqlConnection connection = null;
        private bool isConnected = false;
        private string pathMovies = "movies.csv";               //csv soubor, ze kterého načítám filmy
        private string email = "miniCinemA1234@gmail.com";      //email, ze kterého se posílaj emaily
        private string password = "rxplyvaqxnoknunc";
        private List<Zakaznik> zakaznici;
        private List<Film> movies;
        private List<Sedadlo> sedadla;
        private List<Promitani> saly;
        private List<Objednavka> objednavky;
        private List<Promitani> rezSaly;
        private Zakaznik prihlasenyZakaznik;

        public Zakaznik PrihlasenyZakaznik
        {
            get { return prihlasenyZakaznik; }
            set { prihlasenyZakaznik = value; }
        }

        public List<Promitani> RezSaly
        {
            get { return rezSaly; }
            set { rezSaly = value; }
        }

        public List<Sedadlo> Sedadla
        {
            get { return sedadla; }
            set { sedadla = value; }
        }

        public List<Zakaznik> Zakaznici
        {
            get { return zakaznici; }
            set { zakaznici = value; }
        }

        public List<Objednavka> Objednavky
        {
            get { return this.objednavky; }
            set { this.objednavky = value;}
        }

        public string Email
        {
            get { return email; } set
            {
                email = value;
            }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        public List<Promitani> Saly
        {
            get { return saly; }
            set { saly = value; }
        }

        public List<Film> Movies
        {
            get { return movies; }
            set { movies = value; }
        }

        public string PathMovies
        {
            get { return pathMovies; }
            set { pathMovies = value; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }

        /// <summary>
        /// Klasický konstruktor, kde využívám metody, které jednotlivě popíši níže
        /// </summary>
        public Databaze()
        {
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
                connection.Open();
                IsConnected = true;
                zakaznici = new List<Zakaznik>();
                movies = new List<Film>();
                sedadla = new List<Sedadlo>();
                saly = new List<Promitani>();
                objednavky = new List<Objednavka>();
                rezSaly = new List<Promitani>();
                GetMoviesFromFile();
                CheckMoviesInDB();
                connection.Close();
                CentralCheckDB();
                RemoveAlreadyReserved();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error with database: " + ex.Message);
            }
        }

        /// <summary>
        /// Metoda, ktera vrací true/false podle připojení do databáze
        /// </summary>
        /// <returns></returns>
        public bool CheckConnectu()
        {
            if (IsConnected == true) { return true; }
            else
            {
                return false;
            }
        }

        private static string ReadSettings(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "Not Found";
            return result;
        }


        public static void CloseConnection()
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch { }
            finally
            {
                connection = null;
            }
        }


        /// <summary>
        /// Metoda na executování příchozích stringu(příkazů)
        /// </summary>
        /// <param name="vloz"></param>
        public void CentralMethod(string vloz)
        {
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
                connection.Open();

                using (SqlCommand command = new SqlCommand(vloz, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            catch (SqlException ex)
            {
                Console.WriteLine("Error with database: " + ex.Message);
            }
        }

        /// <summary>
        /// Obsáhlejší metoda, která čte data z databáze a podle toho přidává/odebírá např. sály a zákazníky
        /// </summary>
        public void CentralCheckDB()
        {
            string selectZak = "select * from zakaznik;";
            using SqlCommand command = new SqlCommand(selectZak, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string jmeno = (string)reader["jmeno"];
                string prijmeni = (string)reader["prijmeni"];
                string email = (string)reader["email"];
                string heslo = (string)reader["heslo"];
                Zakaznik z = new Zakaznik(jmeno, prijmeni, email, heslo);
                Zakaznici.Add(z);
            }
            connection.Close();

            string selectFilm = "select * from film;";
            using SqlCommand command2 = new SqlCommand(selectFilm, connection);
            connection.Open();
            using SqlDataReader reader2 = command2.ExecuteReader();
            while (reader2.Read())
            {
                string nazev = (string)reader2["nazev"];
                string cena = (string)reader2["cena"];
                string delka = (string)reader2["delka"];
                Film f = new Film(nazev, cena, delka);
            }
            connection.Close();
            

            string selectSedadlo = "select * from sedadlo;";
            using SqlCommand command3 = new SqlCommand(selectSedadlo, connection);
            connection.Open();
            using SqlDataReader reader3 = command3.ExecuteReader();
            while (reader3.Read())
            {
                int cislo = (int)reader3["cislo"];
                Sedadlo s = new Sedadlo(cislo);
                Sedadla.Add(s);
            }
            connection.Close();

            string selectProm = "select * from promitani;";
            using SqlCommand command4 = new SqlCommand(selectProm, connection);
            connection.Open();
            using SqlDataReader reader4 = command4.ExecuteReader();
            while (reader4.Read())
            {
                string nazev = (string)reader4["nazev"];
                int film_id = (int)reader4["film_id"];
                int sedadlo_id = (int)reader4["sedadlo_id"];
                Film film = null;
                Sedadlo s = null;

                for (int i = 1; i <= Movies.Count; i++)
                {
                    if (i == film_id)
                    {
                        film = Movies[i - 1];
                        
                    }
                }

                for(int i = 1; i<= Sedadla.Count; i++)
                {
                    if(i ==  sedadlo_id)
                    {
                        s = Sedadla[i - 1];
                    }
                }
                Promitani p = new Promitani(nazev, film, s);
                RezSaly.Add(p);
            }
            connection.Close();


            string selectObj = "select * from objednavka;";
            using SqlCommand command5 = new SqlCommand(selectObj, connection);
            connection.Open();
            using SqlDataReader reader5 = command5.ExecuteReader();
            while (reader5.Read())
            {
                int cisloObj = (int)reader5["cisloObj"];
                int zakaznik_id = (int)reader5["zakaznik_id"];
                int promitani_id = (int)reader5["promitani_id"];
                Zakaznik zak = null;
                Promitani promitani = null;

                for (int i = 1; i <= Zakaznici.Count; i++)
                {
                    if (i == zakaznik_id)
                    {
                        zak = Zakaznici[i - 1];
                        
                    }
                }

                for (int i = 1; i <= RezSaly.Count; i++)
                {
                    if (i == promitani_id)
                    {
                        promitani = RezSaly[i - 1];
                        
                    }
                }
                Objednavka o = new Objednavka(cisloObj, zak, promitani);
                Objednavky.Add(o);
            }
            connection.Close();
            

        }


        /// <summary>
        /// Metoda, která odebere ze sálů sedadla, která jsou již rezervovaná
        /// </summary>
        public void RemoveAlreadyReserved()
        {
            for (int i = 0; i < RezSaly.Count; i++)
            {
                for (int j = 0; j < Saly.Count; j++)
                {
                    if (RezSaly[i].Nazev == Saly[j].Nazev)
                    {
                        Saly[j].RemoveSedadlo(RezSaly[i].Sedadlo);
                    }
                }
            }
        }


        /// <summary>
        /// Metoda, která dropne všechny tabulky v databázi a následně je hned vytvoří a tím databázi "resetuji"
        /// </summary>
        public void ResetTable()
        {
            
            List<string> drops = new List<string>();
            List<string> creates = new List<string>();

            string dropZak = "drop table zakaznik;";
            string dropSed = "drop table sedadlo;";
            string dropFilm = "drop table film;";
            string dropProm = "drop table promitani;";
            string dropObj = "drop table objednavka;";

            string createZak = "create table zakaznik(id int primary key identity(1,1),jmeno varchar(30) not null,prijmeni varchar(30) not null,email varchar(50) not null check(email like '%@%'),heslo varchar(30) not null);";
            string createSed = "create table sedadlo(id int primary key identity(1,1),cislo int not null check(cislo > 0));";
            string createFilm = "create table film(id int primary key identity(1,1),nazev varchar(30) not null,cena varchar(20) not null,delka varchar(20) not null);";
            string createProm = "create table promitani(id int primary key identity(1,1),nazev varchar(30) not null,film_id int not null foreign key references film(id),sedadlo_id int not null foreign key references sedadlo(id));";
            string createObj = "create table objednavka(id int primary key identity(1,1),cisloObj int not null,zakaznik_id int not null references zakaznik(id),promitani_id int not null references promitani(id));";

            drops.Add(dropObj);
            drops.Add(dropProm);
            drops.Add(dropFilm);
            drops.Add(dropSed);
            drops.Add(dropZak);


            for (int i = 0; i < drops.Count; i++)
            {
                this.CentralMethod(drops[i]);
            }

            creates.Add(createZak);
            creates.Add(createSed);
            creates.Add(createFilm);
            creates.Add(createProm);
            creates.Add(createObj);

            for (int i = 0; i < creates.Count; i++)
            {
                this.CentralMethod(creates[i]);
            }
            
        }


        /// <summary>
        /// Metoda, která hlídá, jestli v db již jsou filmy, pokud ne vloží je tam
        /// </summary>
        public void CheckMoviesInDB()
        {
            string selectFilm = "select * from film;";
            using SqlCommand command = new SqlCommand(selectFilm, connection);
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return;
            }
            else
            {
                foreach (Film film in Movies)
                {
                    InsertFilm(film);
                }
            }
            
        }


        /// <summary>
        /// Metoda, ve které získám filmy z csv souboru + práce s nimi
        /// </summary>
        public void GetMoviesFromFile()
        {
            using (StreamReader reader = new StreamReader(PathMovies))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] tmp = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    Film f = new Film(tmp[0], tmp[1], tmp[2]);
                    Movies.Add(f);
                }
            }
            
            for (int i = 0; i < movies.Count; i++)
            {
                Saly.Add(new Promitani("Sal " + (i + 1), movies[i]));
            }
        }


        /// <summary>
        /// Metoda sloužící k vypsání filmů
        /// </summary>
        /// <returns></returns>
        public string PrintMovies()
        {
            string print = "Filmy =>\n";
            int index = 1;
            for(int i = 0; i < movies.Count; i++)
            {
                if (Saly[i].Sedadla.Count == 0)
                {
                    Saly.Remove(Saly[i]);
                }
                print += index + ") " + movies[i]+"\n";
                index++;
            }
            return print;
        }


        /// <summary>
        /// Metoda, která podle indexu vrátí hledané promítání
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Promitani PrintPromitani(int index)
        {
            
            Promitani print = new Promitani();
            for (int i = 0; i < saly.Count; i++)
            {
                if (Saly[i].Sedadla.Count == 0)
                {
                    Saly.Remove(Saly[i]);
                }
                if (i == index)
                {
                    print = saly[index];
                }
            }
            return print;
        }


        /// <summary>
        /// Metoda pro uložení objednávky
        /// </summary>
        /// <param name="o"></param>
        public void SaveObjednavka(Objednavka o)
        {
            Objednavky.Add(o);
        }   

        

        /// <summary>
        /// Metoda, která vrací počet => index (počet řádků v db v jednotlivé tabulce)
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int ReadIndex(string table)
        {
            int index = 0;
            string select = "select * from "+table+";";
            using SqlCommand command = new SqlCommand(select, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                index++;
            }
            connection.Close();
            return index;
        }


        /// <summary>
        /// Metoda pro zaslání emailu
        /// </summary>
        /// <param name="z"></param>
        public void SendEmail(Zakaznik z)
        {
            
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(Email);
            mailMessage.Subject = "Objednávka lístků v MiniCinema";
            mailMessage.To.Add(new MailAddress(z.Email));
            mailMessage.Body = "<html><body>"+ ReadObjednavkaByZak(z) + "</body></html>";
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Email, Password),
                EnableSsl = true,
            };
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při odesílání e-mailu: " + ex.Message);
            }
        }

        /// <summary>
        /// Log-In => přihlášení
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool Login(Zakaznik z)
        {
            foreach (Zakaznik zak in Zakaznici)
            {
                if (z.Email == zak.Email && z.Heslo == zak.Heslo)
                {
                    PrihlasenyZakaznik = z;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Zde kontroluji, aby v db nebyly stejné emaily => pro lepší přehlednost
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool CheckEmails(Zakaznik z)
        {
            for (int i = 0; i < Zakaznici.Count; i++)
            {
                if (Zakaznici[i].Email == z.Email)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// CRUD Zakaznik
        /// </summary>
        /// <param name="z"></param>
        public void InsertZakaznik(Zakaznik z)
        {
            Zakaznici.Add(z);
            string vlozZak = "insert into zakaznik(jmeno, prijmeni, email, heslo) values ('" + z.Jmeno + "', '" + z.Prijmeni + "', '" + z.Email + "', '" + z.Heslo + "');";
            using (SqlCommand command = new SqlCommand(vlozZak, connection))
            {
                command.Parameters.AddWithValue("@jmeno", z.Jmeno);
                command.Parameters.AddWithValue("@prijmeni", z.Prijmeni);
                command.Parameters.AddWithValue("@email", z.Email);
                command.Parameters.AddWithValue("@heslo", z.Heslo);
                this.CentralMethod(vlozZak);
            }
        }

        public string ReadZakaznik()
        {
            string vypis = "";
            string selectZak = "select * from zakaznik;";
            using SqlCommand command = new SqlCommand(selectZak, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {   
                string jmeno = (string)reader["jmeno"];
                string prijmeni = (string)reader["prijmeni"];
                string email = (string)reader["email"];
                string heslo = (string)reader["heslo"];
                Zakaznik z = new Zakaznik(jmeno, prijmeni, email, heslo);
                vypis += z.ToString() + "\n";
            }
            connection.Close();
            return vypis;
        }

        public void UpdateZakaznik(Zakaznik z, string novy)
        {
            int zakaznik_id = 0;
            for(int i = 0; i< Zakaznici.Count; i++)
            {
                if (Zakaznici[i].Email == z.Email && Zakaznici[i].Heslo == z.Heslo)
                {
                    zakaznik_id = (i+1); 
                }
            }
            string update = "update zakaznik set email = '" + novy + "' where zakaznik.id = "+zakaznik_id+";";
            this.CentralMethod(update);
        }

        public void DeleteZakaznik()
        {
            string del = "delete from zakaznik;";
            this.CentralMethod(del);
        }


        /// <summary>
        /// CRUD Film
        /// </summary>
        /// <param name="f"></param>
        public void InsertFilm(Film f)
        {
            string vlozFilm = "insert into film(nazev, cena, delka) values ('" + f.Nazev + "', '" + f.Cena + "', '" + f.Delka+ "');";
            using (SqlCommand command = new SqlCommand(vlozFilm, connection))
            {
                command.Parameters.AddWithValue("@nazev", f.Nazev);
                command.Parameters.AddWithValue("@cena", f.Cena);
                command.Parameters.AddWithValue("@delka", f.Delka);
                this.CentralMethod(vlozFilm);
            }
        }

        public string ReadFilm()
        {
            string vypis = "";
            string selectFilm = "select * from film;";
            using SqlCommand command = new SqlCommand(selectFilm, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string nazev = (string)reader["nazev"];
                string cena = (string)reader["cena"];
                string delka = (string)reader["delka"];
                Film f = new Film(nazev, cena, delka);
                vypis += f.ToString() + "\n";
            }
            connection.Close();
            return vypis;
        }

        public void UpdateFilm(int novy)
        {
            string update = "update film set cena = " + novy + " where film.id = 1;";
            this.CentralMethod(update);
        }

        public void DeleteFilm()
        {
            string del = "delete from film;";
            this.CentralMethod(del);
        }


        /// <summary>
        /// CRUD Sedadlo
        /// </summary>
        /// <param name="s"></param>
        public void InsertSedadlo(Sedadlo s)
        {
            string vlozSedadlo = "insert into sedadlo(cislo) values (" + s.Cislo + ");";
            using (SqlCommand command = new SqlCommand(vlozSedadlo, connection))
            {
                command.Parameters.AddWithValue("@cislo", s.Cislo);
                this.CentralMethod(vlozSedadlo);
            }
        }

        public string ReadSedadlo()
        {
            string vypis = "";
            string selectSedadlo = "select * from sedadlo;";
            using SqlCommand command = new SqlCommand(selectSedadlo, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int cislo = (int)reader["cislo"];
                Sedadlo s = new Sedadlo(cislo);
                vypis += s.ToString() + "\n";
            }
            connection.Close();
            return vypis;
        }

        public void UpdateSedadlo(int novy)
        {
            string update = "update sedadlo set cislo = " + novy + " where sedadlo.id = 1;";
            this.CentralMethod(update);
        }

        public void DeleteSedadlo()
        {
            string del = "delete from sedadlo;";
            this.CentralMethod(del);
        }


        /// <summary>
        /// CRUD Promitani
        /// </summary>
        /// <param name="p"></param>
        /// <param name="film_id"></param>
        /// <param name="sedadlo_id"></param>
        public void InsertPromitani(Promitani p, int film_id, int sedadlo_id)
        {
            string vlozProm = "insert into promitani(nazev, film_id, sedadlo_id) values ('" + p.Nazev + "', " + (film_id+1) + ", " + sedadlo_id + ");";
            using (SqlCommand command = new SqlCommand(vlozProm, connection))
            {
                command.Parameters.AddWithValue("@nazev", p.Nazev);
                command.Parameters.AddWithValue("@film_id", film_id);
                command.Parameters.AddWithValue("@sedadlo_id", sedadlo_id);
                
                this.CentralMethod(vlozProm);
            }
        }

        public string ReadPromitani()
        {
            string vypis = "";
            string selectProm = "select * from promitani;";
            using SqlCommand command = new SqlCommand(selectProm, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string nazev = (string)reader["nazev"];
                int film_id = (int)reader["film_id"];

                
                for(int i = 1; i<= Movies.Count; i++)
                {
                    if(i == film_id)
                    {
                        Film film = Movies[i-1];
                        vypis += film.ToString() + "\n";
                    }
                }
            }
            connection.Close();
            return vypis;
        }

        public void UpdatePromitani(string novy)
        {
            string update = "update promitani set nazev = " + novy + " where promitani.id = 1;";
            this.CentralMethod(update);
        }

        public void DeletePromitani()
        {
            string del = "delete from promitani;";
            this.CentralMethod(del);
        }


        /// <summary>
        /// CRUD Objednavka
        /// </summary>
        /// <param name="o"></param>
        public void InsertObj(Objednavka o)
        {
            string vlozProm = "insert into objednavka(cisloObj, zakaznik_id, promitani_id) values (" + o.CisloObj + ", " + (ReadIndex("zakaznik")) + ", "+ (ReadIndex("promitani")) + ");";
            using (SqlCommand command = new SqlCommand(vlozProm, connection))
            {
                command.Parameters.AddWithValue("@cisloObj", o.CisloObj);
                command.Parameters.AddWithValue("@zakaznik_id", ReadIndex("zakaznik"));
                command.Parameters.AddWithValue("@promitani_id", ReadIndex("promitani"));

                this.CentralMethod(vlozProm);
            }
        }

        public string ReadObjednavka()
        {
            string vypis = "";
            string selectObj = "select * from objednavka;";
            using SqlCommand command = new SqlCommand(selectObj, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int cisloObj = (int)reader["cisloObj"];
                int zakaznik_id = (int)reader["zakaznik_id"];
                int promitani_id = (int)reader["promitani_id"];

                vypis += "\nObjednavka c." + cisloObj+ "\n-----------------------------------------------------------------------------\n"; 
                for (int i = 1; i <= Zakaznici.Count; i++)
                {
                    if (i == zakaznik_id)
                    {
                        Zakaznik zak = Zakaznici[i - 1];
                        vypis += zak.ToString() + "\n";
                    }
                }

                for (int i = 1; i <= Saly.Count; i++)
                {
                    if (i == promitani_id)
                    {
                        Promitani promitani = Saly[i - 1];
                        vypis += promitani.ToString();
                    }
                }
            }
            connection.Close();
            return vypis;
        }

        public string ReadObjednavkaByZak(Zakaznik z)
        {
            string vypis = "";
            Zakaznik zak = null;    
            string selectObj = "select objednavka.cisloObj, objednavka.zakaznik_id, objednavka.promitani_id from objednavka inner join zakaznik on zakaznik.id = objednavka.zakaznik_id where zakaznik.email = '" + z.Email + "';";
            using SqlCommand command = new SqlCommand(selectObj, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int cisloObj = (int)reader["cisloObj"];
                int zakaznik_id = (int)reader["zakaznik_id"];
                int promitani_id = (int)reader["promitani_id"];

                vypis += "\nObjednavka c." + cisloObj + "\n-----------------------------------------------------------------------------\n";
                for (int i = 1; i <= Zakaznici.Count; i++)
                {
                    if (i == zakaznik_id)
                    {
                        zak = Zakaznici[i - 1];
                        vypis += zak.ToString() + "\n";
                    }
                }

                for (int i = 1; i <= RezSaly.Count; i++)
                {
                    if (i == promitani_id && zak.Email == Zakaznici[i-1].Email)
                    {
                        Promitani promitani = RezSaly[i - 1];
                        vypis += promitani.Vypis() + "\n-----------------------------------------------------------------------------\n";
                    }
                }
            }
            connection.Close();
            return vypis;
        }

        public string ReadObjednavkaByCisloObj(int cislo)
        {
            string vypis = "";
            string selectObj = "select * from objednavka where objednavka.cisloObj = "+cislo+";";
            using SqlCommand command = new SqlCommand(selectObj, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int cisloObj = (int)reader["cisloObj"];
                int zakaznik_id = (int)reader["zakaznik_id"];
                int promitani_id = (int)reader["promitani_id"];


                vypis += "\nObjednavka c." + cisloObj + "\n-----------------------------------------------------------------------------\n";
                for (int i = 1; i <= Zakaznici.Count; i++)
                {
                    if (i == zakaznik_id)
                    {
                        Zakaznik zak = Zakaznici[i - 1];
                        vypis += zak.ToString() + "\n";
                    }
                }

                for (int i = 1; i <= Saly.Count; i++)
                {
                    if (i == promitani_id)
                    {
                        Promitani promitani = Saly[i - 1];
                        vypis += promitani.ToString()+ "\n-----------------------------------------------------------------------------\n";
                    }
                }
            }
            connection.Close();
            return vypis;
        }


        public void UpdateObjednavka(int cisloObj, int novyCislo)
        {
            string update = "update objednavka set cisloObj = " + novyCislo + " where objednavka.cisloObj = "+cisloObj+";";
            this.CentralMethod(update);
        }

        public void DeleteObjenavka(int cisloObj)
        {

            string del = "delete from objednavka where objednavka.cisloObj = " + cisloObj + ";";
            this.CentralMethod(del);
            for (int i = 0; i < Objednavky.Count; i++)
            {
                if (Objednavky[i].CisloObj == cisloObj)
                {
                    Objednavky.Remove(Objednavky[i]);
                }
            }
        }
    }
}
