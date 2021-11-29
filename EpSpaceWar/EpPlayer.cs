using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EpShootingGame
{

    public class EpPlayer : EpMover
    {
        
        private static Image image = Properties.Resources.Player;
        public int width = image.Width;
        public int height = image.Height;

        public EpPlayer(float x, float y, int life) : base(x, y, life)
        {
            Speed = 5.0f;
        }

        public void AddDir(Dir dir)
        {
            Dir |= dir;
        }

        public void RemoveDir(Dir dir)
        {
            Dir &= ~dir;
        }
        public void Move()
        {
            if ((Dir & Dir.Shot) != 0) { }
                
            if ((Dir & Dir.Slow) != 0)
                Speed = 2.0f;
            else
                Speed = 5.0f;
            if ((Dir & Dir.Left) != 0)
            {
                if (X < 0)
                    return;
                X -= Speed;
            }
            if ((Dir & Dir.Right) != 0)
            {
                if (X > 640)
                    return;
                X += Speed;
            }
            if ((Dir & Dir.Up) != 0)
            {
                if (Y < 0)
                    return;
                Y -= Speed;
            }
            if ((Dir & Dir.Down) != 0)
            {
                if (Y > 650)
                    return;
                Y += Speed;
            }
        }
        public void Draw(Graphics g)
        {
            g.DrawImage(image, X, Y);
        }

        public bool IsAlive()
        {
            return Life > 0;
        }

        public bool Hit(double x, double y)
        {
            // 플레이어 히트박스 생성 피격반정 6x6
            if (x < this.X + width / 2 - 3)
                return false;

            if (x >= this.X + width / 2 + 3)
                return false;

            if (y < this.Y + height / 2 + 3)
                return false;

            if (y >= this.Y + height / 2 + 9)
                return false;

            return true;
        }

        public bool Hit(double x, double y, int i)
        {
            // 플레이어 히트박스 생성 피격반정 6x6
            if (x < this.X + width / 2 - 3 - i * 2)
                return false;

            if (x >= this.X + width / 2 + 3 + i * 2)
                return false;

            if (y < this.Y + height / 2 + 3 - i * 2)
                return false;

            if (y >= this.Y + height / 2 + 9 + i * 2)
                return false;

            return true;
        }

    }

    public class Shot : EpMover
    {
        private static float PI = 3.1415926535898f;
        private static float PI2 = PI * 2f;
        private static Image image = Properties.Resources.bullet;

        private int ShotType;
        private float Angle;
        public Shot(float x, float y, float angle, float speed, int shot_type = 0) : base(x, y, 1)
        {
            Angle = angle;
            Speed = speed;
            ShotType = shot_type;
        }

        public void Move()
        {
            float rad = Angle * PI2;
            X += (float)Math.Cos(rad) * Speed;
            Y += (float)Math.Sin(rad) * Speed;
        }
        public void Draw(Graphics g)
        {
            double rad = (Angle - 0.75f) * PI2;
            g.DrawImage(image, X, Y);
        }

        public Boolean IsHit(EpEnemy e)
        {
            return e.Hit(X, Y);
            //throw new NotImplementedException();
        }

    }
}
