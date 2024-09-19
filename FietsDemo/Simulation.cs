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

        public Simulation()
        {

        }
        
        public void runSimulation(int times)
        {
            data = data.Replace("-", " ");
            string time = data.Substring(18, 2);
            int duration = int.Parse(time, System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < times; i++)
            {
                duration++;
                time = duration.ToString();
                Console.WriteLine(duration.ToString());
            }
        }
    }
}
