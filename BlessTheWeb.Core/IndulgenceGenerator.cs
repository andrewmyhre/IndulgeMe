using System;
using System.Drawing;
using System.IO;
using WebSupergoo.ABCpdf7;

namespace BlessTheWeb.Core
{
    public class IndulgenceGenerator : IIndulgenceGenerator
    {
        public void Generate(Indulgence indulgence, string pdfOutputPath, string imageOutputPath, string charityName, string fontsDirectory, string contentDirectory)
        {
            string pdfFilename = Path.Combine(pdfOutputPath, string.Format("{0}.pdf", indulgence.Id));
            string imageFilename = Path.Combine(pdfOutputPath, string.Format("{0}.png", indulgence.Id));

            if (!Directory.Exists(Path.GetDirectoryName(pdfFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(pdfFilename));
            if (File.Exists(pdfFilename)) File.Delete(pdfFilename);
            if(File.Exists(imageFilename)) File.Delete(imageFilename);

            Doc theDoc = new Doc();

            double w = theDoc.MediaBox.Width;
            double h = theDoc.MediaBox.Height;
            double l = theDoc.MediaBox.Left;
            double b = theDoc.MediaBox.Bottom;
            theDoc.Transform.Rotate(90, l, b);
            theDoc.Transform.Translate(w, 0);

            // rotate our rectangle
            theDoc.Rect.Width = h;
            theDoc.Rect.Height = w;
            theDoc.Rect.Inset(50, 50);

            theDoc.FontSize = 32;
            theDoc.TextStyle.Justification = 1;
            double embellishmentWidth = 100, embellishmentHeight=100;
            XRect rectangle = theDoc.Rect;
            double top = rectangle.Top;
            double height = rectangle.Height;
            double left = rectangle.Left;
            double width = rectangle.Width;
            double bottom = rectangle.Bottom;
            theDoc.Rect.Inset(16,16);
            theDoc.Rect.SetRect(left + embellishmentWidth, top-embellishmentHeight, width - embellishmentWidth, embellishmentHeight);
            var theID = theDoc.AddHtml(indulgence.Confession);

            if (theDoc.Chainable(theID))
            {
                theDoc.Rect.SetRect(left, bottom, width, height-embellishmentHeight);
                theID = theDoc.AddHtml("", theID);
            }

            theDoc.Rect.Left = bottom;

            theDoc.VPos = 1;
            theDoc.Rect.SetRect(left, bottom, width, height);
            theDoc.Color.String = "128 128 128";
            theDoc.AddHtml(string.Format("This indulgence was granted to {0} through the pious act of donating {1:c} to {2}",
                indulgence.Name, indulgence.AmountDonated, charityName));

            theID = theDoc.GetInfoInt(theDoc.Root, "Pages");
            theDoc.SetInfo(theID, "/Rotate", "90");

            theDoc.Save(pdfFilename);

            theDoc.Rendering.DotsPerInch = 96;
            theDoc.Rect.String = theDoc.CropBox.String;
            theDoc.Rendering.Save(imageFilename);

            theDoc.Clear();
        }
    }
}
