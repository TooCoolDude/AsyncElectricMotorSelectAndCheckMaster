using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace MotorSelectAndCheck
{
    public class ChartLoader
    {
        public Task GetTemperatureChart(List<(double,double)> values)
        {
            //HttpClient client = new HttpClient();
            //var body = new StringContent("src\\body.json");
            ////var content = new FormUrlEncodedContent(values);
            //var response = await client.PostAsync("https://yotx.ru/", body);
            //string generatedUrl = "!1/3_h/sH@xcH@0YM4X9t/2j/YP9g309Kre1vXPAudjdAvK0tyNbuBoy3s7W7sXXG29mBXOxu7Ozwdi4gF7sbO2e8nbPdjR0Eb@d0d39nn0TDbuycMh5PtxiPW5cXu/tb@1v7AA==";
            //var responseContent = await response.Content.ReadAsStringAsync();
            //string result = "https://yotx.ru/#" + generatedUrl;

            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Температура двигателя" };

            var series = new OxyPlot.Series.FunctionSeries() {
                MarkerType = MarkerType.Circle,
                Title = "Series",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };
            series.Points.AddRange(values.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            myModel.Axes.Add(new LinearAxis
            {
                Title = " t, мин",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom
            });

            myModel.Axes.Add(new LinearAxis
            {
                Title = " T, град",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
            });

            myModel.Series.Add(series);
            chartForm.plotView1.Model = myModel;
            myModel.Background = OxyColors.White;

            var pngExporter = new PngExporter { Width = 1650, Height = 1650 };
            pngExporter.ExportToFile(myModel, "src\\temperature.png");

            return Task.CompletedTask;
        }
    }
}
