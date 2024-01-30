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
        
        public Task GetCharacteristicsChart(List<(double, double)> valuesMst, List<(double, double)> valuesMw, List<(double, double)> valuesIw)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Нагрузочные диаграммы" };



            var seriesMst = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "Mstatic",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };
            seriesMst.Points.AddRange(valuesMst.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesMw = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "M(w)",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };
            seriesMw.Points.AddRange(valuesMw.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesIw = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "I(w)",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };
            seriesIw.Points.AddRange(valuesIw.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());



            myModel.Axes.Add(new LinearAxis
            {
                Title = " M, Нм ; I, А",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom
            });

            myModel.Axes.Add(new LinearAxis
            {
                Title = " w, рад/с",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
            });

            myModel.Series.Add(seriesMst);
            myModel.Series.Add(seriesMw);
            myModel.Series.Add(seriesIw);

            chartForm.plotView1.Model = myModel;
            myModel.Background = OxyColors.White;

            var pngExporter = new PngExporter { Width = 1650, Height = 1650 };
            pngExporter.ExportToFile(myModel, "src\\characteristics.png");

            return Task.CompletedTask;
        }
    }
}
