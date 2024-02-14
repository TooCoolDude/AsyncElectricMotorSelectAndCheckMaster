using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using unvell.ReoGrid.IO;
using Word = Microsoft.Office.Interop.Word;

namespace CurseDeliverer
{
    public static class DocumentInteractor
    {
        public static void ReplaceFormulas(string document, Dictionary<string, string> replacements)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(document, true))
            {
                string? docText = null;


                if (wordDoc.MainDocumentPart is null)
                {
                    throw new ArgumentNullException("MainDocumentPart and/or Body is null.");
                }

                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (var r in replacements)
                {
                    Regex regexText = new Regex(r.Key);
                    docText = regexText.Replace(docText, r.Value);
                }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }

        private static void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;

            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }

        public static void WriteChanges(string document, Dictionary<string, string> replacements)
        {
            ReplaceFormulas(document, replacements);

            object fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, document);
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = true };
            Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(fileName, ReadOnly: false, Visible: true);
            aDoc.Activate();

            foreach (var r in replacements)
            {
                FindAndReplace(wordApp, r.Key, r.Value);
            }

            ReplaceImage(wordApp, aDoc, "{image1}", "\\src\\diagramPower.png");
            ReplaceImage(wordApp, aDoc, "{image2}", "\\src\\temperature.png");
            ReplaceImage(wordApp, aDoc, "{image3}", "\\src\\characteristics.png");
            ReplaceImage(wordApp, aDoc, "{image4}", "\\src\\characteristics2.png");
            ReplaceImage(wordApp, aDoc, "{image5}", "\\src\\diagramCurrent.png");
        }

        private static void ReplaceImage(Word.Application word, Word.Document doc, string pattern, string imagePath)
        {
            object missing = Type.Missing;

            Word.Range range = word.ActiveDocument.Content;
            Word.Find find = range.Find;

            find.Text = pattern;
            find.ClearFormatting();

            if (find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
              ref missing, ref missing, ref missing, ref missing, ref missing,
               ref missing, ref missing, ref missing, ref missing, ref missing))
            {

                var shape = range.InlineShapes.AddPicture(Directory.GetCurrentDirectory() + imagePath, ref missing, ref missing, ref missing);
                shape.Width = 425;
                shape.Height = 425;

                find.Replacement.ClearFormatting();
                find.Replacement.Text = "";
                object replaceOne = Microsoft.Office.Interop.Word.WdReplace.wdReplaceOne;
                find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                   ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref replaceOne, ref missing, ref missing, ref missing, ref missing);


                doc.Save();
            }

            else
            {
                MessageBox.Show("The text could not be located.");
            }
        }
    }
}
