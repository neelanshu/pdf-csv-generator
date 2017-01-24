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
            //new ClassLibrary1.Class1().CreatePdf();
            new ClassLibrary1.Class1().CreatePdfNewNew();
            //new DataSource().GetFormDetails();
            //var ret = new GenericLPRObjService().GenerateReportObject();
            //var stringJson = JsonConvert.SerializeObject(ret); 

            //File.WriteAllText(@"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\AO.LPR.Reports\full_json_new.json", stringJson);

            //Console.ReadKey();
        }
    }
}
