using CurseDeliverer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid.IO;
using unvell.ReoGrid;

namespace MotorSelectAndCheck
{
    public static class AEswitchReader
    {
        public static AEswitchValues[] GetValues()
        {
            var path = Directory.GetCurrentDirectory() + @"\src\AEswitches.xlsx";
            var workbook = ReoGridControl.CreateMemoryWorkbook();
            workbook.Load(path, FileFormat.Excel2007);
            var sheet1 = workbook.Worksheets[0];

            var values = new List<AEswitchValues>();

            for (int i = 2; i < 100; i+=2)
            {
                if (sheet1.GetCellData<string>("A" + i) is not { } cell1Data)
                    break;
                var value = new AEswitchValues
                    (
                        Type: cell1Data,
                        CurrentValues: sheet1.GetCellData<string>("A" + (i+1))
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(double.Parse)
                        .ToArray()
                    );

                values.Add(value);
            }

            return values.ToArray();
        }
    }
}
