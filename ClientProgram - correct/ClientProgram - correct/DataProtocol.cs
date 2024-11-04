using System;

namespace ClientProgram___correct
{
    internal class DataProtocol
    {
        static IBike iBike;
        public DataProtocol(IBike bike) 
        { 
            iBike = bike;
        }

        public static void DataRecieved(string s)
        {
            if (s.StartsWith("setResistance:"))
            {
                string[] subs = s.Split(':');
                int resistance = 10;
                try
                {
                    resistance = Int32.Parse(subs[1]);
                } catch (Exception e) 
                { 
                    Console.WriteLine("resistence not a number so has been set to 10");   
                }
                iBike.sendResistance(resistance);
            }
            else
            {
                Console.WriteLine(s);
            }
        }
    }
}
