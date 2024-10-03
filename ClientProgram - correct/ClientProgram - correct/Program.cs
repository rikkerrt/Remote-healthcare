using System;
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
            IBike connection = new Simulation();
            Console.WriteLine(connection.getSpeed());
            while (true)
            {
                //Console.WriteLine(connection.getSpeed());
            }
        }




    }
}