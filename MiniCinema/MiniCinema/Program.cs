using System.Text.RegularExpressions;

namespace MiniCinema
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Stiskněte Enter pro vstup do programu MiniCinema...");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyInfo = Console.ReadKey();
            }

            ///Vytvoreni db
            Databaze databaze = new Databaze();

            ///Pripojeni do db
            if (databaze.CheckConnectu() == true)
            {
                Console.WriteLine("----------------------------------" + "Pripojeno" + "----------------------------------");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Error, with connecting to database");
                Environment.Exit(0);
            }
            Console.WriteLine("\nVítejte v programu =>\n-----------------------------------------------------------------------------\n");

            ///Hlavní cyklus
            bool isRunning = true;
            while(isRunning)
            {
                Console.Write("1) Objednat lístky\n2) Log-In\n3) Reset DB\n4) Konec");
                int volba;
                do
                {
                    Console.Write("\nVaše volba (1-4): ");
                } while (!int.TryParse(Console.ReadLine(), out volba) || volba < 1 || volba > 4);
                volba -= 1;
                Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                switch (volba)
                {
                    case 0:
                        ObjednaniListku(databaze);      ///Volání statické metody pro objednání lístků
                        databaze = new Databaze();      ///Reload db
                        Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                        break;
                    case 1:
                        LogIn(databaze);                ///Volání statické metody pro přihlášení
                        databaze = new Databaze();      ///Reload db
                        Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                        break;
                    case 2:
                        databaze.ResetTable();          ///Volání metody, která dropne a creatne tabulky v db
                        databaze = new Databaze();      ///Reload db
                        Console.WriteLine("----------------------------------" + "Resetováno" + "---------------------------------");
                        break;
                    case 3:
                        isRunning = false;              ///Ukončí cyklus => program
                        break;
                }
            }
        }

        /// <summary>
        /// Statická metod pro objednání lístků
        /// </summary>
        /// <param name="databaze"></param>
        public static void ObjednaniListku(Databaze databaze)
        {
            Console.WriteLine(databaze.PrintMovies());
            int volba;
            do
            {
                Console.Write("\nVaše volba (1-"+databaze.Movies.Count+"): ");
            } while (!int.TryParse(Console.ReadLine(), out volba) || volba < 1 || volba > databaze.Movies.Count);
            volba -= 1;
            Console.WriteLine("\n-----------------------------------------------------------------------------\n");
            switch (volba)
            {
                case 0:
                    InfoOdUzi(databaze, volba);
                    break;
                case 1:
                    InfoOdUzi(databaze, volba);
                    break;
                case 2:
                    InfoOdUzi(databaze, volba);
                    break;
                case 3:
                    InfoOdUzi(databaze, volba);
                    break;
                case 4:
                    InfoOdUzi(databaze, volba);
                    break;
            }
        }

        /// <summary>
        /// Statická metoda, ve které získávám info o uživateli, vložím data do db a nakonec pošlu email na zadaný email 
        /// </summary>
        /// <param name="databaze"></param>
        /// <param name="volba"></param>
        public static void InfoOdUzi(Databaze databaze, int volba)
        {
            Console.Write("Zadej jmeno: ");
            string jmeno = Console.ReadLine();
            while (jmeno.Length < 3)
            {
                jmeno = Console.ReadLine();
            }
            Console.Write("Zadej prijmeni: ");
            string prijmeni = Console.ReadLine();
            while (prijmeni.Length < 3)
            {
                prijmeni = Console.ReadLine();
            }
            Console.Write("Zadej email: ");
            string email = Console.ReadLine();
            string pattern = @"^[A-Za-z0-9]+([._-][A-Za-z0-9]+)*@[A-Za-z0-9]+\.[A-Za-z]+$";
            bool isValidEmail = Regex.IsMatch(email, pattern);
            while(!isValidEmail)
            {
                email = Console.ReadLine();
            }
            Console.Write("Zadej heslo: ");
            string heslo = Console.ReadLine();
            while (heslo.Length < 3)
            {
                heslo = Console.ReadLine();
            }
            Zakaznik z = new Zakaznik(jmeno, prijmeni, email, heslo);

            if (databaze.CheckEmails(z))
            {
                Console.WriteLine("Nelze vytvorit ucet...");
                return;
            }
            else
            {
                Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                Console.WriteLine(databaze.PrintPromitani(volba));

                int choice;
                do
                {
                    Console.Write("\nVýběr sedadla (1-5): ");
                } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5);
                Sedadlo s = new Sedadlo(choice);
                choice -= 1;

                databaze.Saly[volba].RemoveSedadlo(s);

                Objednavka o = new Objednavka(z, databaze.PrintPromitani(choice));

                databaze.InsertZakaznik(z);
                databaze.InsertSedadlo(s);
                databaze.InsertPromitani(databaze.PrintPromitani(volba), volba, databaze.ReadIndex("sedadlo"));
                databaze.InsertObj(o);
                databaze.SaveObjednavka(o);
                Console.WriteLine("----------------------------------" + "Objednano" + "----------------------------------");

                databaze.SendEmail(z);
            }
        }


        /// <summary>
        /// Statická metoda, ve které se uživatel může přihlásit a následně provádět další operace
        /// </summary>
        /// <param name="d"></param>
        public static void LogIn(Databaze d)
        {
            Console.Write("Zadej jmeno: ");
            string jmeno = Console.ReadLine();
            while (jmeno.Length < 3)
            {
                jmeno = Console.ReadLine();
            }
            Console.Write("Zadej prijmeni: ");
            string prijmeni = Console.ReadLine();
            while (prijmeni.Length < 3)
            {
                prijmeni = Console.ReadLine();
            }
            Console.Write("Zadej email: ");
            string email = Console.ReadLine();
            string pattern = @"^[A-Za-z0-9]+([._-][A-Za-z0-9]+)*@[A-Za-z0-9]+\.[A-Za-z]+$";
            bool isValidEmail = Regex.IsMatch(email, pattern);
            while (!isValidEmail)
            {
                email = Console.ReadLine();
            }
            Console.Write("Zadej heslo: ");
            string heslo = Console.ReadLine();
            while (heslo.Length < 3)
            {
                heslo = Console.ReadLine();
            }
            Zakaznik z = new Zakaznik(jmeno, prijmeni, email, heslo);

                if (!d.Login(z))
                {
                    Console.WriteLine("Neuspesne prihlaseni...");
                    return;
                }
                else
                {
                Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                Console.WriteLine("\n1) Vypis objednavek\n2) Edit\n3) Delete objednavky");

                int choice;
                do
                {
                     Console.Write("\nVáš výběr (1-3): ");
                } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3);
                choice -= 1;
                Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                switch (choice)
                {
                     case 0:
                         Console.WriteLine(d.ReadObjednavkaByZak(z));
                         break;

                     case 1:
                         Console.WriteLine("\n1)Update Zakaznik\n2)Update Objednavky\n");

                         int volba;
                         do
                         {
                             Console.Write("\nVáš výběr (1-2): ");
                         } while (!int.TryParse(Console.ReadLine(), out volba) || volba < 1 || volba > 2);
                         volba -= 1;
                        Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                        switch (volba)
                        {
                            case 0:
                                Console.Write("Zadejte novy email: ");
                                string newEmail = Console.ReadLine();
                                d.UpdateZakaznik(z, newEmail);
                                break;
                            case 1:
                                Console.Write("Zadej stare cislo objednavky: ");
                                int oldCisloObj = Int32.Parse(Console.ReadLine());
                                Console.Write("Zadej nove cislo objednavky: ");
                                int newCisloObj = Int32.Parse(Console.ReadLine());
                                d.UpdateObjednavka(oldCisloObj, newCisloObj);
                                break;
                        }
                         break;

                     case 2:

                         int cisloObj;
                        Console.WriteLine("\n-----------------------------------------------------------------------------\n");
                        do
                        {
                            Console.Write("\nZadej cislo objednavky, kterou chcete smazat: ");
                        } while (!int.TryParse(Console.ReadLine(), out cisloObj) || cisloObj < 0);

                        d.DeleteObjenavka(cisloObj);
                        break;
                    }
                }
        }
    }
}
