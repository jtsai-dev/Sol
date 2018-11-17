using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using System.Device.Location;

namespace JT.Infrastructure
{
    public class MathExtensions
    {
        /// <summary>
        /// Returns the greatest common denominator between value1 and value2
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>The greatest common denominator if one exists</returns>
        public static int GreatestCommonDenominator(int value1, int value2)
        {
            if (value1 == int.MinValue)
                throw new ArgumentOutOfRangeException("value1", "Value can not be Int32.MinValue");
            if (value2 == int.MinValue)
                throw new ArgumentOutOfRangeException("value2", "Value can not be Int32.MinValue");

            value1 = System.Math.Abs(value1);
            value2 = System.Math.Abs(value2);

            while (value1 != 0 && value2 != 0)
            {
                if (value1 > value2)
                    value1 %= value2;
                else
                    value2 %= value1;
            }
            return value1 == 0 ? value2 : value1;
        }

        public static double DistanceBetweenGeoCoordinates(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var sCoord = new GeoCoordinate(sLatitude, sLongitude);
            var eCoord = new GeoCoordinate(eLatitude, eLongitude);

            return sCoord.GetDistanceTo(eCoord);
        }
    }
}
