using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AO.LPR.Reports;
using ClassLibrary1;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //new ClassLibrary1.PDfBase().CreatePdfNewNew();
            new PDfBase().GenerateSearchCsvReport(); 
        }
    }
}
