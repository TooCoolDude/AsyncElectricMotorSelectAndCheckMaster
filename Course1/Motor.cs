using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseDeliverer
{
    public record Motor()
    {
        /// <summary>
        /// Тип двигателя
        /// </summary>
        public string Type { get; init; }
        /// <summary>
        /// Коэффициент мощности
        /// </summary
        public double cosFi { get; init; }
        /// <summary>
        /// Номинальная мощность
        /// </summary>
        public double P2nom { get; init; }
        /// <summary>
        /// Скорость ВМП статора
        /// </summary>
        public double n0 { get; init; }
        /// <summary>
        /// КПД
        /// </summary>
        public double kpd { get; init; }
        /// <summary>
        /// Коэффициент пускового момента
        /// </summary>
        public double mP { get; init; }
        /// <summary>
        /// Коэффициент минимального момента
        /// </summary>
        public double mM { get; init; }
        /// <summary>
        /// Коэффициент критического момента
        /// </summary>
        public double mK { get; init; }
        /// <summary>
        /// Номинальное скольжение
        /// </summary>
        public double sN { get; init; }
        /// <summary>
        /// Критическое скольжение
        /// </summary>
        public double sK { get; init; }
        /// <summary>
        /// Коэффициент пускового тока
        /// </summary>
        public double ki { get; init; }
        /// <summary>
        /// Момент инерции двигателя
        /// </summary>
        public double J { get; init; }
        /// <summary>
        /// Масса двигателя
        /// </summary>
        public double m { get; init; }

        public override string ToString()
        {
            return $"{Type}, P2ном = {P2nom}";
        }
    }
}
