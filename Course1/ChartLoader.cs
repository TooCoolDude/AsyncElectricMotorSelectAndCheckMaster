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
using CurseDeliverer;

namespace MotorSelectAndCheck
{
    public class ChartLoader
    {
        public Task GetCurrentDiagram(List<(double, double)> values)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Нагрузочная диаграмма электродвигателя I(t)" };

            var series = new OxyPlot.Series.AreaSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "I(t)",
                Color = OxyColors.Black,
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
                Title = " I, А",
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
            pngExporter.ExportToFile(myModel, Directory.GetCurrentDirectory() + "\\src\\diagramCurrent.png");

            return Task.CompletedTask;
        }

        public Task GetPowerDiagram(VariantValues v)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Нагрузочная диаграмма рабочей машины P(t)" };

            var series = new OxyPlot.Series.AreaSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "P(t)",
                Color = OxyColors.Black,
            };
            var t2 = v.t2 + v.t1;
            var t3 = v.t3 + t2;
            var t4 = v.t4 + t3;
            series.Points.AddRange(new[]
            {
                new DataPoint(0, 0),
                new DataPoint(0.0, v.P1), new DataPoint(v.t1, v.P1),
                new DataPoint(v.t1, v.P2), new DataPoint(t2, v.P2),
                new DataPoint(t2, v.P3), new DataPoint(t3, v.P3),
                new DataPoint(t3, v.P4), new DataPoint(t4, v.P4),
                new DataPoint(t4, 0.0)
            });

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
                Title = " P, кВт",
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
            pngExporter.ExportToFile(myModel, Directory.GetCurrentDirectory() + "\\src\\diagramPower.png");

            return Task.CompletedTask;
        }

        public Task GetTemperatureChart(List<(double,double)> values)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Температура двигателя" };

            var series = new OxyPlot.Series.FunctionSeries() {
                MarkerType = MarkerType.Circle,
                Title = "T(t)",
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
            pngExporter.ExportToFile(myModel, Directory.GetCurrentDirectory() + "\\src\\temperature.png");

            return Task.CompletedTask;
        }
        
        public Task GetCharacteristicsChart(List<(double, double)> valuesMst, List<(double, double)> valuesMw, List<(double, double)> valuesIw, List<(double, double)[]> wList, List<(double, double)> valuesMdin)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "Графоаналитический метод построения нагрузочных диаграмм" };

            var seriesW1 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w1",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW1.Points.AddRange(wList[0].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesW2 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w2",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW2.Points.AddRange(wList[1].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesW3 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w3",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW3.Points.AddRange(wList[2].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesW4 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w4",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW4.Points.AddRange(wList[3].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesW5 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w5",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW5.Points.AddRange(wList[4].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesW6 = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w уст",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Black,
            };
            seriesW6.Points.AddRange(wList[5].Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesMst = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "Mstatic",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Blue,
            };
            seriesMst.Points.AddRange(valuesMst.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesMw = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "M(w)",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color= OxyColors.Orange,
            };
            seriesMw.Points.AddRange(valuesMw.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesMdin = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "Mдин",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Blue,
            };
            seriesMdin.Points.AddRange(valuesMdin.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());

            var seriesIw = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "I(w)",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Red,
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
            myModel.Series.Add(seriesW1);
            myModel.Series.Add(seriesW2);
            myModel.Series.Add(seriesW3);
            myModel.Series.Add(seriesW4);
            myModel.Series.Add(seriesW5);
            myModel.Series.Add(seriesW6);
            myModel.Series.Add(seriesMdin);

            chartForm.plotView1.Model = myModel;
            myModel.Background = OxyColors.White;

            var pngExporter = new PngExporter { Width = 1650, Height = 1650 };
            pngExporter.ExportToFile(myModel, Directory.GetCurrentDirectory() + "\\src\\characteristics.png");

            return Task.CompletedTask;
        }

        public Task GetCharacteristicsChart2(List<(double, double)> valuesIt, List<(double, double)> valuesWt)
        {
            var chartForm = new ChartForm();
            chartForm.Show();

            var myModel = new PlotModel { Title = "I(t), w(t)" };

            var seriesIt = new OxyPlot.Series.FunctionSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "I(t)",
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
                Color = OxyColors.Red,
            };
            seriesIt.Points.AddRange(valuesIt.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());
            var seriesItLine = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "I(t)",
                Color = OxyColors.Red,
            };
            var lastIt = valuesIt.Last();
            seriesItLine.Points.AddRange(new[] 
            { 
                new DataPoint(lastIt.Item1, lastIt.Item2), new DataPoint(lastIt.Item1 + lastIt.Item1, lastIt.Item2)
            });

            var seriesWt = new OxyPlot.Series.FunctionSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w(t)",
                Color = OxyColors.Green,
            };
            seriesWt.Points.AddRange(valuesWt.Select(v => new DataPoint(v.Item1, v.Item2)).ToArray());
            var seriesWtLine = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                Title = "w(t)",
                Color = OxyColors.Green,
            };
            var lastWt = valuesWt.Last();
            seriesWtLine.Points.AddRange(new[]
            {
                new DataPoint(lastWt.Item1, lastWt.Item2), new DataPoint(lastWt.Item1 + lastWt.Item1, lastWt.Item2)
            });

            myModel.Axes.Add(new LinearAxis
            {
                Title = " t, сек",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom
            });

            myModel.Axes.Add(new LinearAxis
            {
                Title = "I, А ; w, рад/с",
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
            });

            myModel.Series.Add(seriesIt);
            myModel.Series.Add(seriesItLine);
            myModel.Series.Add(seriesWt);
            myModel.Series.Add(seriesWtLine);
            chartForm.plotView1.Model = myModel;
            myModel.Background = OxyColors.White;

            var pngExporter = new PngExporter { Width = 1650, Height = 1650 };
            pngExporter.ExportToFile(myModel, Directory.GetCurrentDirectory() + "\\src\\characteristics2.png");

            return Task.CompletedTask;
        }
    }
}
