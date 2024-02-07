using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid.IO;
using unvell.ReoGrid;
using System.Globalization;

namespace CurseDeliverer
{
    public static class MotorReader
    {
        public static Motor[] Read()
        {
            var path = Directory.GetCurrentDirectory() + @"\src\Motors.xlsx";
            var workbook = ReoGridControl.CreateMemoryWorkbook();
            workbook.Load(path, FileFormat.Excel2007);
            var sheet1 = workbook.Worksheets[0];

            var motorList = new List<Motor>();

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            for (int i = 3; i < 100; i++)
            {
                if (sheet1["A" + i] == null)
                {
                    break;
                }

                var motor = new Motor()
                {
                    Type = (string)sheet1["A" + i],
                    P2nom = sheet1.GetCellData<double>("B" + i),
                    n0 = sheet1.GetCellData<double>("C" + i),
                    kpd = sheet1.GetCellData<double>("D" + i),
                    cosFi = sheet1.GetCellData<double>("E" + i),
                    mP = sheet1.GetCellData<double>("F" + i),
                    mM = sheet1.GetCellData<double>("G" + i),
                    mK = sheet1.GetCellData<double>("H" + i),
                    sN = sheet1.GetCellData<double>("I" + i),
                    sK = sheet1.GetCellData<double>("J" + i),
                    ki = sheet1.GetCellData<double>("K" + i),
                    J = sheet1.GetCellData<double>("L" + i),
                    m = sheet1.GetCellData<double>("M" + i)
                };
                motorList.Add(motor);
            }

            return motorList.ToArray();
        }
    }
}
