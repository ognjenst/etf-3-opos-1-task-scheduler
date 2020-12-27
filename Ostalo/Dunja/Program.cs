using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak1.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            ////Console.WriteLine("Unesite maksimalan broj niti kojima rasporedjivac raspolaze: ");
            ////int n = Convert.ToInt32(Console.ReadLine());
            ////Console.WriteLine("Unesite ukupno vrijeme izvrsenja svih zadataka proslijedjenih rasporedjivacu: (u sek) ");
            ////int t = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("RASPOREDJIVAC BEZ PRIORITETA:\n");
            //Rasporedjivac RasporedjivacBezPrioriteta = new Rasporedjivac(3, 15, 0);    // broj niti i vrijeme trajanja
            //Random rnd = new Random();                                                // 0--> bez prioriteta
            //Console.WriteLine("~ Lista spremnih taskova za izvrsavanje: ");
            ////dodavanje taskova
            //for (int i = 0; i < 5; i++)
            //    RasporedjivacBezPrioriteta.GenerisanjeTaska(i, rnd.Next(1, 5));

            //RasporedjivacBezPrioriteta.Start();

            //Thread.Sleep(5000);

            //for (int i = 5; i < 10; i++)
            //{
            //    RasporedjivacBezPrioriteta.GenerisanjeTaska(i, rnd.Next(1, 5)); // 0--> sa prioritetom
            //    Thread.Sleep(2000);
            //}


            Console.WriteLine("\nRASPOREDJIVAC SA PRIORITETOM:\n");
            Rasporedjivac RasporedjivacSaPrioritetom = new Rasporedjivac(4, 20, 1); // 1--> sa prioritetom
            Random rnd = new Random();
            Console.WriteLine("~ Lista spremnih taskova za izvrsavanje: ");
            //dodavanje taskova
            for (int i = 0; i < 10; i++)
                RasporedjivacSaPrioritetom.GenerisanjeTaska(i, rnd.Next(1, 5));  //unaprijed kreirani taskovi za izvrsavanje
                                                                                 //random generise vrijeme izvrsavanja taska
            RasporedjivacSaPrioritetom.Start();
            Thread.Sleep(5000);


            for (int i = 10; i < 20; i++)
            {
                RasporedjivacSaPrioritetom.GenerisanjeTaska(i, rnd.Next(1, 5)); //taskovi koji pristizu kada je pokrenut rasporedjivac
                Thread.Sleep(2000);
            }

        }
    }
}
