using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FietsDemo
{
    class Simulation
    {
        string data = "A4-09-4E-05-10-19-3F-8F-00-00-FF-24-84";
        string wrongData = "A4-09-4E-05-10-00-00-00-00-00-00-00-00";
        string speed = "00";
        string time = "00";
        string heartRate = "16-4F-4A-03";

        Random increment = new Random();
        public Simulation()
        {

        }

        public string requestData(string command)
        {
            switch (command)
            {
                case "wrong":
                    return wrongData;
                case "right":
                    return data;
                case "heartRate":
                    return heartRate;
                case "speed":
                    simulateSpeed(speed);
                    return speed;
                case "time":
                    simulateTime(time);
                    return time;

            }
            return "invalid command.";
        }
        public int simulateSpeed(string speed)
        {
            int speedToDecimal = int.Parse(speed,System.Globalization.NumberStyles.HexNumber);
            
            int incrementSpeed = increment.Next(10,20);

            Console.WriteLine(speedToDecimal + incrementSpeed);
            return speedToDecimal + incrementSpeed;
        }

        public void simulateTime(string time)
        {
            int timeToDecimal = int.Parse(speed, System.Globalization.NumberStyles.HexNumber);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(timeToDecimal + i);
            }
        }
    }
}
