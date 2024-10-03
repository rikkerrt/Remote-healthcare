using ClientProgram___correct;
using FietsDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientProgram {
    class Simulation : IBike {

        private int Speed = 0;
        private string Distance = "00";
        private string Duration = "00";
        private string HeartBeat = "00";
        public Simulation() {
            Thread simulation = new Thread(new ThreadStart(run));
            simulation.Start();
        }

        public void run()
        {
            Console.WriteLine("what is the speed?");
            int speed = int.Parse(Console.ReadLine());
            string speedString = speed.ToString("X4");

            Console.WriteLine("What is the heartrate");
            int heartrate = int.Parse(Console.ReadLine());
            string heartrateHexString = heartrate.ToString("X2");

            int time = 00;

            int differnce = 4 - speedString.Length;

            /*for (int i = 0; differnce > i; i++) {
                speedString =  "0" + speedString;
                Console.WriteLine(speedString);
            }*/


            string LSB = speedString.Substring(0, 2);
            string MSB = speedString.Substring(2);

            //Console.WriteLine(LSB);
            //Console.WriteLine(MSB);

            while (true)
            {
                time++;
                Speed++;
                Console.WriteLine(Speed);
                string Fietsdata = "A4 09 4E 05 10 19 " + time.ToString("X2") + " 00 " + LSB + " " + MSB + " " + heartrateHexString + " 24 84";

                Console.WriteLine(Fietsdata);
                //Program.DataReceived(Fietsdata);
                Thread.Sleep(1000);
            }
        }
        
        public string getSpeed()
        {
            return Speed + "";
        }

        public string getDistance()
        {
            return "distance";
        }

        public string getDuration()
        {
            return "duration";
        }

        public string getHeartBeat()
        {
            return "heartBeat";
        }
    }
}