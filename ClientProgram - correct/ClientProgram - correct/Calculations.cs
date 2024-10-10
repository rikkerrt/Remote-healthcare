using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProgram___correct
{
    class Calculations
    {
        private static double Distance = 0;
        private static int DistanceCount = 0;
        private static int lastDistanceValue;

        private static double Duration = 0;
        private static int DurationCount = 0;
        private static int lastDurationValue;

        private static int HexToDecimal(string hexValue)
        {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;

        }

        public static double GetSpeed(string LSB, string MSB)
        {
            string TotalHexValue = MSB + LSB;
            Console.WriteLine(TotalHexValue);

            int DecValue = Convert.ToInt32(TotalHexValue, 16);
            Console.WriteLine(DecValue);
            double SpeedInKmH = DecValue * 3.6;
            return SpeedInKmH;
        }


        public static double GetDistance(string distanceValue)
        {

            int decValue = HexToDecimal(distanceValue);
            if (decValue < lastDistanceValue)
            {
                DistanceCount = DistanceCount + 1;
            }
            Distance = decValue + (DistanceCount * 255);
            lastDistanceValue = decValue;

            return Distance;

        }

        public static int GetDuration(string HexDurationValue)
        {

            int decValue = HexToDecimal(HexDurationValue);
            if (decValue < lastDurationValue)
            {
                DurationCount = DurationCount + 1;
            }
            Duration = decValue + (DurationCount * 255);
            lastDurationValue = decValue;

            return (int)Duration / 4;

        }

        public static int getHeartBeat(string HeartBeatValue)
        {
            return 1;
        }
    }
}
