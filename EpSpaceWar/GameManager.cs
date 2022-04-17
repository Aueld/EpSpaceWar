using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Media;

namespace EpShootingGame
{
    public class GameManager : Form
    {
        protected Graphics g;
        protected Bitmap bmp;
        protected static Bitmap enemyImg;// = Properties.Resources.Enemy;
        protected static EpPlayer player;
        protected static EpKeyState epKey;// = new EpKeyState();
        protected static List<EpEnemy> enemies;// = new List<EpEnemy>();
        protected static List<EpEnemy> fixEnemies;// = new List<EpEnemy>();
        protected static List<EpBullet> bullets;// = new List<EpBullet>();
        protected static List<Shot> shot;// = new List<Shot>();
        protected static SoundPlayer stageSound1;
        protected static SoundPlayer stageSound2;
        protected static SoundPlayer mainSound;
        protected static SoundPlayer enemyDie;

        protected static Bitmap background;
        protected static Bitmap buttonStart;
        protected static Bitmap buttonExit;
        protected static Bitmap mainBack;
        protected static Bitmap GUI;
        protected static Bitmap boom;

        protected static Rectangle startButton;
        protected static Rectangle exitButton;

        protected static Font font;

        protected bool gameOver = false;
        protected bool shooting = false;
        protected bool slowShot = false;
        protected bool bossStage = false;
        protected bool deadly = false;
        protected bool timeCheck = false;
        protected bool gameStart = false;
        protected bool loop = false;
        protected bool check = false;
        protected float ag { get; set; }
        protected float addFx { get; set; }
        protected float addFy { get; set; }
        protected float myBulletSpeed { get; set; }
        protected float imageSize { get; set; }

        protected int count { get; set; }
        protected int rotationPhase { get; set; }
        protected int panelWidth { get; set; }
        protected int panelHeight { get; set; }
        protected int scrollY { get; set; }
        protected int pixY { get; set; }
        protected int playerHp { get; set; }
        protected int enemyHp { get; set; }
        protected int bossHp { get; set; }
        protected long score { get; set; }
        protected int killCount { get; set; }
        protected int hitCount { get; set; }
        protected int timer { get; set; }
        protected int transTimer { get; set; }
        protected int level { get; set; }
        protected int onePlay { get; set; }
        
        protected static Random random = new Random();

        protected Point point = new Point();

        protected static EpBullet b;
        protected static EpEnemy enemy;
        protected static EpEnemy fixEnemy;

        private static Bitmap ShipBMP;

        static GameManager() // new 다 여기로 뺼것
        {
            enemyImg = Properties.Resources.Enemy;
            ShipBMP = new Bitmap(Properties.Resources.Enemy);
            epKey = new EpKeyState();
            enemies = new List<EpEnemy>();
            fixEnemies = new List<EpEnemy>();
            bullets = new List<EpBullet>();
            shot = new List<Shot>();

            stageSound1 = new SoundPlayer(Properties.Resources.Dube);
            stageSound2 = new SoundPlayer(Properties.Resources.rain);
            mainSound = new SoundPlayer(Properties.Resources.menu);
            enemyDie = new SoundPlayer(Properties.Resources.die);
            background = Properties.Resources.Backk;
            buttonStart = Properties.Resources.BStart;
            buttonExit = Properties.Resources.BEXIT;
            mainBack = Properties.Resources.spaceMainBack;
            GUI = Properties.Resources.GUI;
            boom = Properties.Resources.boom;

            font = new System.Drawing.Font("맑은 고딕", 18);

            startButton = new Rectangle(1000 / 2 - buttonStart.Width / 4, 380, 150, 50);
            exitButton = new Rectangle(1000 / 2 - buttonStart.Width / 4, 480, 150, 50);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected void BulletDelete()
        {
            //1000 x 750
            for (int i = 0; i < shot.Count; i++)
            {
                if (shot[i].X < -20 || shot[i].X > 680 || shot[i].Y < -20 || shot[i].Y > 750)
                    shot.RemoveAt(i);
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].X < -20 || bullets[i].X > 680 || bullets[i].Y < -20 || bullets[i].Y > 750)
                    bullets.RemoveAt(i);
            }
        }

