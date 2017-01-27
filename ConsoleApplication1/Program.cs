using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AO.LPR.Reports;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            new ClassLibrary1.Class1().CreatePdfNewNew();
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;

            //Console.WriteLine(elapsedMs.ToString());
            //Console.ReadKey();
        }
    }
}
