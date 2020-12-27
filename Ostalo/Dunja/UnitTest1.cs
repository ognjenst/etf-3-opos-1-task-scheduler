using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Zadatak1.Tests.XUnit
{
    public class UnitTest1
    {
        [Fact]
        public void DodatTask()
        {
            Random rnd = new Random();
            Rasporedjivac Rasporedjivac = new Rasporedjivac(4, 20, 0);
            Rasporedjivac.GenerisanjeTaska(1, rnd.Next(1, 5));
            Thread.Sleep(5000);

            Assert.Equal(1,Rasporedjivac.TaskInfo());
        }
        [Fact]
        public void BrojAktivnihNiti()
        {
            Rasporedjivac Rasporedjivac = new Rasporedjivac(4, 10, 0);
            Rasporedjivac.GenerisanjeTaska(1, 2);
            Rasporedjivac.GenerisanjeTaska(2, 3);
            Rasporedjivac.Start();
            Thread.Sleep(2000);

            Assert.Equal(2, Rasporedjivac.getBrojAktivnihNiti());
        }
        [Fact]
        public void BrojNiti()
        {
            Rasporedjivac Rasporedjivac = new Rasporedjivac(4, 20, 0);

            Assert.Equal(4, Rasporedjivac.getMaxBrojNiti());
        }
        [Fact]
        public void IstekloVrijeme()
        {
            Rasporedjivac Rasporedjivac = new Rasporedjivac(4, 5, 0);
            Rasporedjivac.GenerisanjeTaska(1, 2);
            Rasporedjivac.GenerisanjeTaska(2, 3);
            Rasporedjivac.Start();

            Assert.Equal(5.0, Rasporedjivac.getUkupnoVrijeme()); ;
        }
    }
}
