namespace cegesauto
{
    public class Forgalom
    {
        public int Nap { get; set; }
        public string OOPP { get; set; }
            public int Ora { get; set; }
            public int Perc { get; set; }

        public string Rendszam { get; set; }
        public int Azonosito { get; set; }
        public int Km { get; set; }
        public bool KiBe { get; set; }
    }

    class Program
    {
        static List<Forgalom> forgalom = new List<Forgalom>();
        static void Main(string[] args)
        {
            //1. feladat
            Beolvas();

            //2. feladat
            Feladat2();

            //3. feladat
            Feladat3();

            //4. feladat
            Feladat4();

            //5. feladat
            Feladat5();

            //6. feladat
            Feladat6();

            //7. feladat
            Feladat7();

            Console.ReadLine();
        }
        private static void Beolvas()
        {
            StreamReader sr = new StreamReader(@"autok.txt");

            while (!sr.EndOfStream)
            {
                string[] sor = sr.ReadLine().Split(' ');
                Forgalom auto = new Forgalom();

                auto.Nap = Convert.ToInt32(sor[0]);

                auto.OOPP = sor[1];
                auto.Ora = Convert.ToInt32(sor[1].Substring(0, 2));
                auto.Perc = Convert.ToInt32(sor[1].Substring(3, 2));

                auto.Rendszam = sor[2];
                auto.Azonosito = Convert.ToInt32(sor[3]);
                auto.Km = Convert.ToInt32(sor[4]);
                auto.KiBe = sor[5] == "1" ? true : false;

                forgalom.Add(auto);
            }

            sr.Close();
        }
        private static void Feladat2()
        {
            Console.WriteLine("2. feladat");

            var utolso = forgalom.Where(x => !x.KiBe).LastOrDefault();

            Console.WriteLine($"{utolso.Nap}. nap rendszám: {utolso.Rendszam}");
        }
        private static void Feladat3()
        {
            Console.WriteLine("3. feladat");

            Console.Write("Nap: ");
            int bekertNap = Convert.ToInt32(Console.ReadLine());

            var szures = forgalom.Where(x => x.Nap == bekertNap).OrderBy(x => x.OOPP).ToList();

            Console.WriteLine($"Forgalom a(z) {bekertNap}. napon: ");

            foreach (var item in szures)
            {
                string irany = "";
                if (!item.KiBe) irany = "ki";
                else irany = "be";
                Console.WriteLine($"{item.OOPP} {item.Rendszam} {irany}");
            }
        }
        private static void Feladat4()
        {
            Console.WriteLine("4. feladat");

            int count = 0;
            var szures = from x in forgalom
                         orderby x.Rendszam, x.Nap, x.OOPP
                         group x by x.Rendszam into g
                         select new { Azonosito = g.Key, Utolso = g.Select(x => x.KiBe).LastOrDefault() };

            foreach (var group in szures)
            {
                if (!group.Utolso) count++;
            }

            Console.WriteLine($"A hónap végén {count} autót nem hoztak vissza");
        }
        private static void Feladat5()
        {
            Console.WriteLine("5. feladat");

            var csoport = forgalom.GroupBy(x => x.Rendszam).OrderBy(x => x.Key).ToList();

            foreach (var group in csoport)
            {
                int osszKm = 0, kezdo = 0, veg = 0;

                foreach (var item in group)
                {
                    if (!item.KiBe) kezdo = item.Km;
                    else if (item.KiBe) veg = item.Km;

                    if (kezdo != 0 && veg != 0)
                    {
                        osszKm += veg - kezdo;
                        kezdo = 0;
                        veg = 0;
                    }
                }

                Console.WriteLine($"{group.Key} {osszKm} km");
            }
        }
        private static void Feladat6()
        {
            Console.WriteLine("6. feladat");

            var csoport = forgalom.GroupBy(x => x.Azonosito).ToList();
            int maxAzonosito = 0, maxKm = 0;

            foreach (var group in csoport)
            {
                int osszKm = 0, kezdo = 0, veg = 0;

                foreach (var item in group)
                {
                    if (!item.KiBe) kezdo = item.Km;
                    else if (item.KiBe) veg = item.Km;

                    if (kezdo != 0 && veg != 0)
                    {
                        osszKm = veg - kezdo;

                        if (osszKm >= maxKm)
                        {
                            maxKm = osszKm;
                            maxAzonosito = group.Key;
                        }

                        kezdo = 0;
                        veg = 0;
                    }
                }
            }

                Console.WriteLine($"Leghoszabb út: {maxKm} km, személy: {maxAzonosito}");
        }
        private static void Feladat7()
        {
            Console.WriteLine("7. feladat");
            Console.Write("Rendszám: ");
            string bekertRendszam = Console.ReadLine().ToUpper();

            var csoport = forgalom.Where(x => x.Rendszam == bekertRendszam).ToList();
            StreamWriter sw = new StreamWriter($@"{bekertRendszam}_menetlevel.txt");

            int kezdoKm = 0, vegKm = 0,
                kezdoNap = 0, vegNap = 0,
                utolso = csoport.IndexOf(csoport.LastOrDefault());

            string kezdoIdo = "", vegIdo = "";

            foreach (var item in csoport)
            {
                bool swDone = false;

                if (!item.KiBe) 
                { 
                    kezdoKm = item.Km; 
                    kezdoNap = item.Nap;
                    kezdoIdo = item.OOPP;
                }
                else if (item.KiBe)
                { 
                    vegKm = item.Km; 
                    vegNap = item.Nap;
                    kezdoIdo = item.OOPP;
                }

                if (kezdoKm != 0 && vegKm != 0)
                {
                    sw.WriteLine($"{item.Azonosito}\t{kezdoNap.ToString("00")}. {kezdoIdo}\t{kezdoKm} km\t{vegNap.ToString("00")}. {vegIdo}\t{vegKm} km");
                    swDone = true;
                }
                else if (kezdoKm != 0 && vegKm == 0 && csoport.IndexOf(item) == utolso)
                {
                    sw.WriteLine($"{item.Azonosito}\t{kezdoNap.ToString("00")}. {kezdoIdo}\t{kezdoKm} km");
                    swDone = true;
                }

                if (swDone) { kezdoKm = 0; vegKm = 0; }
            }

            Console.WriteLine("Menetlevél kész.");
            sw.Close();
        }
    }
}