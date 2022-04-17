using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpShootingGame
{
    public class Obj
    {
        public double x { get; set; }
        public double y { get; set; }
        protected int width { get; set; }
        protected int height { get; set; }

        public double X
        {
            get
            {
                return x;
            }
        }
        public double Y
        {
            get
            {
                return y;
            }
        }
    }
}
