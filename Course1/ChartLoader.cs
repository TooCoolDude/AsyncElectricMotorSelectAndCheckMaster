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
