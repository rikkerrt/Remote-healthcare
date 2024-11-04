using System;

namespace ClientProgram___correct
{
    
    class Calculations
    {
        private static double distance = 0;
        private static int distanceCount = 0;
        private static int lastDistanceValue;

        private static double duration = 0;
        private static int durationCount = 0;
        private static int lastDurationValue;

        private static int HexToDecimal(string hexValue)
        {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;

        }

        public static double GetSpeed(string LSB, string MSB)
        {
            string TotalHexValue = MSB + LSB;
            //Console.WriteLine(TotalHexValue);

            int DecValue = Convert.ToInt32(TotalHexValue, 16);
            //Console.WriteLine(DecValue);
            double SpeedInKmH = DecValue * 3.6 / 10;
            return SpeedInKmH;
        }


        public static double GetDistance(string distanceValue)
        {

            int decValue = HexToDecimal(distanceValue);
            if (decValue < lastDistanceValue)
            {
                distanceCount = distanceCount + 1;
            }
            distance = decValue + (distanceCount * 255);
            lastDistanceValue = decValue;

            return distance;

        }

        public static int GetDuration(string HexDurationValue)
        {

            int decValue = HexToDecimal(HexDurationValue);
            if (decValue < lastDurationValue)
            {
                durationCount = durationCount + 1;
            }
            duration = decValue + (durationCount * 255);
            lastDurationValue = decValue;

            return (int)duration / 4;

        }

        public static int getHeartBeat(string HeartBeatValue)
        {
            return HexToDecimal(HeartBeatValue);
        }
    }
}
