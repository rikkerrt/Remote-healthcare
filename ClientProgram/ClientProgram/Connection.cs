using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProgram
{
    class Connection : IBike {

        public Connection() { 
        
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
