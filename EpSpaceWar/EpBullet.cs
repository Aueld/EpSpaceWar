using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EpShootingGame
{
    public class EpBullet : Obj
    {
        public enum Sides
        {
            Player, Enemy, Boss,
        }

        public enum MoveWay
        {
            Concentric, Sighting, Drill,
        }

        private double vx { get; set; }
        private double vy { get; set; }
        private double px { get; set; }
        private double py { get; set; }
        private double gx { get; set; }
        private double gy { get; set; }
        private double vgx { get; set; }
        private double vgy { get; set; }
        private Sides side { get; set; }
        public MoveWay moveWay { get; set; }
        public Sides Side
        {
            get
            {
                return side;
            }
        }

        private const int afterimage = 10;
        private int[] oldx = new int[afterimage];
        private int[] oldy = new int[afterimage];

        public Color color { get; set; }
        public int hitSize { get; set; }

        public EpBullet(Sides side, double x, double y, double vx, double vy)
        {
            this.side = side;
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
        }

        public EpBullet(MoveWay moveWay, Sides sides, int x_2, int y_2, double p, double p_2, double px, double py, double speed) : this(sides, x_2, y_2, p, p_2)
        {
            this.moveWay = moveWay;
            this.px = px;
            this.py = py;

            double finalSpeed = speed / Math.Sqrt((px - x) * (px - x) + (py - y) * (py - y));
            switch (moveWay)
            {
                case MoveWay.Concentric:
                    break;

                case MoveWay.Sighting:
                    vx = finalSpeed * (px - x);
                    vy = finalSpeed * (py - y);
                    break;

                case MoveWay.Drill:
                    vx = finalSpeed * (px - x);
                    vy = finalSpeed * (py - y);
                    gx = x;
                    gy = y;
                    vgx = vx;
                    vgy = vy;
                    x = x + 3 * vy;
                    y = y - 3 * vx;
                    break;

                default:
                    break;
            }
        }

        public void Draw(Graphics g)
        {
            Pen p;
            SolidBrush b;
            if (side == Sides.Enemy)
            {
                p = Pens.LightPink;
            }
            else
            {
                p = Pens.SkyBlue;
            }

            int tx;
            int ty;

            tx = (int)x;
            /* int */
            ty = (int)y;

            //AppearanceA(g, p, 5, tx, ty);

            if (moveWay == EpBullet.MoveWay.Drill)
            {
                Pen p0 = p;
                Color c0 = p0.Color;
                Color c1 = Color.Blue;
                for (int i = afterimage - 1; i >= 0; i--)
                {
                    hitSize = 1;

                    c0 = Color.FromArgb(c0.R * i / (i + 1), c0.G * i / (i + 1), c0.B * i / (i + 1));
                    c1 = Color.FromArgb(c1.R * i / (i + 1), c1.G * i / (i + 1), c1.B * i / (i + 1));

                    p = new Pen(c0);
                    b = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
                    tx = oldx[afterimage - 1 - i];
                    ty = oldy[afterimage - 1 - i];

                    g.DrawLine(p, (float)(tx + 1), (float)(ty), (float)(tx + 1), (float)(ty + 2));
                    g.DrawLine(p, (float)(tx), (float)(ty + 1), (float)(tx + 2), (float)(ty + 1));

                    g.DrawLine(p, (float)(tx + 0), (float)(ty), (float)(tx + 0), (float)(ty + 2));
                    g.DrawLine(p, (float)(tx + 2), (float)(ty), (float)(tx + 2), (float)(ty + 2));
                    g.DrawLine(p, (float)(tx + 1), (float)(ty - 1), (float)(tx + 1), (float)(ty + 3));
                    g.DrawLine(p, (float)(tx), (float)(ty + 0), (float)(tx + 2), (float)(ty + 0));
                    g.DrawLine(p, (float)(tx), (float)(ty + 2), (float)(tx + 2), (float)(ty + 2));
                    g.DrawLine(p, (float)(tx - 1), (float)(ty + 1), (float)(tx + 3), (float)(ty + 1));

                    Pen p2 = new Pen(c1);

                    g.DrawLine(p2, (float)(tx + 1), (float)(ty), (float)(tx + 1), (float)(ty + 2));
                    g.DrawLine(p2, (float)(tx), (float)(ty + 1), (float)(tx + 2), (float)(ty + 1));
                    
                    AppearanceB(g, p, b, hitSize, tx, ty);
                }
                p = p0;
            }
            else if (moveWay == EpBullet.MoveWay.Concentric)
            {
                hitSize = 1;
                p = Pens.Blue;
                g.DrawLine(p, (float)(tx + 1), (float)(ty), (float)(tx + 1), (float)(ty + 2));
                g.DrawLine(p, (float)(tx), (float)(ty + 1), (float)(tx + 2), (float)(ty + 1));

                AppearanceA(g, p, hitSize, tx, ty);

            }
            else if (moveWay == EpBullet.MoveWay.Sighting)
            {
                hitSize = 10;
                p = Pens.Blue;
                b = new SolidBrush(Color.FromArgb(255, 250, 250, 250));
                AppearanceB(g, p, b, hitSize, tx, ty);
 
                //g.DrawLine(p, (float)(tx + 5), (float)(ty), (float)(tx + 5), (float)(ty + 10));
                //g.DrawLine(p, (float)(tx), (float)(ty + 5), (float)(tx + 10), (float)(ty + 5));
            }
            /* int */




            //throw new NotImplementedException();

        }
        private void AppearanceA(Graphics g, Pen p, int i, int tx, int ty)
        {
            p = Pens.White;
            
            g.DrawLine(p, (float)(tx + i), (float)(ty), (float)(tx + i), (float)(ty + i * 2));
            g.DrawLine(p, (float)(tx), (float)(ty + i), (float)(tx + i * 2), (float)(ty + i));

            g.DrawLine(p, (float)(tx), (float)(ty), (float)(tx), (float)(ty + i * 2));
            g.DrawLine(p, (float)(tx + i * 2), (float)(ty), (float)(tx + i * 2), (float)(ty + i * 2));
            g.DrawLine(p, (float)(tx + i), (float)(ty - i), (float)(tx + i), (float)(ty + i * 3));
            g.DrawLine(p, (float)(tx), (float)(ty), (float)(tx + i * 2), (float)(ty));
            g.DrawLine(p, (float)(tx), (float)(ty + i * 2), (float)(tx + i * 2), (float)(ty + i * 2));
            g.DrawLine(p, (float)(tx - i), (float)(ty + i), (float)(tx + i * 3), (float)(ty + i));
            //p.RotateTransform(100);
        }

        private void AppearanceB(Graphics g, Pen p, SolidBrush b, int i, int tx, int ty)
        {
            g.FillEllipse(b, new Rectangle(tx - i, ty - i, i * 5, i * 5));
            b.Color = Color.FromArgb(255, 0, 0, 0);
            g.FillEllipse(b, new Rectangle(tx - i/2, ty - i/2, i * 4, i * 4));
            
            //g.DrawLine(p, (float)(tx + i), (float)(ty), (float)(tx + i), (float)(ty + i*2));
            //g.DrawLine(p, (float)(tx), (float)(ty + i), (float)(tx + i * 2), (float)(ty + i));
            
            //g.DrawLine(p, (float)(tx), (float)(ty), (float)(tx), (float)(ty + i * 2));
            //g.DrawLine(p, (float)(tx + i * 2), (float)(ty), (float)(tx + i * 2), (float)(ty + i * 2));
            //g.DrawLine(p, (float)(tx + i), (float)(ty - i), (float)(tx + i), (float)(ty + i * 3));
            //g.DrawLine(p, (float)(tx), (float)(ty), (float)(tx + i * 2), (float)(ty));
            //g.DrawLine(p, (float)(tx), (float)(ty + i * 2), (float)(tx + i * 2), (float)(ty + i * 2));
            //g.DrawLine(p, (float)(tx - i), (float)(ty + i), (float)(tx + i * 3), (float)(ty + i));
            //p.RotateTransform(100);
        }




        public void Move()
        {
            switch (moveWay)
            {
                case MoveWay.Concentric:
                    x += vx;
                    y += vy;
                    break;

                case MoveWay.Sighting:
                    x += vx;
                    y += vy;
                    break;

                case MoveWay.Drill:

                    double gravity = 48.8;

                    int distancex = (int)(x - gx);
                    int distancey = (int)(y - gy);

                    int distanceSquare = distancex * distancex + distancey * distancey;

                    if (distanceSquare == 0)
                        distanceSquare = 1;

                    double ivx;
                    ivx = -distancex * gravity / distanceSquare;
                    vx += ivx;
                    double ivy;
                    ivy = -distancey * gravity / distanceSquare;
                    vy += ivy;

                    int d = 1;

                    x += vx * d;
                    y += vy * d;

                    for (int i = afterimage - 1; i > 0; i--)
                    {
                        oldx[i] = oldx[i - 1];
                        oldy[i] = oldy[i - 1];
                    }
                    oldx[0] = (int)x;
                    oldy[0] = (int)y;

                    gx += vgx;
                    gy += vgy;

                    break;
                default:
                    break;
            }
        }

        public Boolean IsFadeOut(int screenWidth, int screenHeight)
        {
            if (x >= screenWidth) return true;
            if (x < 0) return true;
            if (y >= screenHeight) return true;
            if (y < 0) return true;
            return false;
        }

        public Boolean Hp(int hp)
        {
            if (hp == 0)
                return false;
            else
                return true;
        }

        public Boolean IsHit(EpPlayer p)
        {
            if (moveWay == EpBullet.MoveWay.Sighting)
                return p.Hit(x, y, hitSize);

            else
                return p.Hit(x, y);
            //throw new NotImplementedException();
        }
    }
}
