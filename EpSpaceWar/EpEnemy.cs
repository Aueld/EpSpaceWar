using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;

namespace EpShootingGame
{
    public class EpEnemy : Obj
    {
        protected bool isAlive;
        private Bitmap bmp { get; set; }
        //private int width;
        //private int height;
        private double ivx { get; set; }
        private double ivy { get; set; }
        private int fadeCount { get; set; }
        private bool isDesappeared = false;

        public bool IsDesappeared
        {
            get
            {
                return isDesappeared;
            }
        }
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
        }
        public int hp { get; set; }

        public EpEnemy(Bitmap ShipBMP, int hp, double x, double y, double ivx, double ivy)
        {
            bmp = ShipBMP;
            width = bmp.Width;
            height = bmp.Height;
            bmp.MakeTransparent();
            
            this.hp = hp;
            this.x = x - width / 2;
            this.y = y - height / 2;
            this.ivx = ivx;
            this.ivy = ivy;
            isAlive = true;
        }

        internal void Draw(Graphics g)
        { 
            g.DrawImage(bmp, (int)(x), (int)(y));
        }

        public void Fade(Bitmap enemyImg, int scrollY)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var tp = bmp.GetPixel(0, height - 1);
                    var p = bmp.GetPixel(x, y);
                    if ((int)(this.x) + x < 0)
                        continue;

                    if ((int)(this.x) + x >= enemyImg.Width)
                        continue;

                    if ((int)(this.y) + y - scrollY < 0)
                        continue;

                    if ((int)(this.y) + y - scrollY >= enemyImg.Height)
                        continue;

                    var hp = enemyImg.GetPixel((int)(this.x) + x, (int)(this.y) + y - scrollY);
                    //var cc = new ColorConverter();
                    var np = Color.FromArgb(p.R * (fadeCount - 1) / fadeCount + hp.R / fadeCount, p.G * (fadeCount - 1) / fadeCount + hp.G / fadeCount, p.B * (fadeCount - 1) / fadeCount + hp.B / fadeCount);
                    
                    if (p == tp)
                        continue;

                    bmp.SetPixel(x, y, np);
                }
            }
            fadeCount--;

            if (fadeCount == 0)
                isDesappeared = true;

        }

        public Boolean Hit(double x, double y)
        {
            if (isAlive == false)
                return false;
            
            if (x < this.x)
                return false;
            
            if (x >= this.x + width)
                return false;
            
            if (y < this.y)
                return false;
            
            if (y >= this.y + height)
                return false;

            return true;
            
            //throw new NotImplementedException();
        }


        public void Move(Point gravitySource)
        {
            double gravity = 68.8;  // 중력값
            int dx = (int)x - gravitySource.X;
            int dy = (int)y - gravitySource.Y;
            // 삼각함수
            int distanceSquare = dx * dx + dy * dy;
            //double d = Math.Sqrt(dx * dx + dy * dy);

            if (isAlive)
            {
                if (distanceSquare == 0)
                    distanceSquare = 1;

                ivx += -dx * gravity / distanceSquare;
                ivy += -dy * gravity / distanceSquare;

            }
            else
            {
                ivx = 0;
                ivy = 1;
            }

            double d = 0.04; // 속도 계수

            x += ivx * d;
            y += ivy * d;
            //x += dx * d;
            //y += dy * d;
        }

        public void Die()
        {
            isAlive = false;

            fadeCount = 30;
        }

        public void Die(Bitmap Boom)
        {
            
            Graphics.FromImage(Boom);

            isAlive = false;

            fadeCount = 30;
        }

        public bool IsFadeOut(int screenWidth, int screenHeight)
        {
            if (x >= screenWidth - 310) return true;
            if (x < -20 - width / 2) return true;
            if (y >= screenHeight + 20) return true;
            if (y < -20 - height / 2) return true;

            return false;
        }
    }
}
