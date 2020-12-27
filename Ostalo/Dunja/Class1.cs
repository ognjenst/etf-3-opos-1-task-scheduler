using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Zadatak1
{
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable  //omogucava dupliranje kljuceva u sortiranoj listi
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else
                return result;
        }
    }
    public class Rasporedjivac : System.Threading.Tasks.TaskScheduler, IDisposable
    {
        //private static Queue<Tuple<Action,int>> TaskoviZaIzvrsavanje;
        private static SortedList<int, Tuple<Action, int>> TaskoviZaIzvrsavanje;
        private static List<Tuple<Task, int>> ListaAktivnihTaskova;

        private static int MaxBrojNiti;
        private static int BrojAktivnihNiti = 0;
        private static int UkupnoVrijemeIzvrsavanja;
        private static int Identifikator;                          // 0-> nepreventivno 1-> preventivno
        private Stopwatch time;           //tajmer za ukupno vrijeme

        private Thread KontrolaSlobodnihNiti;
        private Thread KontrolaZavsenihNiti;
        private Thread PrioritetnoDodavanje;

        public int getBrojAktivnihNiti() { return BrojAktivnihNiti; }
        public int getMaxBrojNiti() { return MaxBrojNiti; }
        public double getUkupnoVrijeme() { return time.ElapsedMilliseconds/1000;  }

        public Rasporedjivac(int BrojNiti, int vrijeme, int identifikator)         //konstruktor (broj niti i ukupno vrijeme izvrsavanja)
        {

            MaxBrojNiti = BrojNiti;
            UkupnoVrijemeIzvrsavanja = vrijeme;
            Identifikator = identifikator;
            time = Stopwatch.StartNew();

            //TaskoviZaIzvrsavanje = new Queue<Tuple<Action,int>>();
            TaskoviZaIzvrsavanje = new SortedList<int, Tuple<Action, int>>(new DuplicateKeyComparer<int>());
            ListaAktivnihTaskova = new List<Tuple<Task, int>>();

            KontrolaSlobodnihNiti = new Thread(new ThreadStart(ProvjeraSlobodnih));     //pravljenje novih kontrolnih niti
            KontrolaZavsenihNiti = new Thread(new ThreadStart(ProvjeraZavrsenih));
            PrioritetnoDodavanje = new Thread(new ThreadStart(Prioritetno));

        }
        public void Start()
        {
            KontrolaSlobodnihNiti.Start();                                              //njihovo pokretanje
            KontrolaZavsenihNiti.Start();
            if (Identifikator == 1)
                PrioritetnoDodavanje.Start();
        }

        public void ProvjeraSlobodnih()
        {
            while (true)
            {
                if (time.ElapsedMilliseconds >= (UkupnoVrijemeIzvrsavanja * 1000))
                {
                    Console.WriteLine("Isteklo je ukupno vrijeme izvrsavanja taskova. ");
                    time.Stop();
                    Environment.Exit(0);
                }

                if (BrojAktivnihNiti < MaxBrojNiti)
                {
                    PokretanjeTaska();
                }
            }
        }
        public void ProvjeraZavrsenih()
        {
            while (true)
            {
                if (ListaAktivnihTaskova.Count != 0)
                {
                    foreach (Tuple<Task, int> task in ListaAktivnihTaskova.ToArray())
                    {
                        if (task != null)
                        {
                            if (task.Item1.Status == TaskStatus.RanToCompletion || task.Item1.Status == TaskStatus.Canceled)       //ako je task zavrsen
                            {
                                Interlocked.Decrement(ref BrojAktivnihNiti);    //smanji broj aktivnih niti
                                ListaAktivnihTaskova.Remove(task);             //ukloni se zavsen task iz liste aktivnih taskova
                            }
                        }
                    }

                }
            }
        }
        public void Prioritetno() // ako se generise broj 1, dodaje zadatak na pocetak liste, tako sto mu stavlja prioritet 0
        {
            Random identifikator = new Random();
            while (true)
            {
                Thread.Sleep(2000);
                int n = identifikator.Next(1, 5);
                if (n == 1)
                {
                    TaskoviZaIzvrsavanje.Add(0, new Tuple<Action, int>(new Action(() =>
                    {
                        Stopwatch timer = Stopwatch.StartNew();
                        Console.WriteLine(" \n--> Prioritetni task --> HELLO - Thread {0}\n",
                            Thread.CurrentThread.ManagedThreadId);
                        Thread.Sleep(2000);
                        Console.WriteLine(" Vrijeme izvrsavanja prioritetnog taska : {0}", timer.ElapsedMilliseconds / 1000);
                    }), 5000));
                }

            }
        }
        public void PokretanjeTaska()
        {
            if (TaskoviZaIzvrsavanje.Count != 0)                                                  // ako lista nije prazna
            {
                Tuple<Action, int> akcija = TaskoviZaIzvrsavanje.Values[0];
                Task temp = Task.Factory.StartNew(akcija.Item1, new CancellationTokenSource(akcija.Item2).Token); // omogucen prekid

                ListaAktivnihTaskova.Add(Tuple.Create(temp, akcija.Item2)); //pokrece se prvi task iz liste taskova koje treba izvrsiti
                TaskoviZaIzvrsavanje.RemoveAt(0);
                Interlocked.Increment(ref BrojAktivnihNiti);                                     //povecava se broj aktivnih 
                //TaskInfo();
            }
        }
        static int i = 1;
        static int indeks = 1;
        public void DodajTask(Action akcija)
        {
            if (Identifikator == 0)
            {
                TaskoviZaIzvrsavanje.Add(i++, new Tuple<Action, int>(akcija, 5000));
            }
            else
            {
                Random prioritet = new Random();
                int n = prioritet.Next(1, 5);
                //Taskovi.Add(i++,Tuple.Create(new Action(akcija),5000));
                TaskoviZaIzvrsavanje.Add(n, new Tuple<Action, int>(akcija, 5000));  // svaki task je ogranicen na max 5 sekundi
                Console.WriteLine("--> Dodavanje taska {0} sa prioritetom : {1}", indeks++, n);
            }
        }
        public int TaskInfo()
        {
            Console.WriteLine("Broj taskova za izvrsavanje: {0} ", TaskoviZaIzvrsavanje.Count);
            return TaskoviZaIzvrsavanje.Count;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        //tri funkcije koje je potrebno preklopiti ako koristimo System.Threading.Tasks.TaskScheduler 
        protected override IEnumerable<Task> GetScheduledTasks()  //niz taskova koji su spremni za izvrsavanje
        {
            throw new NotImplementedException();
        }
        protected override void QueueTask(Task task) // dodaje novi task u listu
        {
            return;
        }
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)  // izvrsava task
        {
            return false;
        }

        public void GenerisanjeTaska(int i, int time)
        {
            this.DodajTask(new Action(() => {
                Stopwatch timer = Stopwatch.StartNew();
                Console.WriteLine("    {0} --> HI - Thread {1}",
                    i + 1, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(time * 1000);
                Console.WriteLine(" Vrijeme izvrsavanja taska {0} : {1}", i + 1, timer.ElapsedMilliseconds / 1000);
            }));
        }
    }
}
