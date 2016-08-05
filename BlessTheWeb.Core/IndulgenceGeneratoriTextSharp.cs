using System;
using System.Collections.Generic;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using BlessTheWeb.Core.Repository;
using System.Configuration;
using System.IO;
using System.Drawing.Imaging;
using Ghostscript.NET;

namespace BlessTheWeb.Core
{
    public class IndulgenceGeneratoriTextSharp : IIndulgenceGenerator
    {
        private readonly IFileStorage _storage;

        public IndulgenceGeneratoriTextSharp(IFileStorage storage)
        {
            _storage = storage;
        }

        public void Generate(Indulgence indulgence, string fontsDirectory, string contentDirectory, string bkFilename,
            string pdfFilename, string imageThumbnailFileName_1, string imageThumbnailFileName_2,
            string imageThumbnailFileName_3, string imageThumbnailFileName_4)
        {
            string thumb1Filename = imageThumbnailFileName_1;
            string thumb2Filename = imageThumbnailFileName_2;
            string thumb3Filename = imageThumbnailFileName_3;
            string thumb4Filename = imageThumbnailFileName_4;

            Rectangle background = PageSize.LETTER.Rotate();
            background.BackgroundColor = new BaseColor(0, 0, 0, 0);
            Document doc = new Document(PageSize.LETTER.Rotate(), 20, 20, 0, 0); // 70, 70, 130, 70

            byte[] pdfData = null;
            using (var ms = new System.IO.MemoryStream())
            {
                var pdfWriter = PdfWriter.GetInstance(doc, ms);
                pdfWriter.PageEvent = new ParchmentPageEventHelper(_storage, contentDirectory, bkFilename);

                BaseFont uechiGothicBase = BaseFont.CreateFont(System.IO.Path.Combine(fontsDirectory, "UECHIGOT.ttf"),
                    BaseFont.CP1252, BaseFont.EMBEDDED);
                BaseFont trajanProBase =
                    BaseFont.CreateFont(System.IO.Path.Combine(fontsDirectory, "TrajanPro-Regular.otf"), BaseFont.CP1252,
                        BaseFont.EMBEDDED);
                Font uechiGothic = new Font(uechiGothicBase, 150, iTextSharp.text.Font.NORMAL, new BaseColor(139, 0, 0));

                Font trajanProConfession = new Font(trajanProBase, 45, iTextSharp.text.Font.BOLDITALIC,
                    new BaseColor(139, 0, 0));

                Font trajanProBoldSmall = new Font(trajanProBase, 24, Font.BOLD, new BaseColor(139, 54, 38));
                Font trajanProAttribution = new Font(trajanProBase, 24, iTextSharp.text.Font.NORMAL,
                    new BaseColor(139, 54, 38));

                doc.Open();

                var t = new PdfPTable(1);
                t.WidthPercentage = 100;
                var c = new PdfPCell();
                c.VerticalAlignment = Element.ALIGN_MIDDLE;
                c.MinimumHeight = doc.PageSize.Height - (doc.BottomMargin + doc.TopMargin);


                // confession
                Phrase firstLetterPhrase = new Phrase(indulgence.Confession.Substring(0, 1).ToUpper(), uechiGothic);
                Phrase confessionPhrase = new Phrase(indulgence.Confession.Substring(1), trajanProConfession);
                var confessionParagraph = new Paragraph(firstLetterPhrase);
                confessionParagraph.Add(confessionPhrase);
                confessionParagraph.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                confessionParagraph.Leading = 45;
                

                // attribution
                List<Phrase> phrases = new List<Phrase>();
                phrases.Add(new Phrase("On this ", trajanProAttribution));
                phrases.Add(new Phrase(TextUtils.DayOfMonth(indulgence.DateConfessed), trajanProBoldSmall));
                phrases.Add(new Phrase(" day of ", trajanProAttribution));
                phrases.Add(new Phrase(string.Format("{0:MMMM}", indulgence.DateConfessed), trajanProBoldSmall));
                phrases.Add(new Phrase(" in the year of our Lord ", trajanProAttribution));
                phrases.Add(new Phrase(indulgence.DateConfessed.Year.ToString(), trajanProBoldSmall));
                phrases.Add(new Phrase(", ", trajanProAttribution));
                var attributionName = indulgence.Name;
                if (string.IsNullOrWhiteSpace(attributionName))
                {
                    attributionName = "An Anonymous Believer";
                }
                phrases.Add(new Phrase(attributionName, trajanProBoldSmall));
                phrases.Add(new Phrase(" selflessly gave the sum of ", trajanProAttribution));
                phrases.Add(new Phrase(string.Format("{0:c}", indulgence.AmountDonated), trajanProBoldSmall));
                phrases.Add(new Phrase(" to the deserving organisation ", trajanProAttribution));
                phrases.Add(new Phrase(indulgence.CharityName, trajanProBoldSmall));
                phrases.Add(new Phrase(" and received this plenary indulgence", trajanProAttribution));

                var attribution = new Paragraph();
                confessionParagraph.Add(Environment.NewLine);
                foreach (var phrase in phrases)
                    confessionParagraph.Add(phrase);
                attribution.Leading = 24;
                /*attribution.Insert(0, attributionName);
            attribution.Add(attributionDonation);
            attribution.Add(attributionTo);
            attribution.Add(attributionCharity);*/
                attribution.SpacingBefore = 30;
                c.AddElement(confessionParagraph);
                t.AddCell(c);
                doc.Add(t);

                doc.Close();
                pdfData = ms.ToArray();
            }

            _storage.Store(pdfFilename, pdfData, true);

            System.Drawing.Image img = null;
            using (MemoryStream pdfStream = new MemoryStream(pdfData))
            using (MemoryStream pngStream = new MemoryStream())
            {
                GhostscriptVersionInfo gvi =
                    new GhostscriptVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"gsdll32.dll"));
                using (var rasterizer = new Ghostscript.NET.Rasterizer.GhostscriptRasterizer())
                {
                    rasterizer.Open(pdfStream, gvi, true);
                    rasterizer.EPSClip = false;
                    img = rasterizer.GetPage(96, 96, 1);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    img.Save(pngStream, ImageFormat.Png);
                    _storage.Store(thumb1Filename, pngStream.ToArray(), true);
                }
            }

