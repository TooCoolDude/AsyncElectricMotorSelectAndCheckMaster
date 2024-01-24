using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseDeliverer
{
    public static class Calculator
    {
        public static async Task<Dictionary<string, string>> GetVariablesAndValues(VariantValues v)
        {
            var d = new Dictionary<string, string>();
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            //var c = CultureInfo.CurrentCulture;
            
            //Входные данные
            d["{variant1}"] = v.Variant[0].ToString();
            d["{variant2}"] = v.Variant[1].ToString();
            d["{P1}"] = v.P1.ToString();
            d["{P2}"] = v.P2.ToString();
            d["{P3}"] = v.P3.ToString();
            d["{P4}"] = v.P4.ToString();
            d["{t1}"] = v.t1.ToString();
            d["{t2}"] = v.t2.ToString();
            d["{t3}"] = v.t3.ToString();
            d["{t4}"] = v.t4.ToString();

            //Предварительный выбор электродвигателя
            var P1sqr = v.P1 * v.P1 * v.t1;
            d["{P1sqrXt}"] = P1sqr.ToString();
            var P2sqr = v.P2 * v.P2 * v.t2;
            d["{P2sqrXt}"] = P2sqr.ToString();
            var P3sqr = v.P3 * v.P3 * v.t3;
            d["{P3sqrXt}"] = P3sqr.ToString();
            var P4sqr = v.P4 * v.P4 * v.t4;
            d["{P4sqrXt}"] = P4sqr.ToString();
            var tsum = v.t1 + v.t2 + v.t3 + v.t4;
            d["{tsum}"] = tsum.ToString();
            var Pekv = Math.Sqrt((P1sqr + P2sqr + P3sqr + P4sqr) / tsum);
            d["{Pekv}"] = Pekv.ToString();
            var Km = Math.Sqrt(((0.6 + 1) / (1 - Math.Pow(2.7, -tsum / 20)) - 0.6));
            d["{Km}"] = Km.ToString();
            var Pras = Pekv / Km;
            d["{Pras}"] = Pras.ToString();

            //Выбор электродвигателя
            var form = new MotorSelectAndCheck.MotorCalculationForm();
            form.labelPras.Text = Pras.ToString();

            //исправить это всё
            var motors = MotorReader.Read();
            foreach ( var m in motors.Where(x => x.P2nom > Pras))
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                //Таблица хараактеристик электродвигателя
                d["{P2nom}"] = m.P2nom.ToString();
                d["{P2nomK}"] = (m.P2nom * 1000).ToString();
                d["{n0}"] = m.n0.ToString();
                d["{kpd}"] = m.kpd.ToString();
                d["{cosFi}"] = m.cosFi.ToString();
                d["{mP}"] = m.mP.ToString();
                d["{mM}"] = m.mM.ToString();
                d["{mK}"] = m.mK.ToString();
                d["{sNp}"] = m.sN.ToString();
                d["{sN}"] = (m.sN / 100).ToString();
                d["{sKp}"] = m.sK.ToString();
                d["{sK}"] = (m.sK / 100).ToString();
                d["{ki}"] = m.ki.ToString();
                d["{J}"] = m.J.ToString();
                d["{m}"] = m.m.ToString();

                //Проверка на перегрузочную способность
                var Mnom = m.P2nom * 1000 / (157 * (1 - (m.sN / 100)));
                d["{Mnom}"] = Mnom.ToString();
                var Mk = Mnom * m.mK;
                d["{Mk}"] = Mk.ToString();
                var Pmax = new[] { v.P1, v.P2, v.P3, v.P4 }.Max() * 1000;
                d["{Pmax}"] = Pmax.ToString();
                d["{PmaxK}"] = (Pmax / 1000).ToString();
                var Msmax = Pmax / (157 * (1 - (m.sK / 100)));
                d["{Msmax}"] = Msmax.ToString();
                var Mk09 = Mk * Math.Pow(1 - 0.1, 2);
                d["{Mk09}"] = Mk09.ToString();
                
                if (Mk09 < Msmax)
                    continue;
                if (m.P2nom < 23)
                    continue;

                //Проверка на нагрев
                //



                break;
            }


            return d;
        }
    }
}
