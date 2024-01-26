using DocumentFormat.OpenXml.Drawing.Diagrams;
using MotorSelectAndCheck;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurseDeliverer
{
    public static class Calculator
    {
        public static async Task<Dictionary<string, string>> GetVariablesAndValues(VariantValues v)
        {
            var d = new Dictionary<string, string>();
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            
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

            //возможно перенести
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

                var kpd0 = m.kpd / 100;
                d["{kpd0}"] = kpd0.ToString();


                //Проверка на нагрев методом средних потерь
                var dPnom = m.P2nom * (1 - kpd0) / kpd0;
                d["{dPnom}"] = dPnom.ToString();
                var match = Regex.Match(m.Type, @"4(AM|АМ)(\d+)[A-ZА-Я]").Groups[2];
                var engineHeight = double.Parse(match.Value);
                var Tdop = engineHeight < 150 ? 80 : 100;
                d["{Tdop}"] = Tdop.ToString();
                var TN = (6 * m.m * Tdop * kpd0) / (m.P2nom * 1000 * (1 - kpd0));
                d["{TN}"] = TN.ToString();
                var KT = 1 / (1 - Math.Pow(2.7, -tsum / TN));
                d["{KT}"] = KT.ToString();

                var x1 = v.P1 / m.P2nom;
                d["{x1}"] = x1.ToString();
                var x2 = v.P2 / m.P2nom;
                d["{x2}"] = x2.ToString();
                var x3 = v.P3 / m.P2nom;
                d["{x3}"] = x3.ToString();
                var x4 = v.P4 / m.P2nom;
                d["{x4}"] = x4.ToString();
                var kpd1 = 1 / (1 + Math.Pow((1 / (kpd0)) - 1, ((0.6 / x1) + x1) / (0.6 + 1)));
                d["{kpd1}"] = kpd1.ToString();
                var kpd2 = 1 / (1 + Math.Pow((1 / (kpd0)) - 1, ((0.6 / x2) + x2) / (0.6 + 1)));
                d["{kpd2}"] = (kpd2.ToString());
                var kpd3 = 1 / (1 + Math.Pow((1 / (kpd0)) - 1, ((0.6 / x3) + x3) / (0.6 + 1)));
                d["{kpd3}"] = kpd3.ToString();
                var kpd4 = 1 / (1 + Math.Pow((1 / (kpd0)) - 1, ((0.6 / x4) + x4) / (0.6 + 1)));
                d["{kpd4}"] = kpd4.ToString();
                var dP1 = v.P1 * (1 - kpd1) / kpd1;
                d["{dP1}"] = dP1.ToString();
                var dP2 = v.P2 * (1 - kpd2) / kpd2;
                d["{dP2}"] = dP2.ToString();
                var dP3 = v.P3 * (1 - kpd3) / kpd3;
                d["{dP3}"] = dP3.ToString();
                var dP4 = v.P4 * (1 - kpd4) / kpd4;
                d["{dP4}"] = dP4.ToString();

                var dPsred = (dP1 * v.t1 + dP2 * v.t2 + dP3 * v.t3 + dP4 * v.t4) / tsum;
                d["{dPsred}"] = dPsred.ToString();
                
                if (dPnom < dPsred / KT)
                    continue;


                //Проверка на нагрев методом расчета температуры
                var dPnomK = dPnom * 1000;
                d["{dPnomK}"] = dPnomK.ToString();
                var Ateplo = dPnomK / Tdop;
                d["{Ateplo}"] = Ateplo.ToString();

                var dP1K = dP1 * 1000;
                d["{dP1K}"] = dP1K.ToString();
                var dP2K = dP2 * 1000;
                d["{dP2K}"] = dP2K.ToString();
                var dP3K = dP3 * 1000;
                d["{dP3K}"] = dP3K.ToString();
                var dP4K = dP4 * 1000;
                d["{dP4K}"] = dP4K.ToString();

                var Tust1 = dP1K / Ateplo;
                d["{Tust1}"] = Tust1.ToString();
                var Tust2 = dP2K / Ateplo;
                d["{Tust2}"] = Tust2.ToString();
                var Tust3 = dP3K / Ateplo;
                d["{Tust3}"] = Tust3.ToString();
                var Tust4 = dP4K / Ateplo;
                d["{Tust4}"] = Tust4.ToString();

                var T1sr = Tust1 * (1 - Math.Pow(2.7, -v.t1 / (2 * TN)));
                d["{T1sr}"] = T1sr.ToString();
                var T1kon = Tust1 * (1 - Math.Pow(2.7, -v.t1 / TN));
                d["{T1kon}"] = T1kon.ToString();

                var T2sr = (Tust2 * (1 - Math.Pow(2.7, -v.t2 / (2 * TN)))) + (T1kon * Math.Pow(2.7, -v.t2 / (2 * TN)));
                d["{T2sr}"] = T2sr.ToString();
                var T2kon = (Tust2 * (1 - Math.Pow(2.7, -v.t2 / TN))) + (T1kon * Math.Pow(2.7, -v.t2 / TN));
                d["{T2kon}"] = T2kon.ToString();

                var T3sr = (Tust3 * (1 - Math.Pow(2.7, -v.t3 / (2 * TN)))) + (T2kon * Math.Pow(2.7, -v.t3 / (2 * TN)));
                d["{T3sr}"] = T3sr.ToString();
                var T3kon = (Tust3 * (1 - Math.Pow(2.7, -v.t3 / TN))) + (T2kon * Math.Pow(2.7, -v.t3 / TN));
                d["{T3kon}"] = T3kon.ToString();

                var T4sr = (Tust4 * (1 - Math.Pow(2.7, -v.t4 / (2 * TN)))) + (T3kon * Math.Pow(2.7, -v.t4 / (2 * TN)));
                d["{T4sr}"] = T4sr.ToString();
                var T4kon = (Tust4 * (1 - Math.Pow(2.7, -v.t4 / TN))) + (T3kon * Math.Pow(2.7, -v.t4 / TN));
                d["{T4kon}"] = T4kon.ToString();

                var t1kon = v.t1;
                d["{t1kon}"] = t1kon.ToString();
                var t1sr = t1kon / 2;
                d["{t1sr}"] = t1sr.ToString();
                var t2kon = v.t2 + t1kon;
                d["{t2kon}"] = t2kon.ToString();
                var t2sr = (t2kon + t1kon) / 2;
                d["{t2sr}"] = t2sr.ToString();
                var t3kon = v.t3 + t2kon;
                d["{t3kon}"] = t3kon.ToString();
                var t3sr = (t2kon + t3kon) / 2;
                d["{t3sr}"] = t3sr.ToString();
                var t4kon = t3kon + v.t4;
                d["{t4kon}"] = t4kon.ToString();
                var t4sr = (t4kon + t3kon) / 2;
                d["{t4sr}"] = t4sr.ToString();

                var T0 = TN * 2;
                d["{T0}"] = T0.ToString();
                var T01 = T0 * 1;
                d["{T01}"] = T01.ToString();
                var T02 = T0 * 2;
                d["{T02}"] = T02.ToString();
                var T03 = T0 * 3;
                d["{T03}"] = T03.ToString();
                var T04 = T0 * 4;
                d["{T04}"] = T04.ToString();
                var T05 = T0 * 5;
                d["{T05}"] = T05.ToString();
                
                var Tohl1 = T4kon * Math.Pow(2.7, -T01 / T0);
                d["{T1ohl}"] = Tohl1.ToString();
                var Tohl2 = T4kon * Math.Pow(2.7, -T02 / T0);
                d["{T2ohl}"] = Tohl2.ToString();
                var Tohl3 = T4kon * Math.Pow(2.7, -T03 / T0);
                d["{T3ohl}"] = Tohl3.ToString();
                var Tohl4 = T4kon * Math.Pow(2.7, -T04 / T0);
                d["{T4ohl}"] = Tohl4.ToString();
                var Tohl5 = T4kon * Math.Pow(2.7, -T05 / T0);
                d["{T5ohl}"] = Tohl5.ToString();

                d["{aa1}"] = Tohl1.ToString();
                d["{aa2}"] = Tohl2.ToString();
                d["{aa3}"] = Tohl3.ToString();
                d["{aa4}"] = Tohl4.ToString();
                d["{aa5}"] = Tohl5.ToString();

                //График температуры
                var chartLoader = new ChartLoader();
                var points = new List<(double,double)>();
                points.AddRange(new[] { 
                    (0,0), (t1sr,T1sr), (t1kon,T1kon), (t2sr,T2sr), (t2kon,T2kon), 
                    (t3sr,T3sr), (t3kon,T3kon), (t4sr,T4sr), (t4kon,T4kon),
                    (T01,Tohl1), (T02,Tohl2), (T03,Tohl3), (T04,Tohl4), (T05,Tohl5)
                });
                await chartLoader.GetTemperatureChart(points);


                //Метод эквивалентных величин
                var wN = 157 * (1 - (m.sN / 100));
                d["{wN}"] = wN.ToString();
                var wK = 157 * (1 - (m.sK / 100));
                d["{wK}"] = wK.ToString();
                var Mm = Mnom * m.mM;
                d["{Mm}"] = Mm.ToString();
                double wM = 22;
                var Mp = Mnom * m.mP;
                d["{Mp}"] = Mp.ToString();

                var Inom = m.P2nom * 1000 / (Math.Sqrt(3) * 380 * kpd0 * m.cosFi);
                d["{Inom}"] = Inom.ToString();
                var sinFi = Math.Sqrt(1 - Math.Pow(m.cosFi, 2));
                d["{sinFi}"] = sinFi.ToString();
                var Ip = m.ki * Inom;
                d["{Ip}"] = Ip.ToString();
                var Ik = 0.75 * Ip;
                d["{Ik}"] = Ik.ToString();
                var I0 = Inom * (sinFi - (m.cosFi / (2 * m.mK)));
                d["{I0}"] = I0.ToString();


                //Характеристики на графике



                
                break;
            }
            
            foreach (var key in d.Keys)
            {
                d[key] = ShortenStringNum(d[key]);
            }

            return d;
        }

        private static string ShortenStringNum(string s)
        {
            int pointPosition = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                { 
                    pointPosition = i;
                    for (int j = 3; j > 0; j--)
                    {
                        if (pointPosition + j <= s.Length)
                        { 
                            return s.Substring(0, pointPosition + j);
                        }
                    }
                }
            }
            return s;
        }

    }
}