            float ratio = (float)img.Height/(float)img.Width;
            SaveThumbnail(img, new SizeF(800, 800*ratio), thumb2Filename);
            SaveThumbnail(img, new SizeF(300, 300*ratio), thumb3Filename);
            SaveThumbnail(img, new SizeF(150, 150*ratio), thumb4Filename);

            // TODO: generate thumbnails as byte arrays and store to IFileStorage
        }

        private void SaveThumbnail(System.Drawing.Image original, SizeF newSize, string thumbnailFilename)
        {
            using (MemoryStream imageout = new MemoryStream())
            {
                var thumb = original.GetThumbnailImage((int) newSize.Width, (int) newSize.Height, () => false,
                    IntPtr.Zero);
                thumb.Save(imageout, ImageFormat.Png);
                _storage.Store(thumbnailFilename, imageout.ToArray(), true);
            }
        }


        public class ParchmentPageEventHelper : PdfPageEventHelper
        {
            private readonly string _contentDirectory;
            iTextSharp.text.Image parchment = null;
            public ParchmentPageEventHelper(IFileStorage fileStorage, string contentDirectory, string bkFilename)
            {
                _contentDirectory = contentDirectory;
                parchment = iTextSharp.text.Image.GetInstance(fileStorage.Get(string.Format("{0}{1}.jpg",
                    ConfigurationManager.AppSettings["AssetsRelativePath"],bkFilename)));
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                //parchment.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                parchment.ScaleAbsolute(document.PageSize.Width-10, document.PageSize.Height);
                parchment.SetAbsolutePosition(0, 0);
                parchment.Alignment = Image.ALIGN_JUSTIFIED_ALL;
                
                document.Add(parchment);
                base.OnEndPage(writer, document);
            }
        }
    }
}
