using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GhostscriptSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace BlessTheWeb.Core
{
    public class IndulgenceGeneratoriTextSharp : IIndulgenceGenerator
    {
        public void Generate(Indulgence indulgence, string pdfOutputPath, string imageOutputPath, string charityName, string fontsDirectory, string contentDirectory)
        {
            //string pdfFilename = Path.Combine(pdfOutputPath, string.Format("{0}.pdf", id.Replace('/','\\')));
            //string imageFilename = Path.Combine(pdfOutputPath, string.Format("{0}.png", id.Replace('/', '\\')));
            string pdfFilename = pdfOutputPath;
            string imageFilename = imageOutputPath;
            string thumb1Filename = imageOutputPath.Replace(".png", "_100.png");
            string thumb2Filename = imageOutputPath.Replace(".png", "_50.png");
            string thumb3Filename = imageOutputPath.Replace(".png", "_25.png");
            string thumb4Filename = imageOutputPath.Replace(".png", "_10.png");

            if (!Directory.Exists(Path.GetDirectoryName(pdfFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(pdfFilename));
            if (File.Exists(pdfFilename)) File.Delete(pdfFilename);
            if (File.Exists(imageFilename)) File.Delete(imageFilename);

            Rectangle background = PageSize.A4.Rotate();
            background.BackgroundColor = new BaseColor(0,0,0,0);
            Document doc = new Document(PageSize.A4.Rotate(), 70, 70, 130, 70);
            
            var pdfWriter = PdfWriter.GetInstance(doc, new FileStream(pdfFilename, FileMode.Create));
            pdfWriter.PageEvent = new ParchmentPageEventHelper(contentDirectory);

            BaseFont uechiGothicBase = BaseFont.CreateFont(Path.Combine(fontsDirectory, "UECHIGOT.ttf"), BaseFont.CP1252, BaseFont.EMBEDDED);
            BaseFont trajanProBase = BaseFont.CreateFont(Path.Combine(fontsDirectory, "TrajanPro-Regular.otf"), BaseFont.CP1252, BaseFont.EMBEDDED);
            Font uechiGothic = new Font(uechiGothicBase, 150, iTextSharp.text.Font.NORMAL, new BaseColor(139,0,0));
            
            Font trajanProConfession = new Font(trajanProBase, 30, iTextSharp.text.Font.BOLDITALIC, new BaseColor(139, 0, 0));

            Font trajanProBoldSmall = new Font(trajanProBase, 24, Font.BOLD, new BaseColor(139, 54, 38));
            Font trajanProAttribution = new Font(trajanProBase, 24, iTextSharp.text.Font.NORMAL, new BaseColor(139, 54, 38));

            doc.Open();

            // confession
            Phrase firstLetterPhrase = new Phrase(indulgence.Confession.Substring(0, 1).ToUpper(), uechiGothic);
            Phrase confessionPhrase = new Phrase(indulgence.Confession.Substring(1), trajanProConfession);
            var confessionParagraph = new Paragraph(firstLetterPhrase);
            confessionParagraph.Add(confessionPhrase);
            confessionParagraph.Alignment = iTextSharp.text.Image.ALIGN_TOP;
            confessionParagraph.Leading = 45;
            doc.Add(confessionParagraph);

            // attribution
            List<Phrase> phrases = new List<Phrase>();
            phrases.Add(new Phrase("On this ", trajanProAttribution));
            phrases.Add(new Phrase(DayOfMonth(indulgence.DateConfessed), trajanProBoldSmall));
            phrases.Add(new Phrase(" day of ", trajanProAttribution));
            phrases.Add(new Phrase(string.Format("{0:MMMM}", indulgence.DateConfessed), trajanProBoldSmall));
            phrases.Add(new Phrase(" in the year of our Lord ", trajanProAttribution));
            phrases.Add(new Phrase(indulgence.DateConfessed.Year.ToString(), trajanProBoldSmall));
            phrases.Add(new Phrase(", ", trajanProAttribution));
            phrases.Add(new Phrase(indulgence.Name, trajanProBoldSmall));
            phrases.Add(new Phrase(" selflessly gave the sum of ", trajanProAttribution));
            phrases.Add(new Phrase(string.Format("{0:c}", indulgence.AmountDonated), trajanProBoldSmall));
            phrases.Add(new Phrase(" to the deserving organisation ", trajanProAttribution));
            phrases.Add(new Phrase(indulgence.CharityName, trajanProBoldSmall));
            phrases.Add(new Phrase(" and received this pleniary indulgence", trajanProAttribution));

            var attribution = new Paragraph();
            foreach (var phrase in phrases)
                attribution.Add(phrase);
            attribution.Leading = 24;
            /*attribution.Insert(0, attributionName);
            attribution.Add(attributionDonation);
            attribution.Add(attributionTo);
            attribution.Add(attributionCharity);*/
            attribution.SpacingBefore = 30;
            doc.Add(attribution);

            doc.Close();

            GhostscriptWrapper.GeneratePageThumb(pdfFilename, thumb3Filename, 1, 25, 25);
            GhostscriptWrapper.GeneratePageThumb(pdfFilename, thumb1Filename, 1, 80, 80);
            GhostscriptWrapper.GeneratePageThumb(pdfFilename, thumb4Filename, 1, 10, 10);
        }

        private static string DayOfMonth(DateTime dateConfessed)
        {
            if (dateConfessed.Day > 10 && dateConfessed.Day < 14)
                return (dateConfessed.Day + "th");
            if (dateConfessed.Day.ToString().EndsWith("1"))
                return (dateConfessed.Day + "st");
            if (dateConfessed.Day.ToString().EndsWith("2"))
                return dateConfessed.Day + "nd";
            if (dateConfessed.Day.ToString().EndsWith("3"))
                return dateConfessed.Day + "rd";
            return dateConfessed.Day + "th";
        }

        public class ParchmentPageEventHelper : PdfPageEventHelper
        {
            private readonly string _contentDirectory;
            iTextSharp.text.Image parchment = null;
            public ParchmentPageEventHelper(string contentDirectory)
            {
                _contentDirectory = contentDirectory;
                parchment = iTextSharp.text.Image.GetInstance(Path.Combine(_contentDirectory, "YeOldeParchment4.png"));
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                parchment.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                parchment.SetAbsolutePosition(0, 0);
                parchment.Alignment = Image.UNDERLYING;
                
                document.Add(parchment);
                base.OnEndPage(writer, document);
            }
        }
    }
}
