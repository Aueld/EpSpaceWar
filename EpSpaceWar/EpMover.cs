using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpShootingGame
{
    public enum Dir
    {
        Shot = 0, Left = 1, Right = 2, Up = 4, Down = 8, Slow = 16, reset, start,
    }
    public class EpMover
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Dir Dir { get; set; }
        public float Speed { get; set; }
        public int Life { get; set; }

        public EpMover(float x, float y, int life)
        {
            X = x;
            Y = y;
            Life = life;
        }
    }
}
