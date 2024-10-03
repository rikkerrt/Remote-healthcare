﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;
using ClientProgram;
using ClientProgram___correct;

namespace FietsDemo {
    class Program {

        private static bool FirstRun = false;
        private static double DurationDeviation = 0;
        private static double DistanceDeviation = 0;
        static async Task Main(string[] args) {
            IBike connection = new Connection();
            Console.WriteLine(connection.getSpeed);
            while (true)
            {
                Console.WriteLine();
            }
        }




    }
    class Calculations {
        private static double Distance = 0;
        private static int DistanceCount = 0;
        private static int lastDistanceValue;

        private static double Duration = 0;
        private static int DurationCount = 0;
        private static int lastDurationValue;

        private static int HexToDecimal(string hexValue) {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;

        }

        public static double GetSpeed(string LSB, string MSB) {
            string TotalHexValue = LSB + MSB;

            int DecValue = Convert.ToInt32(TotalHexValue, 16);
            double SpeedInKmH = DecValue * 3.6;
            return SpeedInKmH;
        }


        public static double GetDistance(string distanceValue) {

            int decValue = HexToDecimal(distanceValue);
            if (decValue < lastDistanceValue) {
                DistanceCount = DistanceCount + 1;
            }
            Distance = decValue + (DistanceCount * 255);
            lastDistanceValue = decValue;

            return Distance;

        }

        public static double GetDuration(string HexDurationValue) {

            int decValue = HexToDecimal(HexDurationValue);
            if (decValue < lastDurationValue) {
                DurationCount = DurationCount + 1;
            }
            Duration = decValue + (DurationCount * 255);
            lastDurationValue = decValue;

            return Duration / 4;

        }
    }
}