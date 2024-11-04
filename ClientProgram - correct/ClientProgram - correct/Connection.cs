using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ClientProgram___correct
{
    class Connection : IBike
    {
        private static string Speed = "00";
        private static string Duration = "00";
        private static string Distance = "00";
        private static string HeartBeat = "00";
        private static BLE bleBike;


        public Connection() 
        { 
            Thread connection = new Thread(new ThreadStart(run));
            connection.Start();
        }

        public async void run()
        {
            int errorCode = 0;
            bleBike = new BLE();
            BLE bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 01140");
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services)
            {
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

            Console.Read();
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            //Console.WriteLine("ontvangen");
            //Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
            //    BitConverter.ToString(e.Data).Replace("-", " "),
            //Encoding.UTF8.GetString(e.Data));

            String filter = BitConverter.ToString(e.Data).Replace("-", " ");

            if (filter.Substring(0, 2).Equals("16"))
            {
                Console.WriteLine(filter + "\n");
            }
            else
            {
                if (filter.Substring(12, 2).Equals("10"))
                {
                    //if (FirstRun)
                    //{
                    //    Console.WriteLine("Je bent in de IF");
                    //    DurationDeviation = GetDuration(filter.Substring(18, 2));
                    //    DistanceDeviation = GetDistance(filter.Substring(21, 2));
                    //    FirstRun = false;
                    //}

                    //Console.WriteLine("Afstand: " + GetDistance("aa"));
                    //Console.WriteLine("Snelheid: " + GetSpeed(filter.Substring(24, 2), filter.Substring(27, 2)) + " Km/h");
                    //Console.WriteLine("Tijd: " + GetDuration("ab" + " S"));

                    Speed = filter.Substring(24, 2) + filter.Substring(27, 2);
                    //double Speed = GetSpeed(filter.Substring(24, 2), filter.Substring(27, 2));
                    //Console.WriteLine(Speed);
                    Duration = filter.Substring(18, 2);
                    //double Duration = GetDuration(filter.Substring(18, 2)) - DurationDeviation;
                    Distance = filter.Substring(21, 2);
                    //double Distance = GetDistance(filter.Substring(21, 2)) - DistanceDeviation;

                    //Console.WriteLine("Tijdsduur: " + Duration);
                    //Console.WriteLine("Afstand: " + Distance);
                    //Console.WriteLine("Snelheid: " + Speed + "\n");
                }
                else
                {
                    Console.WriteLine(filter + "\n");
                }
            }
        }

        public void sendResistance(int resistance)
        {
            byte data = 0x00;
            if (resistance > 200)
            {
                data = (byte)200;
            }
            else if (resistance < 0)
            {
                data = (byte)0;
            }
            else
            {
                data = (byte)resistance;
            }


            byte[] bytes = { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, data };  //laatste veranderen 

            byte checkSum = 0x00;
            for (int i = 0; i < bytes.Length - 1; i++)
            {
                checkSum ^= bytes[i];
            }

            byte[] toSend = new byte[bytes.Length + 1];
            bytes.CopyTo(toSend, 0);
            toSend[toSend.Length - 1] = checkSum;

            bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", toSend);
            Console.WriteLine("done");
        }

        public static void sendResistance(BLE bleBike)
        {

            byte[] bytes = { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };  //laatste veranderen 

            byte checkSum = 0x00;
            for (int i = 0; i < bytes.Length - 1; i++)
            {
                checkSum ^= bytes[i];
            }

            byte[] toSend = new byte[bytes.Length + 1];
            bytes.CopyTo(toSend, 0);
            toSend[toSend.Length - 1] = checkSum;

            bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", toSend);
            Console.WriteLine("done");
        }

        private static int HexToDecimal(string hexValue)
        {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;

        }

        public string getSpeed()
        {
            return Connection.Speed;
        }

        public string getDistance()
        {
            return Distance;
        }

        public string getDuration()
        {
            return Duration;
        }

        public string getHeartBeat()
        {
            return Connection.HeartBeat;
        }
    }
}
