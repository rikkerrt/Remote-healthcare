using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace FietsDemo {
    class Program {

        private static bool FirstRun = false;
        private static double DurationDeviation = 0;
        private static double DistanceDeviation = 0;
        static async Task Main(string[] args) {
            int errorCode = 0;
            BLE bleBike = new BLE();
            BLE bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList) {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 01140");
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services) {
                Console.WriteLine($"Service: {service.Name}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            await bleHeart.SetService("HeartRate");
            // __TODO__ error check

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            await bleHeart.SubscribeToCharacteristic("HeartRate");

            sendResistance(bleBike);

            Console.Read();
        }

        private static void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e) {
            //Console.WriteLine("ontvangen");
            //Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
            //    BitConverter.ToString(e.Data).Replace("-", " "),
            //Encoding.UTF8.GetString(e.Data));

            String filter = BitConverter.ToString(e.Data).Replace("-", " ");

            if (filter.Substring(0, 2).Equals("16")) {
                Console.WriteLine(filter + "\n");
            }
            else {
                if (filter.Substring(12, 2).Equals("10")) {
                    if (FirstRun) {
                        Console.WriteLine("Je bent in de IF");
                        DurationDeviation = GetDuration(filter.Substring(18, 2));
                        DistanceDeviation = GetDistance(filter.Substring(21, 2));
                        FirstRun = false;
                    }

                    //Console.WriteLine("Afstand: " + GetDistance("aa"));
                    //Console.WriteLine("Snelheid: " + GetSpeed(filter.Substring(24, 2), filter.Substring(27, 2)) + " Km/h");
                    //Console.WriteLine("Tijd: " + GetDuration("ab" + " S"));

                    double Speed = GetSpeed(filter.Substring(24, 2), filter.Substring(27, 2));
                    Console.WriteLine(Speed);
                    double Duration = GetDuration(filter.Substring(18, 2)) - DurationDeviation;
                    double Distance = GetDistance(filter.Substring(21, 2)) - DistanceDeviation;

                    //Console.WriteLine("Tijdsduur: " + Duration);
                    //Console.WriteLine("Afstand: " + Distance);
                    //Console.WriteLine("Snelheid: " + Speed + "\n");
                }
                else {
                    Console.WriteLine(filter + "\n");
                }
            }
        }

        public static void DataReceived(string data) {
            if (data.Substring(0, 2).Equals("16")) {
                Console.WriteLine(data + "\n");
            }
            else {
                if (data.Substring(12, 2).Equals("10")) {
                    if (FirstRun) {
                        Console.WriteLine("Je bent in de IF");
                        DurationDeviation = GetDuration(data.Substring(18, 2));
                        DistanceDeviation = GetDistance(data.Substring(21, 2));
                        FirstRun = false;
                    }

                    //Console.WriteLine("Afstand: " + GetDistance("aa"));
                    //Console.WriteLine("Snelheid: " + GetSpeed(filter.Substring(24, 2), filter.Substring(27, 2)) + " Km/h");
                    //Console.WriteLine("Tijd: " + GetDuration("ab" + " S"));

                    double Speed = GetSpeed(data.Substring(24, 2), data.Substring(27, 2));
                    double Duration = GetDuration(data.Substring(18, 2)) - DurationDeviation;
                    double Distance = GetDistance(data.Substring(21, 2)) - DistanceDeviation;

                    Console.WriteLine("Tijdsduur: " + Duration);
                    Console.WriteLine("Afstand: " + Distance);
                    Console.WriteLine("Snelheid: " + Speed + "\n");
                }
                else {
                    Console.WriteLine(data + "\n");
                }
            }
        }


        public static void sendResistance(BLE bleBike) {

            byte[] bytes = { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };  //laatste veranderen 

            byte checkSum = 0x00;
            for (int i = 0; i < bytes.Length - 1; i++) {
                checkSum ^= bytes[i];
            }

            byte[] toSend = new byte[bytes.Length + 1];
            bytes.CopyTo(toSend, 0);
            toSend[toSend.Length - 1] = checkSum;

            bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", toSend);
            Console.WriteLine("done");
        }

        private static int HexToDecimal(string hexValue) {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;

        }

        private static double GetSpeed(string LSB, string MSB) {
            //string TotalHexValue = MSB + LSB;
            //int DecValue = HexToDecimal(TotalHexValue);
            //Double SpeedInKmH = (DecValue * 0.001) * 3.6;
            //return SpeedInKmH;

            return Calculations.GetSpeed(LSB, MSB);
        }

        public static double GetDistance(string distanceValue) {

            //int decValue = HexToDecimal(distanceValue);
            //if (decValue < lastDistanceValue) {
            //    DistanceCount = DistanceCount + 1;
            //}
            //Distance = decValue + (DistanceCount * 255);
            //lastDistanceValue = decValue;

            return Calculations.GetDistance(distanceValue);

        }

        public static double GetDuration(string HexDurationValue) {

            //int decValue = HexToDecimal(HexDurationValue);
            //if (decValue < lastDurationValue) {
            //    DurationCount = DurationCount + 1;
            //}
            //Duration = decValue + (DurationCount * 255);
            //lastDurationValue = decValue;

            return Calculations.GetDuration(HexDurationValue);

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
            string TotalHexValue = MSB + LSB;
            int DecValue = HexToDecimal(TotalHexValue);
            Double SpeedInKmH = (DecValue * 0.001) * 3.6;
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