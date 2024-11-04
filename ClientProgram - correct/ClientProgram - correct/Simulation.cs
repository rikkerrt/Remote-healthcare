using ClientProgram___correct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientProgram {
    class Simulation : IBike {

        private int Mode;
        private string Speed = "00";
        private string Distance = "00";
        private int Duration = 0;
        private int Resistance = 0;
        private string HeartBeat = "00";
        private Random r = new Random(); 
        private bool up = true;
        public Simulation(int mode) {
            Mode = mode;
            Thread simulation = new Thread(new ThreadStart(run));
            simulation.Start();
        }

        public void run()
        {
            while (true)
            {
                if (Mode == 1)
                {
                    ModeOne();
                }
                else if (Mode == 2)
                {
                    ModeTwo();
                }
                else if (Mode == 3)
                {
                    ModeThree();
                }
                else if (Mode == 4)
                {
                    ModeFour();
                }

                durationReset();

                string Fietsdata = "A4 09 4E 05 10 19 " + Duration.ToString("X2") + " 00 " + Speed.Substring(2) + " " + Speed.Substring(0, 2) + " " + HeartBeat + " 24 84";

                //Console.WriteLine(Fietsdata);
                //Program.DataReceived(Fietsdata);
                Thread.Sleep(1000);
            }
        }

        public void ModeOne() {
            Speed = "B55C";
            Distance = "11";
            Duration++;
            HeartBeat = "32";
        }

        public void ModeTwo()
        {
            int randomSpeed = r.Next(0, 65535);
            int randomDistance = r.Next(0, 255);
            Duration++;
            int randomHeartBeat = r.Next(0, 255);

            Speed = randomSpeed.ToString("X");
            Distance = randomDistance.ToString("X");
            HeartBeat = randomHeartBeat.ToString("X");

            Speed = fillHex(Speed, 4);
            Distance = fillHex(Distance, 2);
            HeartBeat = fillHex(HeartBeat, 2);

        }

        public void ModeThree()
        {
            int randomSpeed = r.Next(0, 10);
            int randomDistance = r.Next(0, 255);
            Duration++;
            int randomHeartBeat = r.Next(0, 10);

            int speed;
            int heartBeat;

            if (r.Next(0, 3) <= 1)
            {
                speed = Convert.ToInt32(Speed, 16) + randomSpeed;
                if (speed > 65535)
                {
                    speed = 65535;
                }
                heartBeat = Convert.ToInt32(HeartBeat, 16) + randomHeartBeat;
                if (heartBeat > 255)
                {
                    heartBeat = 255;
                }
            } else
            {
                speed = Convert.ToInt32(Speed, 16) - randomSpeed;
                if (speed < 0)
                {
                    speed = 0;
                }
                heartBeat = Convert.ToInt32(HeartBeat, 16) - randomHeartBeat;
                if (heartBeat < 0)
                {
                    heartBeat = 0;
                }
            }

            Speed = speed.ToString("X");
            Distance = randomDistance.ToString("X");
            HeartBeat = heartBeat.ToString("X");

            Speed = fillHex(Speed, 4);
            Distance = fillHex(Distance, 2);
            HeartBeat = fillHex(HeartBeat, 2);
        }

        public void ModeFour()
        {
            int speed;
            int distance;
            int heartBeat;
            if (up)
            {
                speed = Convert.ToInt32(Speed, 16) + 257;
                distance = Convert.ToInt32(Distance, 16) + 1;
                heartBeat = Convert.ToInt32(HeartBeat, 16) + 1;
                if (distance == 255)
                {
                    up = false;
                }
            }
            else 
            {
                speed = Convert.ToInt32(Speed, 16) - 257;
                distance = Convert.ToInt32(Distance, 16) - 1;
                heartBeat = Convert.ToInt32(HeartBeat, 16) - 1;
                if (distance == 0)
                {
                    up = true;
                }
            }

            Speed = speed.ToString("X");
            Distance = distance.ToString("X");
            HeartBeat = heartBeat.ToString("X");

            Speed = fillHex(Speed, 4);
            Distance = fillHex(Distance, 2);
            HeartBeat = fillHex(HeartBeat, 2);
            Duration++;

        }

        public string fillHex(string value, int max)
        {
            string input = value;
            while (input.Length < max)
            {
                input = "0" + input;
            }
            return input;
        }

        public void durationReset()
        {
            if (Duration > 255)
            {
                Duration = 0;
            }
        }

        public void sendResistance(int resistance)
        {
            Console.WriteLine(resistance);
            Resistance = resistance;
        }




        public string getSpeed()
        {
            return Speed;
        }

        public string getDistance()
        {
            return Distance;
        }

        public string getDuration()
        {
            return Duration.ToString("X");
        }

        public string getHeartBeat()
        {
            return HeartBeat;
        }

        public int getResistance()
        {
            return Resistance;
        }
    }
}