using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpShootingGame
{
    // Player의 부모 클래스

    public enum Dir     // Player의 방향 및 상태
    {
        Shot = 0, Left = 1, Right = 2, Up = 4, Down = 8, Slow = 16, reset, start,
    }

    // 생성자
    public class EpPlayable
    {
        public float X { get; set; }    // X 프로퍼티
        public float Y { get; set; }    // Y 프로퍼티
        public Dir Dir { get; set; }    // Dir 프로퍼티
        public float Speed { get; set; }// Speed 프로퍼티
        public int Life { get; set; }   // 생명 프로퍼티

        public EpPlayable(float x, float y, int life)   // 생성자
        {
            X = x;
            Y = y;
            Life = life;
        }
    }
}
