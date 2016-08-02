using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Repository;
using btwc=BlessTheWeb.Core;

namespace IndulgenceGenerator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var ig = new btwc.IndulgenceGeneratoriTextSharp(new FileSystemStorage());

            try
            {
                var indulgence = new Indulgence()
                                     {
                                         Id = "indulgences/1",
                                         Confession = Text,
                                         Name = "Andrew",
                                         AmountDonated = 200,
                                         CharityName = "test charity",
                                         DateConfessed = DateTime.Now
                                     };

                ig.Generate(indulgence, 
                    Path.Combine(Environment.CurrentDirectory, "fonts"), 
                    Path.Combine(Environment.CurrentDirectory, "content"),
                    "parchment3",
                    "1.pdf","1_1.png","1_2.png","1_3.png","1_4.png");
            } catch( Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            sw.Stop();
            Console.WriteLine(string.Format("Took {0}", sw.Elapsed));
        }

        static string Text
        {
            get { return "the quick brown fox jumped over the lazy dog"; }
        }
    }
}
