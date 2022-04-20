using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpShootingGame
{
    // Enemy, Bullet의 부모클래스
    public class EpObject
    {
        // 오차 범위를 줄이기 위해 double을 사용
        public double x { get; set; }       // X 위치
        public double y { get; set; }       // Y 위치
        protected int width { get; set; }   // 객체 이미지의 가로 길이
        protected int height { get; set; }  // 객체 이미지의 세로 길이

        // X 프로퍼티
        public double X
        {
            get
            {
                return x;
            }
        }
        // Y 프로퍼티
        public double Y
        {
            get
            {
                return y;
            }
        }
    }
}
