using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BlessTheWeb.Core;
using btwc=BlessTheWeb.Core;

namespace IndulgenceGenerator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var ig = new btwc.IndulgenceGeneratoriTextSharp();

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
                    Path.Combine(Environment.CurrentDirectory, "output\\indulgence.pdf"), Path.Combine(Environment.CurrentDirectory, "output\\indulgence.png"), "charity name",
                    Path.Combine(Environment.CurrentDirectory, "fonts"), Path.Combine(Environment.CurrentDirectory, "content"));
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
