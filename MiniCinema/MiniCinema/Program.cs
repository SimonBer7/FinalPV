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

            Databaze databaze = new Databaze();

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
            Console.WriteLine("\nVítejte v programu =>\n------------------\n");
            //Console.WriteLine("\n"+databaze.CentralCheckDB()+"\n");
            bool isRunning = true;
            while(isRunning)
            {
                Console.Write("1) Objednat lístky\n2) Log-In\n3) Konec\n");
                int volba;
                do
                {
                    Console.Write("Vaše volba (1-3): ");
                } while (!int.TryParse(Console.ReadLine(), out volba) || volba < 1 || volba > 3);
                volba -= 1;

                switch (volba)
                {
                    case 0:
                        ObjednaniListku(databaze);
                        Console.WriteLine("\n-------------------\n");
                        break;
                    case 1:

                        Console.WriteLine("\n-------------------\n");
                        break;
                    case 2:
                        Console.WriteLine("\n-------------------\n");
                        isRunning = false;
                        break;
                }
            }
            

        }


        public static void ObjednaniListku(Databaze databaze)
        {
            Console.WriteLine(databaze.PrintMovies());
            int volba;
            do
            {
                Console.Write("Vaše volba (1-"+databaze.Movies.Count+"): ");
            } while (!int.TryParse(Console.ReadLine(), out volba) || volba < 1 || volba > databaze.Movies.Count);
            volba -= 1; 

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

        public static void InfoOdUzi(Databaze databaze, int volba)
        {
            Console.Write("Zadej jmeno: ");
            string jmeno = Console.ReadLine();
            Console.Write("Zadej prijmeni: ");
            string prijmeni = Console.ReadLine();
            Console.Write("Zadej email: ");
            string email = Console.ReadLine();
            Console.Write("Zadej heslo: ");
            string heslo = Console.ReadLine();
            Zakaznik z = new Zakaznik(jmeno, prijmeni, email, heslo);

            Console.WriteLine(databaze.PrintPromitani(volba));

            int choice;
            do
            {
                Console.Write("Výběr sedadla (1-" + databaze.Saly[volba].Sedadla.Count +"): ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > (databaze.Saly[volba].Sedadla.Count));
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

            //metoda pro poslani emailu
            //databaze.SendEmail(o);
        }




    }
}