using FietsDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientProgram {
    class Simulation {
        private string speedstring;
        private int heartrateHexString;
        private string LSB;
        private string MSB;
        private int time;

        public Simulation() {
            Console.WriteLine("What mode do you want to use?\n" +
                              "1) Constante waardes\n" +
                              "2) Lineare waarden\n" +
                              "3) Random waardes");

            string mode = Console.ReadLine();

            switch (mode) {
               case "1":
                    askValues();
                    GenerateConstantValues();
                    break;
               case "2":
                    askValues();
                    GenerateLinearValues();
                    break;
               case "3":
                    askValues();
                    break;
               default:
                    break;

            }

            //Console.WriteLine(LSB);
            //Console.WriteLine(MSB);

            while (true) {
                this.time++;

                Thread.Sleep(1000);
            }
        }

        private void askValues() {
            Console.WriteLine("what is the speed?");
            int speed = int.Parse(Console.ReadLine());
            string speedString = speed.ToString("X4");

            Console.WriteLine("What is the heartrate");
            int heartrate = int.Parse(Console.ReadLine());
            string heartrateHexString = heartrate.ToString("X2");

            int time = 00;

            //int differnce = 4 - speedString.Length;
            /*for (int i = 0; differnce > i; i++) {
                speedString =  "0" + speedString;
                Console.WriteLine(speedString);
            }*/
        }

        public void GenerateConstantValues() {
            while (true) {
                LSB = speedstring.Substring(0, 2);
                MSB = speedstring.Substring(2);

                string Fietsdata = "A4 09 4E 05 10 19 " + time.ToString("X2") + " 00 " + LSB + " " + MSB + " " + heartrateHexString + " 24 84";
                Console.WriteLine(Fietsdata);
                Program.DataReceived(Fietsdata);
            } 
        }

        public void GenerateLinearValues() {
            while(true) {


                string Fietsdata = "A4 09 4E 05 10 19 " + time.ToString("X2") + " 00 " + LSB + " " + MSB + " " + heartrateHexString + " 24 84";

                Console.WriteLine(Fietsdata);
                Program.DataReceived(Fietsdata);
            }
        }
    }
}
