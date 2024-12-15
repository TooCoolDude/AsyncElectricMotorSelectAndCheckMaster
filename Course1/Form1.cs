using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using MotorSelectAndCheck;
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

            var variant = variants.Where(x => x.Variant1 == textBox1.Text && x.Variant2 == textBox2.Text);
            if (variant.Count() < 1)
            {
                textBox1.Text = "wrong";
                return;
            }

            //test
            //var points = new List<(int, int)>();
            //points.AddRange(new[] { (0, 0), (5, 5), (10, 5), (15, 0) });
            //var chartLoader = new ChartLoader();
            //await chartLoader.GetTemperatureChart(points);

            File.Copy("src\\Template.docx", Directory.GetCurrentDirectory() + "\\Result.docx", true);

            var replacements = Calculator.GetVariablesAndValues(variant.First());
            DocumentInteractor.WriteChanges(Directory.GetCurrentDirectory() + "\\Result.docx", await replacements);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
