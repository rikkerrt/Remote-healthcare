using FietsDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProgram {
    class Simulation : IBike { 
        static Simulation() {

//            Console.WriteLine("what is the speed?");
//            int speed = int.Parse(Console.ReadLine());
//            string speedString = speed.ToString("X4");

//            Console.WriteLine("What is the heartrate");
//            int heartrate = int.Parse(Console.ReadLine());
//            string heartrateHexString = heartrate.ToString("X2");
            
//            int time = 00;

//            int differnce = 4 - speedString.Length;

//            /*for (int i = 0; differnce > i; i++) {
//                speedString =  "0" + speedString;
//                Console.WriteLine(speedString);
//            }*/


//            string LSB = speedString.Substring(0, 2);
//            string MSB = speedString.Substring(2);

//            //Console.WriteLine(LSB);
//            //Console.WriteLine(MSB);

//            while (true) {
//                time ++;
//                string Fietsdata = "A4 09 4E 05 10 19 " + time.ToString("X2") + " 00 " + LSB + " " + MSB  + " " + heartrateHexString + " 24 84";

                Console.WriteLine(Fietsdata);
                FietsDemo.Program.DataReceived(Fietsdata);
                Thread.Sleep(1000);
            }
        }

        public String getSpeed()
        {
            return "speed";
        }

        public String getDistance()
        {
            return "distance";
        }

        public String getDuration()
        {
            return "duration";
        }

        public String getHeartBeat()
        {
            return "heartbeat";
        }

    }
}
