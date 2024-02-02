using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Office.Interop.Word;
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
                var w0 = 2 * Math.PI * m.n0 / 60;
                d["{w0}"] = w0.ToString();
                var Mnom = m.P2nom * 1000 / (w0 * (1 - (m.sN / 100)));
                d["{Mnom}"] = Mnom.ToString();
                var Mk = Mnom * m.mK;
                d["{Mk}"] = Mk.ToString();
                var Pmax = new[] { v.P1, v.P2, v.P3, v.P4 }.Max() * 1000;
                d["{Pmax}"] = Pmax.ToString();
                d["{PmaxK}"] = (Pmax / 1000).ToString();
                var Msmax = Pmax / (w0 * (1 - (m.sK / 100)));
                d["{Msmax}"] = Msmax.ToString();
                var Mk09 = Mk * Math.Pow(1 - 0.1, 2);
                d["{Mk09}"] = Mk09.ToString();
                
                if (Mk09 < Msmax)
                    continue;
                //if (m.P2nom < 23)
                //    continue;

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
                points.AddRange(new[] 
                { 
                    (0,0), (t1sr,T1sr), (t1kon,T1kon), (t2sr,T2sr), (t2kon,T2kon), 
                    (t3sr,T3sr), (t3kon,T3kon), (t4sr,T4sr), (t4kon,T4kon),
                    (T01,Tohl1), (T02,Tohl2), (T03,Tohl3), (T04,Tohl4), (T05,Tohl5)
                });
                await chartLoader.GetTemperatureChart(points);


                //Метод эквивалентных величин
                var wN = w0 * (1 - (m.sN / 100));
                d["{wN}"] = wN.ToString();
                var wK = w0 * (1 - (m.sK / 100));
                d["{wK}"] = wK.ToString();
                var Mm = Mnom * m.mM;
                d["{Mm}"] = Mm.ToString();
                var wM = w0 * (1 - 0.86);
                d["{wM}"] = wM.ToString();
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
                var dw2 = ((int)w0) / 6 + (w0 % 6);
                d["{dw2}"] = dw2.ToString();
                int dw1 = 25; //((int)w0) / 6;
                d["{dw1}"] = dw1.ToString();
                double w1 = dw1;
                d["{w1}"] = dw1.ToString();
                double w2 = w1 + dw1;
                d["{w2}"] = w2.ToString();
                double w3 = w2 + dw1;
                d["{w3}"] = w3.ToString();
                double w4 = w3 + dw1;
                d["{w4}"] = w4.ToString();
                double w5 = w4 + dw1;
                d["{w5}"] = w5.ToString();
                double w6 = w5 + dw1;
                d["{w6}"] = w6.ToString();

                var Mstatic = Mnom * 0.3;
                d["{Mst}"] = Mstatic.ToString();

                var MstList = new List<(double, double)>();
                MstList.AddRange(new[]
                {
                    (Mstatic, 0.0), (Mstatic, w1), (Mstatic, w2), (Mstatic, w3), (Mstatic, w4), (Mstatic, w5), (Mstatic, w6), (Mstatic, w6 + dw1)
                });
                
                var MwList = new List<(double, double)>();
                MwList.AddRange(new[]
                {//Некоторые точки дублируются, чтобы работал CatmullRomInterpolator
                    (0.0, w0), (0.0, w0), (Mnom, wN), (Mk, wK), (Mm, wM), (Mp, 0.0), (Mp, 0.0)
                });
                //{
                //    (0.0, w0), (Mnom, wN), (Mk, wK), (Mm, wM), (Mp, 0.0)
                //});

                var IwList = new List<(double, double)>();
                IwList.AddRange(new[]
                {//Некоторые точки дублируются, чтобы работал CatmullRomInterpolator
                    (I0, w0), (I0, w0), (Inom, wN), (Ik, wK), (Ip, 0.0), (Ip, 0.0)
                });
                //{
                //    (I0, w0), (Inom, wN), (Ik, wK), (Ip, 0.0)
                //});

                //var MwListInterpolated = GenerateInterpolatedPoints(MwList);
                //var IwListInterpolated = GenerateInterpolatedPoints(IwList);
                var MwListInterpolated = CatmullRomSpline.Generate(MwList.ToArray(), 1000).ToList();
                var IwListInterpolated = CatmullRomSpline.Generate(IwList.ToArray(), 1000).ToList();

                var w1Points = new[]
                {
                    (0.0, w1),
                    (MwList.Select(x => x.Item1).Max() + 30, w1)
                };
                var w2Points = new[]
                {
                    (0.0, w2),
                    (MwList.Select(x => x.Item1).Max() + 30, w2)
                };
                var w3Points = new[]
                {
                    (0.0, w3),
                    (MwList.Select(x => x.Item1).Max() + 30, w3)
                };
                var w4Points = new[]
                {
                    (0.0, w4),
                    (MwList.Select(x => x.Item1).Max() + 30, w4)
                };
                var w5Points = new[]
                {
                    (0.0, w5),
                    (MwList.Select(x => x.Item1).Max() + 30, w5)
                };
                var wust = FindNearestPointByX(MwListInterpolated, Mstatic).Item2;
                var w6Points = new[]
                {
                    (0.0, w6),
                    (MwList.Select(x => x.Item1).Max() + 30, w6)
                };
                var wList = new List<(double, double)[]>();
                wList.AddRange(new[] { w1Points, w2Points, w3Points, w4Points, w5Points, w6Points });

                
                //Графоаналитический метод
                var I31 = FindNearestPointByY(IwListInterpolated, w1).Item1;
                d["{I31}"] = I31.ToString();
                var I32 = FindNearestPointByY(IwListInterpolated, w2).Item1;
                d["{I32}"] = I32.ToString();
                var I33 = FindNearestPointByY(IwListInterpolated, w3).Item1;
                d["{I33}"] = I33.ToString();
                var I34 = FindNearestPointByY(IwListInterpolated, w4).Item1;
                d["{I34}"] = I34.ToString();
                var I35 = FindNearestPointByY(IwListInterpolated, w5).Item1;
                d["{I35}"] = I35.ToString();
                var I36 = FindNearestPointByY(IwListInterpolated, w6).Item1;
                d["{I36}"] = I36.ToString();
                
                d["{M10}"] = Mp.ToString();
                var M11 = FindNearestPointByY(MwListInterpolated, w1).Item1;
                d["{M11}"] = M11.ToString();
                var M12 = FindNearestPointByY(MwListInterpolated, w2).Item1;
                d["{M12}"] = M12.ToString();
                var M13 = FindNearestPointByY(MwListInterpolated, w3).Item1;
                d["{M13}"] = M13.ToString();
                var M14 = FindNearestPointByY(MwListInterpolated, w4).Item1;
                d["{M14}"] = M14.ToString();
                var M15 = FindNearestPointByY(MwListInterpolated, w5).Item1;
                d["{M15}"] = M15.ToString();
                var M16 = FindNearestPointByY(MwListInterpolated, w6).Item1;
                d["{M16}"] = M16.ToString();

                var M40 = Mp - Mstatic;
                d["{M40}"] = M40.ToString();
                var M41 = M11 - Mstatic;
                d["{M41}"] = M41.ToString();
                var M42 = M12 - Mstatic;
                d["{M42}"] = M42.ToString();
                var M43 = M13 - Mstatic;
                d["{M43}"] = M43.ToString();
                var M44 = M14 - Mstatic;
                d["{M44}"] = M44.ToString();
                var M45 = M15 - Mstatic;
                d["{M45}"] = M45.ToString();
                var M46 = M16 - Mstatic;
                d["{M46}"] = M46.ToString();

                var M90 = (M40 + M41) / 2;
                d["{M90}"] = M90.ToString();
                var M91 = (M41 + M42) / 2;
                d["{M91}"] = M91.ToString();
                var M92 = (M42 + M43) / 2;
                d["{M92}"] = M92.ToString();
                var M93 = (M43 + M44) / 2;
                d["{M93}"] = M93.ToString();
                var M94 = (M44 + M45) / 2;
                d["{M94}"] = M94.ToString();
                var M95 = (M45 + M46) / 2;
                d["{M95}"] = M95.ToString();

                var Ii1 = v.P1 * 1000 / (Math.Sqrt(3) * 380 * kpd0 * m.cosFi);
                d["{Ii1}"] = Ii1.ToString();
                var Ii2 = v.P2 * 1000 / (Math.Sqrt(3) * 380 * kpd0 * m.cosFi);
                d["{Ii2}"] = Ii2.ToString();
                var Ii3 = v.P3 * 1000 / (Math.Sqrt(3) * 380 * kpd0 * m.cosFi);
                d["{Ii3}"] = Ii3.ToString();
                var Ii4 = v.P4 * 1000 / (Math.Sqrt(3) * 380 * kpd0 * m.cosFi);
                d["{Ii4}"] = Ii4.ToString();

                var Jsum = 3 * m.J;
                d["{Jsum}"] = Jsum.ToString();

                var dt1 = Jsum * dw1 / M90;
                d["{dt1}"] = dt1.ToString();
                var dt2 = Jsum * dw1 / M91;
                d["{dt2}"] = dt2.ToString();
                var dt3 = Jsum * dw1 / M92;
                d["{dt3}"] = dt3.ToString();
                var dt4 = Jsum * dw1 / M93;
                d["{dt4}"] = dt4.ToString();
                var dt5 = Jsum * dw1 / M94;
                d["{dt5}"] = dt5.ToString();
                var dt6 = Jsum * dw2 / M95;
                d["{dt6}"] = dt6.ToString();

                var tpuska = dt1 + dt2 + dt3 + dt4 + dt5 + dt6;
                d["{tpuska}"] = tpuska.ToString();

                var Iekvp = Math.Sqrt((I31*I31*dt1 + I32*I32*dt2 + I33*I33*dt3 + I34*I34*dt4 + I35*I35*dt5 + I36*I36*dt6) / tpuska);
                d["{Iekvp}"] = Iekvp.ToString();

                var IekvR = Math.Sqrt((Iekvp*Iekvp*tpuska + Ii1*Ii1*v.t1 + Ii2*Ii2*v.t2 + Ii3*Ii3*v.t3 + Ii4*Ii4*v.t4) / (tpuska + tsum));
                d["{IekvR}"] = IekvR.ToString();

                var tp1 = dt1;
                d["{tp1}"] = tp1.ToString();
                var tp2 = tp1 + dt2;
                d["{tp2}"] = tp2.ToString();
                var tp3 = tp2 + dt3;
                d["{tp3}"] = tp3.ToString();
                var tp4 = tp3 + dt4;
                d["{tp4}"] = tp4.ToString();
                var tp5 = tp4 + dt5;
                d["{tp5}"] = tp5.ToString();
                var tp6 = tp5 + dt6;
                d["{tp6}"] = tp6.ToString();

                var KM = Math.Sqrt((KT * (1 + 0.6)) - 0.6);
                d["{KM}"] = KM.ToString();

                var KMxInom = KM * Inom;
                d["{KMxInom}"] = KMxInom.ToString();
                
                if (KMxInom < IekvR)
                    continue;

                var MdinList = new List<(double, double)>();
                MdinList.AddRange(new[]
                {
                    (M40, 0.0), (M41, w1), (M42, w2), (M43, w3), (M44, w4), (M45, w5), (M46, w6), (0.0, wust)
                });

                var chartLoader2 = new ChartLoader();
                await chartLoader2.GetCharacteristicsChart(
                MstList,
                MwListInterpolated,
                IwListInterpolated,
                wList,
                MdinList);

                var ItList = new List<(double, double)>();
                ItList.AddRange(new[]
                {
                    (0.0, Ip), (tp1, I31), (tp2, I32), (tp3, I33), (tp4, I34), (tp5, I35), (tp6, I36)
                });

                var wtList = new List<(double, double)>();
                wtList.AddRange(new[]
                {
                    (0.0, 0.0), (tp1, w1), (tp2, w2), (tp3, w3), (tp4, w4), (tp5, w5), (tp6, w6)
                });

                var chartloader3 = new ChartLoader();
                await chartloader3.GetCharacteristicsChart2(ItList, wtList);

                var chartloader4 = new ChartLoader();
                await chartloader4.GetPowerDiagram(v);

                var currentDiagramPoints = new List<(double, double)>();
                currentDiagramPoints.AddRange(new[]
                {
                    (0.0, 0.0), (0.0, Ip), (tpuska, Ip), (tpuska, Ii1), (v.t1 + tpuska, Ii1),
                    (v.t1 + tpuska, Ii2), (v.t2 + v.t1 + tpuska, Ii2), (v.t2 + v.t1 + tpuska, Ii3),
                    (v.t3 + v.t2 + v.t1 + tpuska, Ii3), (v.t3 + v.t2 + v.t1 + tpuska, Ii4),
                    (v.t4 + v.t3 + v.t2 + v.t1 + tpuska, Ii4), (v.t4 + v.t3 + v.t2 + v.t1 + tpuska, 0.0)
                });
                var chartLoader5 = new ChartLoader();
                await chartLoader5.GetCurrentDiagram(currentDiagramPoints);

                //Расчет и выбор автоматического выключателя

                var Awork = ((v.P1 * v.t1 / kpd1) + (v.P2 * v.t2 / kpd2) + (v.P3 * v.t3 / kpd3) + (v.P4 * v.t4 / kpd4)) / 60;
                d["{Awork}"] = Awork.ToString();

                break;
            }
            
            foreach (var key in d.Keys)
            {
                d[key] = ShortenStringNum(d[key]);
            }

            return d;
        }

        private static (double, double) FindNearestPointByY(List<(double, double)> points, double yPoint)
        {
            var nearestY = points.Select(p => p.Item2).MinBy(y => Math.Abs(y - yPoint));
            var nearestPoint = points.Where(p => p.Item2 == nearestY);
            return nearestPoint.FirstOrDefault();
        }

        private static (double, double) FindNearestPointByX(List<(double, double)> points, double  xPoint)
        {
            var nearestX = points.Select(p => p.Item1).MinBy(x => Math.Abs(x - xPoint));
            var nearestPoint = points.Where(p => p.Item1 == nearestX);
            return nearestPoint.FirstOrDefault();
        }

        private static string ShortenStringNum(string s)
        {
            int pointPosition = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                { 
                    pointPosition = i;
                    for (int j = 4; j > 0; j--)
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

        private static List<(double,double)> GenerateInterpolatedPoints(List<(double, double)> inputPoints, double step = 10)
        {
            var xValues = new List<double>();
            var yValues = new List<double>();
            var result = new List<(double,double)>();

            foreach (var point in inputPoints)
            {
                xValues.Add(point.Item1);
                yValues.Add(point.Item2);
            }

            var points = NormalSplineInterpolator.InterpolateXY(xValues.ToArray(), yValues.ToArray(), 1000); //1000 - плотность точек графика

            for (int i = 0; i < points.xs.Length; i++)
            {
                result.Add((points.xs[i], points.ys[i]));
            }

            return result;
        }
    }
}