        protected void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameManager
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "GameManager";
            this.Load += new System.EventHandler(this.GameManager_Load);
            this.ResumeLayout(false);

        }

        protected void ScoreLifePlus(int s)
        {
            if (score > s)
            {
                score -= s;

                if (player.Life < 3)
                    player.Life++;
                else
                    score += s;
            }
        }

        protected void EnemySpawn()
        {
            for (int i = 0; i < bullets.Count; i++)// var bullet in bullets)
            {
                //item.zanzou(this, g);
                //bullet.move(new Point(width / 2, height / 2));
                bullets[i].Move();
                bullets[i].Draw(g);
                int bx = (int)bullets[i].X + 1;
                int by = (int)bullets[i].Y + 1;
                if ((bx >= 0) && (bx < panelWidth) && (by >= 0) && (by < panelHeight))
                {
                    bmp.SetPixel(bx, by, Color.White);
                }

                //try
                //{
                //    bmp.SetPixel((int)(bullet.X), (int)(bullet.Y), Color.White);
                //}
                //catch (Exception)
                //{
                //    throw;
                //}
                // 피격시 설정
                switch (bullets[i].Side)
                {
                    case EpBullet.Sides.Enemy:
                        {
                            if (bullets[i].IsHit(player))
                            {
                                hitCount++;
                                bullets.RemoveAt(i);
                                b = null;
                                
                                player.Life--;
                                
                                break;
                                //Close();
                            }
                        }
                        break;

                    default:
                        break;
                }

            }
            foreach (var enemy in enemies)
            {
                for (int j = 0; j < shot.Count; j++)// shoter in shot)
                {
                    if (shot[j].IsHit(enemy))
                    {
                        score += 1;
                        enemy.hp--;

                        shot.RemoveAt(j);
                        if (enemy.hp < 0)
                        {
                            //enemy = null;
                            //enemyDie.Play();
                            enemy.Die();
                            score += 10;

                            ScoreLifePlus(400);

                            killCount++;
                        }
                    }
                }
            }
            
            {
                //foreach (var bullet in bullets)
                //{
                //    //item.zanzou(this, g);
                //    //bullet.move(new Point(width / 2, height / 2));
                //    bullet.Move();
                //    bullet.Draw(g);
                //    int bx = (int)bullet.X + 1;
                //    int by = (int)bullet.Y + 1;
                //    if ((bx >= 0) && (bx < panelWidth) && (by >= 0) && (by < panelHeight))
                //    {
                //        bmp.SetPixel(bx, by, Color.White);
                //    }
                //    //try
                //    //{
                //    //    bmp.SetPixel((int)(bullet.X), (int)(bullet.Y), Color.White);
                //    //}
                //    //catch (Exception)
                //    //{
                //    //    throw;
                //    //}
                //    // 피격시 설정
                //    switch (bullet.Side)
                //    {
                //        case EpBullet.Sides.Enemy:
                //            {
                //                if (bullet.IsHit(player))
                //                {
                //                    hitCount++;
                //                    score -= 100;
                //                    bullets.RemoveAt(bullets.IndexOf(bullet));
                //                    if (score < 0)
                //                    {
                //                        score = 0;
                //                        player.Life--;
                //                    }
                //                    break;
                //                    //Close();
                //                }
                //            }
                //            break;
                //        default:
                //            break;
                //    }
                //    foreach (var enemy in enemies)
                //    {
                //        for (int i = 0; i < shot.Count; i++)// shoter in shot)
                //        {
                //            if (shot[i].IsHit(enemy))
                //            {
                //                score += 1;
                //                enemy.hp--;
                //                shot.RemoveAt(i);
                //                if (enemy.hp < 0)
                //                {
                //                    enemy.Die();
                //                    score += 10;
                //                    killCount++;
                //                }
                //            }
                //        }
                //    }
                //}
            }
            // 랜덤 적 생성 및 설정
            if (random.Next(200) < 1 + (killCount / 40) + level * 10)
            {
                int ix = random.Next(panelWidth + 32) - 24;
                //Bitmap ShipBMP = new Bitmap(Properties.Resources.Enemy);

                enemy = new EpEnemy(ShipBMP, enemyHp, ix, -16, 0, 8);
                enemies.Add(enemy);
            }

            g.DrawImage(enemyImg, random.Next(3), scrollY);
            scrollY++;
            if (scrollY >= 0)
                scrollY = -panelHeight;



            //enemyBulletPatten();
            var e3 = enemies.FindAll(x => x.IsAlive && x.Y < panelHeight * 0.6);
            if (random.Next(400) < 2 + (killCount / 80) + level * 5 && e3.Count > 0)
            {
                bool isAliveEnemy = false;
                //EpEnemy enemy;
                do
                {
                    int index = random.Next(e3.Count);
                    enemy = e3[index];
                    isAliveEnemy = enemy.IsAlive;
                } while (isAliveEnemy == false);
                int x = (int)enemy.X + enemyImg.Width / 2;
                int y = (int)enemy.Y + enemyImg.Height / 2;

                int num = 24;
                //int num4 = 4;
                int num2 = 1;

                double constant = 1 + random.Next(5);

                // 탄 패턴
                if (random.Next(killCount / 5) >= 5)
                //if(true)
                {
                    num = random.Next(killCount / 10);
                    for (int i = 0; i < num; i++)
                    {
                        b = new EpBullet(EpBullet.MoveWay.Concentric, EpBullet.Sides.Enemy, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant, player.X + player.width / 2, player.Y + player.height / 2, constant);

                        bullets.Add(b);

                        b.Draw(g);
                    }
                }
                else if (random.Next((/*startedTime.Count*/ 100)) <= 2)
                {
                    for (int i = 0; i < num2; i++)
                    {
                        b = new EpBullet(EpBullet.MoveWay.Drill, EpBullet.Sides.Enemy, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant, player.X + player.width / 2, player.Y + player.height / 2, constant);
                        bullets.Add(b);

                        b.Draw(g);
                    }
                }
                else
                {
                    b = new EpBullet(EpBullet.MoveWay.Sighting, EpBullet.Sides.Enemy, x, y, 0, 0, player.X + player.width / 2, player.Y + player.height / 2, constant);
                    bullets.Add(b);
                    b.Draw(g);
                }
            }


            foreach (var enemy in enemies)
            {
                int gy = (int)(panelHeight / 2 + 40 * Math.Cos(2 * Math.PI * rotationPhase / 60));
                int gx = (int)(panelWidth / 2 + 100 * Math.Sin(-2 * Math.PI * rotationPhase / 60));


                rotationPhase++;
                if (enemy.IsAlive == false)
                {
                    enemy.Fade(enemyImg, scrollY);
                }
                enemy.Move(new Point(gx, gy));
                enemy.Draw(g);
            }
            enemies.RemoveAll(x => x.IsFadeOut(panelWidth + enemyImg.Width / 2, panelHeight + enemyImg.Height / 2));
            enemies.RemoveAll(x => x.IsDesappeared);


            // Enemy Down시 발동
            foreach(var enemy in enemies)
            {
                if (enemy.IsAlive == false)
                {
                    deadly = true;
                }
            }

        }

        private void enemyBulletPatten(int time)
        {
            var e2 = fixEnemies.FindAll(x => x.IsAlive && x.Y < panelHeight * 0.6);
            //if (random.Next(20) == 0 && e2.Count > 0)
            if(transTimer == time && e2.Count > 0)
            {
                bool isAliveEnemy = false;
                //EpEnemy enemy;
                do
                {
                    int index = random.Next(e2.Count);
                    fixEnemy = e2[index];
                    isAliveEnemy = fixEnemy.IsAlive;
                } while (isAliveEnemy == false);
                int x = (int)fixEnemy.X + enemyImg.Width / 2;
                int y = (int)fixEnemy.Y + enemyImg.Height / 2;

                int num = 24;

                double constant = 1 + random.Next(5);

                // 탄 패턴
                if (random.Next(killCount / 5) >= 4)
                //if(true)
                {
                    num = random.Next(killCount / 3 + 2);
                    for (int i = 0; i < num; i++)
                    {
                        b = new EpBullet(EpBullet.MoveWay.Concentric, EpBullet.Sides.Enemy, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant, player.X + player.width / 2, player.Y + player.height / 2, constant);

                        bullets.Add(b);

                        b.Draw(g);
                    }
                }
                else
                {
                    b = new EpBullet(EpBullet.MoveWay.Sighting, EpBullet.Sides.Enemy, x, y, 0, 0, player.X + player.width / 2, player.Y + player.height / 2, constant);
                    bullets.Add(b);
                    b.Draw(g);
                }
            }
        }

        private void enemyRandomBulletPatten()
        {
            var e2 = fixEnemies.FindAll(x => x.IsAlive && x.Y < panelHeight * 0.6);
            if (random.Next(100) == 0 && e2.Count > 0)
            {
                bool isAliveEnemy = false;
                //EpEnemy enemy;
                do
                {
                    int index = random.Next(e2.Count);
                    fixEnemy = e2[index];
                    isAliveEnemy = fixEnemy.IsAlive;
                } while (isAliveEnemy == false);
                int x = (int)fixEnemy.X + enemyImg.Width / 2;
                int y = (int)fixEnemy.Y + enemyImg.Height / 2;

                int num = 24;

                double constant = 1 + random.Next(5);

                // 탄 패턴
                if (random.Next(killCount / 5) >= 4)
                //if(true)
                {
                    num = random.Next(killCount / 3 + 2);
                    for (int i = 0; i < num; i++)
                    {
                        b = new EpBullet(EpBullet.MoveWay.Concentric, EpBullet.Sides.Enemy, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant, player.X + player.width / 2, player.Y + player.height / 2, constant);

                        bullets.Add(b);

                        b.Draw(g);
                    }
                }
                else
                {
                    b = new EpBullet(EpBullet.MoveWay.Sighting, EpBullet.Sides.Enemy, x, y, 0, 0, player.X + player.width / 2, player.Y + player.height / 2, constant);
                    bullets.Add(b);
                    b.Draw(g);
                }
            }
        }

        private void enemyDoubleShot(int time)
        {
            enemyBulletPatten(time);
            enemyBulletPatten(time);
        }

        protected void enemyTime()
        {
            if (!deadly)
                return;

            int x = (int)enemy.X + enemyImg.Width / 2;
            int y = (int)enemy.Y + enemyImg.Height / 2;
            int num = 16;
            double constant = 1 + random.Next(5);


            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive == false)
                {
                    for (int i = 0; i < num; i++)
                    {
                        b = new EpBullet(EpBullet.MoveWay.Concentric, EpBullet.Sides.Enemy, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant, player.X + player.width / 2, player.Y + player.height / 2, constant);

                        bullets.Add(b);


                    }
                }
            }
            deadly = false;

        }
        protected void enemySpawn_01stage()
        {
            enemyPattern();
            enemyRandomBulletPatten();

            //if (timer == 2) // 1stage boss
            //{
            //    fixEnemy = new EpEnemy(ShipBMP, enemyHp, 50, -16, 0, 8);
            //    fixEnemies.Add(fixEnemy);
            //}
            
            //탄 고정 발사
            {
                enemyDoubleShot(836);
                enemyDoubleShot(847);
                enemyDoubleShot(931);
                enemyDoubleShot(944);
                enemyDoubleShot(968);
                enemyDoubleShot(1031);
                enemyDoubleShot(1042);
                enemyDoubleShot(1055);
                enemyDoubleShot(1066);
                enemyDoubleShot(1081);
                enemyDoubleShot(1092);
                enemyDoubleShot(1165);
                enemyDoubleShot(1224);
                enemyDoubleShot(1237);
                enemyDoubleShot(1321);
                enemyDoubleShot(1334);
                enemyDoubleShot(1358);
                enemyDoubleShot(1421);
                enemyDoubleShot(1432);
                enemyDoubleShot(1445);
                enemyDoubleShot(1456);
                enemyDoubleShot(1469);
                enemyDoubleShot(1469);
                enemyDoubleShot(1480);
                enemyDoubleShot(1495);
                enemyDoubleShot(1616);
                enemyDoubleShot(1627);
                enemyDoubleShot(1687);
                enemyDoubleShot(1687);
                enemyDoubleShot(1700);
                enemyDoubleShot(1711);
                enemyDoubleShot(1711);
                enemyDoubleShot(1724);
                enemyDoubleShot(1737);
                enemyDoubleShot(1761);
                enemyDoubleShot(1809);
                enemyDoubleShot(1822);
                enemyDoubleShot(1822);
                enemyDoubleShot(1835);
                enemyDoubleShot(1846);
                enemyDoubleShot(1859);
                enemyDoubleShot(1869);
                enemyDoubleShot(1906);
                enemyDoubleShot(1919);
                enemyDoubleShot(1932);
                enemyDoubleShot(1932);
                enemyDoubleShot(1943);
                enemyDoubleShot(1956);
                enemyDoubleShot(2004);
                enemyDoubleShot(2017);
                enemyDoubleShot(2028);
                enemyDoubleShot(2064);
                enemyDoubleShot(2114);
                enemyDoubleShot(2127);
                enemyDoubleShot(2138);
                enemyDoubleShot(2151);
                enemyDoubleShot(2199);
                enemyDoubleShot(2212);
                enemyDoubleShot(2223);
                enemyDoubleShot(2236);
                enemyDoubleShot(2249);
                enemyDoubleShot(2260);
                enemyDoubleShot(2273);
                enemyDoubleShot(2356);
                enemyDoubleShot(2420);
                enemyDoubleShot(2457);
                enemyDoubleShot(2491);
                enemyDoubleShot(2504);
                enemyDoubleShot(2541);
                enemyDoubleShot(2615);
                enemyDoubleShot(2639);
                enemyDoubleShot(2663);
                enemyDoubleShot(2700);
                enemyDoubleShot(2737);
                enemyDoubleShot(2784);
                enemyDoubleShot(2810);
                enemyDoubleShot(2847);
                enemyDoubleShot(2882);
                enemyDoubleShot(2895);
                enemyDoubleShot(2906);
                enemyDoubleShot(2932);
                enemyDoubleShot(3005);
                enemyDoubleShot(3027);
                enemyDoubleShot(3053);
                enemyDoubleShot(3077);
                enemyDoubleShot(3101);
                enemyDoubleShot(3124);
                enemyDoubleShot(3174);
                enemyDoubleShot(3185);
                enemyDoubleShot(3198);
                enemyDoubleShot(3235);
                enemyDoubleShot(3235);
                enemyDoubleShot(3248);
                enemyDoubleShot(3259);
                enemyDoubleShot(3272);
                enemyDoubleShot(3272);
                enemyDoubleShot(3283);
                enemyDoubleShot(3306);
                enemyDoubleShot(3319);
                enemyDoubleShot(3430);
                enemyDoubleShot(3441);
                enemyDoubleShot(3467);
                enemyDoubleShot(3478);
                enemyDoubleShot(3501);
                enemyDoubleShot(3514);
                enemyDoubleShot(3564);
                enemyDoubleShot(3575);
                enemyDoubleShot(3588);
                enemyDoubleShot(3660);
                enemyDoubleShot(3673);
                enemyDoubleShot(3696);
                enemyDoubleShot(3709);
                enemyDoubleShot(3759);
                enemyDoubleShot(3770);
                enemyDoubleShot(3783);
                enemyDoubleShot(3794);
                enemyDoubleShot(3807);
                enemyDoubleShot(3820);
                enemyDoubleShot(3831);
                enemyDoubleShot(3855);
                enemyDoubleShot(3868);
                enemyDoubleShot(3881);
                enemyDoubleShot(3891);
                enemyDoubleShot(3909);
                enemyDoubleShot(3956);
                enemyDoubleShot(3982);
                enemyDoubleShot(4008);
                enemyDoubleShot(4019);
                enemyDoubleShot(4030);
                enemyDoubleShot(4043);
                enemyDoubleShot(4054);
                enemyDoubleShot(4067);
                enemyDoubleShot(4080);
                enemyDoubleShot(4091);
                enemyDoubleShot(4104);
                enemyDoubleShot(4151);
                enemyDoubleShot(4164);
                enemyDoubleShot(4175);
                enemyDoubleShot(4188);
                enemyDoubleShot(4201);
                enemyDoubleShot(4212);
                enemyDoubleShot(4225);
                enemyDoubleShot(4249);
                enemyDoubleShot(4262);
                enemyDoubleShot(4275);
                enemyDoubleShot(4286);
                enemyDoubleShot(4299);
                enemyDoubleShot(4346);
                enemyDoubleShot(4359);
                enemyDoubleShot(4370);
                enemyDoubleShot(4420);
                enemyDoubleShot(4433);
                enemyDoubleShot(4444);
                enemyDoubleShot(4457);
                enemyDoubleShot(4470);
                enemyDoubleShot(4481);
                enemyDoubleShot(4494);
                enemyDoubleShot(4541);
                enemyDoubleShot(4591);
                enemyDoubleShot(4602);
                enemyDoubleShot(4615);
                enemyDoubleShot(4639);
                enemyDoubleShot(4652);
                enemyDoubleShot(4676);
                enemyDoubleShot(4689);
                enemyDoubleShot(4762);
                enemyDoubleShot(4799);
                enemyDoubleShot(4834);
                enemyDoubleShot(4847);
                enemyDoubleShot(4884);
                enemyDoubleShot(4931);
                enemyDoubleShot(4931);
                enemyDoubleShot(4957);
                enemyDoubleShot(4981);
                enemyDoubleShot(5005);
                enemyDoubleShot(5042);
                enemyDoubleShot(5126);
                enemyDoubleShot(5152);
                enemyDoubleShot(5224);
                enemyDoubleShot(5237);
                enemyDoubleShot(5272);
                enemyDoubleShot(5345);
                enemyDoubleShot(5369);
                enemyDoubleShot(5395);
                enemyDoubleShot(5467);
                enemyDoubleShot(5516);
                enemyDoubleShot(5540);
                enemyDoubleShot(5612);
                enemyDoubleShot(5627);
                enemyDoubleShot(5662);
                enemyDoubleShot(5711);
                enemyDoubleShot(5735);
                enemyDoubleShot(5759);
                enemyDoubleShot(5783);
                enemyDoubleShot(5802);
                enemyDoubleShot(5857);
                enemyDoubleShot(5906);
                enemyDoubleShot(6002);
                enemyDoubleShot(6015);
                enemyDoubleShot(6052);
                enemyDoubleShot(6099);
                enemyDoubleShot(6125);
                enemyDoubleShot(6149);
                enemyDoubleShot(6175);
                enemyDoubleShot(6197);
                enemyDoubleShot(6147);
                enemyDoubleShot(6294);
                enemyDoubleShot(6307);
                enemyDoubleShot(6318);
                enemyDoubleShot(6344);
                enemyDoubleShot(6355);
                enemyDoubleShot(6368);
                enemyDoubleShot(6381);
                enemyDoubleShot(6392);
                enemyDoubleShot(6405);
                enemyDoubleShot(6429);
                enemyDoubleShot(6442);
                enemyDoubleShot(6489);
                enemyDoubleShot(6502);
                enemyDoubleShot(6513);
                enemyDoubleShot(6516);
                enemyDoubleShot(6539);
                enemyDoubleShot(6550);
                enemyDoubleShot(6563);
                enemyDoubleShot(6522);
                enemyDoubleShot(6600);
                enemyDoubleShot(6613);
                enemyDoubleShot(6624);
                enemyDoubleShot(6637);
                enemyDoubleShot(6684);
                enemyDoubleShot(6697);
                enemyDoubleShot(6708);
                enemyDoubleShot(6734);
                enemyDoubleShot(6734);
                enemyDoubleShot(6745);
                enemyDoubleShot(6758);
                enemyDoubleShot(6769);
                enemyDoubleShot(6782);
                enemyDoubleShot(6795);
                enemyDoubleShot(6819);
                enemyDoubleShot(6829);
                enemyDoubleShot(6879);
                enemyDoubleShot(6892);
                enemyDoubleShot(6903);
                enemyDoubleShot(6916);
                enemyDoubleShot(6916);
                enemyDoubleShot(6929);
                enemyDoubleShot(6940);
                enemyDoubleShot(6953);
                enemyDoubleShot(6977);
                enemyDoubleShot(6990);
                enemyDoubleShot(7014);
                enemyDoubleShot(7024);
                enemyDoubleShot(7074);
                enemyDoubleShot(7085);
                enemyDoubleShot(7098);
                enemyDoubleShot(7124);
                enemyDoubleShot(7137);
                enemyDoubleShot(7148);
                enemyDoubleShot(7161);
                enemyDoubleShot(7172);
                enemyDoubleShot(7183);
                enemyDoubleShot(7209);
                enemyDoubleShot(7209);
                enemyDoubleShot(7219);
                enemyDoubleShot(7219);
                enemyDoubleShot(7269);
                enemyDoubleShot(7280);
                enemyDoubleShot(7293);
                enemyDoubleShot(7304);
                enemyDoubleShot(7319);
                enemyDoubleShot(7330);
                enemyDoubleShot(7341);
                enemyDoubleShot(7365);
                enemyDoubleShot(7380);
                enemyDoubleShot(7401);
                enemyDoubleShot(7414);
                enemyDoubleShot(7464);
                enemyDoubleShot(7475);
                enemyDoubleShot(7488);
                enemyDoubleShot(7501);
                enemyDoubleShot(7525);
                enemyDoubleShot(7560);
                enemyDoubleShot(7573);
                enemyDoubleShot(7599);
                enemyDoubleShot(7609);
                enemyDoubleShot(7659);
                enemyDoubleShot(7670);
                enemyDoubleShot(7683);
                enemyDoubleShot(7694);
                enemyDoubleShot(7718);
                enemyDoubleShot(7731);
                enemyDoubleShot(7757);
                enemyDoubleShot(7770);
                enemyDoubleShot(7791);
                enemyDoubleShot(7809);
                enemyDoubleShot(7859);
                enemyDoubleShot(7869);
                enemyDoubleShot(7882);
                enemyDoubleShot(7919);
                enemyDoubleShot(7956);
                enemyDoubleShot(7967);
                enemyDoubleShot(7991);
                enemyDoubleShot(8004);
                enemyDoubleShot(8054);
                enemyDoubleShot(8064);
                enemyDoubleShot(8077);
                enemyDoubleShot(8090);
                enemyDoubleShot(8103);
                enemyDoubleShot(8125);
                enemyDoubleShot(8151);
                enemyDoubleShot(8164);
                enemyDoubleShot(8186);
                enemyDoubleShot(8199);
                enemyDoubleShot(8249);
                enemyDoubleShot(8259);
                enemyDoubleShot(8272);
                enemyDoubleShot(8309);
                enemyDoubleShot(8346);
                enemyDoubleShot(8357);
                enemyDoubleShot(8381);
                enemyDoubleShot(8394);
                enemyDoubleShot(8441);
                enemyDoubleShot(8441);
                enemyDoubleShot(8441);
                enemyDoubleShot(8441);

            }


            //적 고정 소환
            {
                spownEnemies(769, 50, -16, 0, 1);
                spownEnemies(769, Width - 400, -16, 0, 1);
                spownEnemies(835, 50, -16, 0, 1);
                spownEnemies(835, Width - 400, -16, 0, 1);
                spownEnemies(961, 50, -16, 0, 1);
                spownEnemies(961, Width - 400, -16, 0, 1);
                spownEnemies(1038, 50, -16, 0, 1);
                spownEnemies(1038, Width - 400, -16, 0, 1);
                spownEnemies(1153, 50, -16, 0, 1);
                spownEnemies(1038, Width - 400, -16, 0, 1);
                spownEnemies(1219, 50, -16, 0, 1);
                spownEnemies(1219, Width - 400, -16, 0, 1);
                spownEnemies(1345, 50, -16, 0, 1);
                spownEnemies(1345, Width - 400, -16, 0, 1);
                spownEnemies(1537, 50, -16, 0, 1);
                spownEnemies(1537, Width - 400, -16, 0, 1);
                spownEnemies(1603, 50, -16, 0, 1);
                spownEnemies(1603, Width - 400, -16, 0, 1);
                spownEnemies(1677, 50, -16, 0, 1);
                spownEnemies(1677, Width - 400, -16, 0, 1);
                spownEnemies(1800, 50, -16, 0, 1);
                spownEnemies(1800, Width - 400, -16, 0, 1);
                spownEnemies(1987, 50, -16, 0, 1);
                spownEnemies(1987, Width - 400, -16, 0, 1);
                spownEnemies(2061, 50, -16, 0, 1);
                spownEnemies(2061, Width - 400, -16, 0, 1);
                spownEnemies(2190, 50, -16, 0, 1);
                spownEnemies(2190, Width - 400, -16, 0, 1);




                spownEnemies(3073, 50, -16, 0, 1);
                spownEnemies(3139, 50, -16, 0, 1);
                spownEnemies(3265, 50, -16, 0, 1);
                spownEnemies(3457, 50, -16, 0, 1);
                spownEnemies(3523, 50, -16, 0, 1);
                spownEnemies(3649, 50, -16, 0, 1);
                spownEnemies(3720, 50, -16, 0, 1);
                spownEnemies(3842, 50, -16, 0, 1);
                spownEnemies(3913, 50, -16, 0, 1);
                spownEnemies(3981, 50, -16, 0, 1);
                spownEnemies(4107, 50, -16, 0, 1);
                spownEnemies(4226, 50, -16, 0, 1);
                spownEnemies(4299, 50, -16, 0, 1);
                spownEnemies(4418, 50, -16, 0, 1);
                spownEnemies(4489, 50, -16, 0, 1);
                spownEnemies(4610, 50, -16, 0, 1);
                spownEnemies(4684, 50, -16, 0, 1);
                spownEnemies(4802, 50, -16, 0, 1);
                spownEnemies(4873, 50, -16, 0, 1);
                spownEnemies(4994, 50, -16, 0, 1);
                spownEnemies(5068, 50, -16, 0, 1);
                spownEnemies(5185, 50, -16, 0, 1);
                spownEnemies(5312, 50, -16, 0, 1);
                spownEnemies(5443, 50, -16, 0, 1);
                spownEnemies(5569, 50, -16, 0, 1);
                spownEnemies(5646, 50, -16, 0, 1);
                spownEnemies(5761, 50, -16, 0, 1);
                spownEnemies(5835, 50, -16, 0, 1);
                spownEnemies(5953, 50, -16, 0, 1);
                spownEnemies(6080, 50, -16, 0, 1);
                spownEnemies(6145, 50, -16, 0, 1);
                spownEnemies(6216, 50, -16, 0, 1);
                spownEnemies(6337, 50, -16, 0, 1);
                spownEnemies(6406, 50, -16, 0, 1);
                spownEnemies(6529, 50, -16, 0, 1);
                spownEnemies(6600, 50, -16, 0, 1);
                spownEnemies(6721, 50, -16, 0, 1);
                spownEnemies(6913, 50, -16, 0, 1);
                spownEnemies(6984, 50, -16, 0, 1);
                spownEnemies(7105, 50, -16, 0, 1);
                spownEnemies(7176, 50, -16, 0, 1);
                spownEnemies(7297, 50, -16, 0, 1);
                spownEnemies(7371, 50, -16, 0, 1);
                spownEnemies(7489, 50, -16, 0, 1);
                spownEnemies(7557, 50, -16, 0, 1);
                spownEnemies(7682, 50, -16, 0, 1);
                spownEnemies(7748, 50, -16, 0, 1);
                spownEnemies(7874, 50, -16, 0, 1);
                spownEnemies(7943, 50, -16, 0, 1);
                spownEnemies(8066, 50, -16, 0, 1);
                spownEnemies(8140, 50, -16, 0, 1);

                spownEnemies(3073, Width - 400, -16, 0, 1);
                spownEnemies(3139, Width - 400, -16, 0, 1);
                spownEnemies(3265, Width - 400, -16, 0, 1);
                spownEnemies(3457, Width - 400, -16, 0, 1);
                spownEnemies(3523, Width - 400, -16, 0, 1);
                spownEnemies(3649, Width - 400, -16, 0, 1);
                spownEnemies(3720, Width - 400, -16, 0, 1);
                spownEnemies(3842, Width - 400, -16, 0, 1);
                spownEnemies(3913, Width - 400, -16, 0, 1);
                spownEnemies(3981, Width - 400, -16, 0, 1);
                spownEnemies(4107, Width - 400, -16, 0, 1);
                spownEnemies(4226, Width - 400, -16, 0, 1);
                spownEnemies(4299, Width - 400, -16, 0, 1);
                spownEnemies(4418, Width - 400, -16, 0, 1);
                spownEnemies(4489, Width - 400, -16, 0, 1);
                spownEnemies(4610, Width - 400, -16, 0, 1);
                spownEnemies(4684, Width - 400, -16, 0, 1);
                spownEnemies(4802, Width - 400, -16, 0, 1);
                spownEnemies(4873, Width - 400, -16, 0, 1);
                spownEnemies(4994, Width - 400, -16, 0, 1);
                spownEnemies(5068, Width - 400, -16, 0, 1);
                spownEnemies(5185, Width - 400, -16, 0, 1);
                spownEnemies(5312, Width - 400, -16, 0, 1);
                spownEnemies(5443, Width - 400, -16, 0, 1);
                spownEnemies(5569, Width - 400, -16, 0, 1);
                spownEnemies(5646, Width - 400, -16, 0, 1);
                spownEnemies(5761, Width - 400, -16, 0, 1);
                spownEnemies(5835, Width - 400, -16, 0, 1);
                spownEnemies(5953, Width - 400, -16, 0, 1);
                spownEnemies(6080, Width - 400, -16, 0, 1);
                spownEnemies(6145, Width - 400, -16, 0, 1);
                spownEnemies(6216, Width - 400, -16, 0, 1);
                spownEnemies(6337, Width - 400, -16, 0, 1);
                spownEnemies(6406, Width - 400, -16, 0, 1);
                spownEnemies(6529, Width - 400, -16, 0, 1);
                spownEnemies(6600, Width - 400, -16, 0, 1);
                spownEnemies(6721, Width - 400, -16, 0, 1);
                spownEnemies(6913, Width - 400, -16, 0, 1);
                spownEnemies(6984, Width - 400, -16, 0, 1);
                spownEnemies(7105, Width - 400, -16, 0, 1);
                spownEnemies(7176, Width - 400, -16, 0, 1);
                spownEnemies(7297, Width - 400, -16, 0, 1);
                spownEnemies(7371, Width - 400, -16, 0, 1);
                spownEnemies(7489, Width - 400, -16, 0, 1);
                spownEnemies(7557, Width - 400, -16, 0, 1);
                spownEnemies(7682, Width - 400, -16, 0, 1);
                spownEnemies(7748, Width - 400, -16, 0, 1);
                spownEnemies(7874, Width - 400, -16, 0, 1);
                spownEnemies(7943, Width - 400, -16, 0, 1);
                spownEnemies(8066, Width - 400, -16, 0, 1);
                spownEnemies(8140, Width - 400, -16, 0, 1);

            }

            g.DrawImage(enemyImg, random.Next(3), pixY);


            foreach (var fixEnemy in fixEnemies)
            {
                fixEnemy.Move(new Point(Width / 2 - 350, 100));
                fixEnemy.Draw(g);
            }

            //foreach (var fixEnemy in fixEnemies)
            //{
            //    if (fixEnemy.IsAlive == false)
            //    {
            //        deadly = true;
            //    }
            //}



            //pixY++;
            //if (pixY >= 100)
            //pixY -= 2;// = -panelHeight;

            //bool isAliveEnemy = false;

            //var e2 = enemies.FindAll(x => x.IsAlive && x.Y < panelHeight * 0.6);

            //do
            //{
            //    int index = random.Next(e2.Count);
            //    enemy = e2[index];
            //    isAliveEnemy = enemy.IsAlive;
            //} while (isAliveEnemy == false);
            //int x = (int)enemy.X + enemyImg.Width / 2;
            //int y = (int)enemy.Y + enemyImg.Height / 2;

        }
        protected void enemySpawn_02stage()
        {

        }
        private void spownEnemies(int time, int x, int y, int s, int p)
        {
            if (transTimer == time)
            {
                fixEnemy = new EpEnemy(ShipBMP, enemyHp, x, y, s, p);
                fixEnemies.Add(fixEnemy);
            }
        }

        private void enemyPattern()
        {

            for (int i = 0; i < bullets.Count; i++)// var bullet in bullets)
            {
                //item.zanzou(this, g);
                //bullet.move(new Point(width / 2, height / 2));
                bullets[i].Move();
                bullets[i].Draw(g);
                int bx = (int)bullets[i].X + 1;
                int by = (int)bullets[i].Y + 1;
                if ((bx >= 0) && (bx < panelWidth) && (by >= 0) && (by < panelHeight))
                {
                    bmp.SetPixel(bx, by, Color.White);
                }

                //try
                //{
                //    bmp.SetPixel((int)(bullet.X), (int)(bullet.Y), Color.White);
                //}
                //catch (Exception)
                //{
                //    throw;
                //}
                // 피격시 설정
                switch (bullets[i].Side)
                {
                    case EpBullet.Sides.Enemy:
                        {
                            if (bullets[i].IsHit(player))
                            {
                                hitCount++;
                                bullets.RemoveAt(i);
                                b = null;

                                player.Life--;

                                break;
                                //Close();
                            }
                            
                            //if (bullets[i].IsHit(player))
                            //{
                            //    hitCount++;
                            //    score -= 100;
                            //    bullets.RemoveAt(i);
                            //    b = null;
                            //    if (score < 0)
                            //    {
                            //        score = 0;
                            //        player.Life--;
                            //    }
                            //    break;
                            //    //Close();
                            //}
                        }
                        break;

                    default:
                        break;
                }





            }
            foreach (var fixEnemy in fixEnemies)
            {
                for (int j = 0; j < shot.Count; j++)// shoter in shot)
                {
                    if (shot[j].IsHit(fixEnemy))
                    {
                        //MessageBox.Show("z");
                        score += 1;

                        fixEnemy.hp--;

                        shot.RemoveAt(j);
                        if (fixEnemy.hp < 0)
                        {
                            //enemy = null;
                            //enemyDie.Play();
                            fixEnemy.Die();
                            score += 10;

                            ScoreLifePlus(400);

                            killCount++;
                        }
                    }
                }
            }
            foreach (var fixEnemy in fixEnemies)
            {
                int gy = (int)(panelHeight / 2 + 40 * Math.Cos(2 * Math.PI * rotationPhase / 60));
                int gx = (int)(panelWidth / 2 + 100 * Math.Sin(-2 * Math.PI * rotationPhase / 60));


                rotationPhase++;
                if (fixEnemy.IsAlive == false)
                {
                    fixEnemy.Fade(enemyImg, scrollY);
                }
                fixEnemy.Move(new Point(gx, gy));
                fixEnemy.Draw(g);
            }
            fixEnemies.RemoveAll(x => x.IsFadeOut(panelWidth + enemyImg.Width / 2, panelHeight + enemyImg.Height / 2));
            fixEnemies.RemoveAll(x => x.IsDesappeared);
        }
        
        protected void reStart()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            player = null;
            enemies = null;
            fixEnemies = null;
            shot = null;
            bullets = null;

            score = 0;
            killCount = 0;
            hitCount = 0;
            timer = 0;
            transTimer = 0;
            level = 0;
            onePlay = 0;
            playerHp = 3;

            enemies = new List<EpEnemy>();
            fixEnemies = new List<EpEnemy>();
            shot = new List<Shot>();
            bullets = new List<EpBullet>();
            player = new EpPlayer(320.0f, 600.0f, playerHp);

            gameStart = false;
            gameOver = false;
            check = false;
            shooting = false;
            slowShot = false;

            Invalidate();

            mainSound.Play();
        }
        private void GameManager_Load(object sender, EventArgs e)
        {

        }
    }
}
