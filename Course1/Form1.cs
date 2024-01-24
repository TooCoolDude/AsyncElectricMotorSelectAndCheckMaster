using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using unvell.ReoGrid;
using unvell.ReoGrid.IO;

namespace CurseDeliverer
{
    public partial class Form1 : Form
    {
        VariantValues[] variants = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            variants = VariantReader.GetVariants();

            var variant = variants.Where(x => x.Variant == textBox1.Text);
            if (variant.Count() < 1)
            {
                textBox1.Text = "wrong";
                return;
            }

            File.Copy("E:\\Curse1\\auto\\Sample.docx", "E:\\Curse1\\auto\\Result.docx", true);

            var replacements = await Calculator.GetVariablesAndValues(variant.First());
            DocumentInteractor.WriteChanges("E:\\Curse1\\auto\\Result.docx", replacements);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
