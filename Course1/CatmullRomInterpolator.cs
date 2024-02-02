using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorSelectAndCheck
{
    public static class CatmullRomSpline
    {
        /// <summary>
        /// Generate spline series of points from array of keyframe points
        /// </summary>
        /// <param name="points">array of key frame points</param>
        /// <param name="numPoints">number of points to generate in spline between each point</param>
        /// <returns>array of points describing spline</returns>
        public static (double, double)[] Generate((double, double)[] points, int numPoints)
        {
            if (points.Length < 4)
                throw new ArgumentException("CatmullRomSpline requires at least 4 points", "points");

            var splinePoints = new List<(double, double)>();

            for (int i = 0; i < points.Length - 3; i++)
            {
                for (int j = 0; j < numPoints; j++)
                {
                    splinePoints.Add(PointOnCurve(points[i], points[i + 1], points[i + 2], points[i + 3], (1f / numPoints) * j));
                }
            }

            splinePoints.Add(points[points.Length - 2]);

            return splinePoints.ToArray();
        }

        /// <summary>
        /// Calculates interpolated point between two points using Catmull-Rom Spline
        /// </summary>
        /// <remarks>
        /// Points calculated exist on the spline between points two and three.
        /// </remarks>
        /// <param name="p0">First Point</param>
        /// <param name="p1">Second Point</param>
        /// <param name="p2">Third Point</param>
        /// <param name="p3">Fourth Point</param>
        /// <param name="t">
        /// Normalised distance between second and third point 
        /// where the spline point will be calculated
        /// </param>
        /// <returns>
        /// Calculated Spline Point
        /// </returns>
        public static (double,double) PointOnCurve((double, double) p0, (double, double) p1, (double, double) p2, (double, double) p3, float t)
        {
            (double, double) ret;

            float t2 = t * t;
            float t3 = t2 * t;

            ret.Item1 = 0.5f * ((2.0f * p1.Item1) +
            (-p0.Item1 + p2.Item1) * t +
            (2.0f * p0.Item1 - 5.0f * p1.Item1 + 4 * p2.Item1 - p3.Item1) * t2 +
            (-p0.Item1 + 3.0f * p1.Item1 - 3.0f * p2.Item1 + p3.Item1) * t3);

            ret.Item2 = 0.5f * ((2.0f * p1.Item2) +
            (-p0.Item2 + p2.Item2) * t +
            (2.0f * p0.Item2 - 5.0f * p1.Item2 + 4 * p2.Item2 - p3.Item2) * t2 +
            (-p0.Item2 + 3.0f * p1.Item2 - 3.0f * p2.Item2 + p3.Item2) * t3);

            return ret;
        }
    }
}
