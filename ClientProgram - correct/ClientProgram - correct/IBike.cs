using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProgram___correct
{
    interface IBike
    {
        string getSpeed();
        int getResistance();
        string getDistance();
        string getDuration();
        string getHeartBeat();
        void sendResistance(int resistance);
    }
}
