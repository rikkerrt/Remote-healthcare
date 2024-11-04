using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ClientProgram___correct
{
    class Connection : IBike
    {
        private static string speed = "00";
        private static string duration = "00";
        private static string distance = "00";
        private static string heartBeat = "00";
        private static int Resistance = 0;
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

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 01249");
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            // __TODO__ Error check
                
            var services = bleBike.GetServices;
            //foreach (var service in services)
            //{
            //    Console.WriteLine($"Service: {service.Name}");
            //}

            Thread.Sleep(100);
            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            await bleHeart.SetService("HeartRate");
            // __TODO__ error check

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            //Console.WriteLine("bike connecion succesfull");
            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            await bleHeart.SubscribeToCharacteristic("HeartRate");

            Console.Read();
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            String filter = BitConverter.ToString(e.Data).Replace("-", " ");
            //Console.WriteLine("new reading got");

            if (filter.Substring(0, 2).Equals("16"))
            {
                heartBeat = filter.Substring(3,2);            }
            else
            {
                if (filter.Substring(0,2).Equals("A4") && filter.Substring(12, 2).Equals("10"))
                {
                    speed = filter.Substring(27, 2) + filter.Substring(24, 2);
                    duration = filter.Substring(18, 2);
                    distance = filter.Substring(21, 2);
                }
            }
        }

        public void sendResistance(int resistance)
        {
            Resistance = resistance;
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
            //Console.WriteLine("done");
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
        }

        private static int HexToDecimal(string hexValue)
        {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return decValue;
        }

        public string getSpeed()
        {
            return speed;
        }

        public string getDistance()
        {
            return distance;
        }

        public string getDuration()
        {
            return duration;
        }

        public int getResistance()
        {
            return Resistance;
        }

        public string getHeartBeat()
        {
            return heartBeat;
        }
    }
}
