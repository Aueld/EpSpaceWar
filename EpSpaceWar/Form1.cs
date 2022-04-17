using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Media;

namespace EpShootingGame
{
   
    public partial class Form1 : GameManager
    {


        public void Start()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
           
            panelWidth = panel1.Width;
            panelHeight = panel1.Height;
            scrollY = -panelHeight;
            myBulletSpeed = 20f;
            playerHp = 3;
            enemyHp = 3;
            bossHp = 10000;

            player = new EpPlayer(320.0f, 600.0f, playerHp);
            bmp = new Bitmap(panelWidth, panelHeight);
            g = Graphics.FromImage(bmp);

            mainSound.Play();
                

        }

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Start();
        }

        private void Keydown(object sender, KeyEventArgs e)
        {
            if (gameOver && e.KeyCode == Keys.S)
                reStart();
            if (gameOver) return;

            if (e.KeyCode == Keys.Z)
                shooting = true;

            if (e.KeyCode == Keys.ShiftKey)
            {
                slowShot = true;
                player.AddDir(Dir.Slow);
                ag = -0.01f;
            }

            epKey.KeyDownState(e, player);
            e.Handled = true;
        }
        private void Keyup(object sender, KeyEventArgs e)
        {
            if (gameOver) return;

            if (e.KeyCode == Keys.Z)
                shooting = false;

            if (e.KeyCode == Keys.ShiftKey)
            {
                slowShot = false;
                player.RemoveDir(Dir.Slow);
                ag = 0.01f;
            }

            epKey.KeyUpState(e, player);
            e.Handled = true;
        }
        private void KeyPressing()
        {
            addFx = 30f;
            addFy = 60f;
            if (shooting)
            {
                shot.Add(new Shot(player.X, player.Y, 0.75f, myBulletSpeed));
                shot.Add(new Shot(player.X + addFx, player.Y + addFy, 0.75f + ag, myBulletSpeed));
                shot.Add(new Shot(player.X - addFx, player.Y + addFy, 0.75f - ag, myBulletSpeed));
                if (slowShot)
                {
                    shot.Add(new Shot(player.X + addFx * 2, player.Y + addFy * 1.5f, 0.75f + ag, myBulletSpeed));
                    shot.Add(new Shot(player.X - addFx * 2, player.Y + addFy * 1.5f, 0.75f - ag, myBulletSpeed));
                }
                else
                {
                    shot.Add(new Shot(player.X + addFx * 2, player.Y + addFy * 1.5f, 0.75f + ag, myBulletSpeed));
                    shot.Add(new Shot(player.X - addFx * 2, player.Y + addFy * 1.5f, 0.75f - ag, myBulletSpeed));
                }
            }
        }

        // 그림 처리
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (gameStart)
            {
                if (level == 0)
                {
                    if (transTimer > 100 && transTimer < 200)
                        e.Graphics.DrawString("이동 방향키", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y - 100);
                    if (transTimer > 200 && transTimer < 300)
                        e.Graphics.DrawString("탄발사 : Z", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y - 100);
                    if (transTimer > 300 && transTimer < 400)
                        e.Graphics.DrawString("슬로우 : SHIFT", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y - 100);
                    if (transTimer > 400 && transTimer < 500)
                        e.Graphics.DrawString("행운을 빕니다\n201770042 김재관", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y - 100);
                }
                if (player.IsAlive())
                {
                    player.Draw(e.Graphics);
                    foreach (var shot in shot.Where(x => 0 < x.Life)) shot.Draw(e.Graphics);
                    foreach (var bullet in bullets) bullet.Draw(e.Graphics);
                    foreach (var enemy in enemies) enemy.Draw(e.Graphics);
                    foreach (var fixEnemy in fixEnemies) fixEnemy.Draw(e.Graphics);
                }
                e.Graphics.DrawString("점수 : " + (score + hitCount * 100) + "\n생명 : " + player.Life + "\n처치 : " + killCount + "\n피격 : " + hitCount + "\n총알 : " + bullets.Count, font, Brushes.White, 730, 40);
                e.Graphics.DrawString("Time : " + timer + "\nLevel : " + (level + 1), DefaultFont, Brushes.Yellow, 600, 20);
                if (gameOver)
                {
                    if (!check)
                    {
                        MessageBox.Show("최종점수 : " + (score + hitCount * 100));
                        check = true;
                    }

                    e.Graphics.DrawString("아 주금", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y - 30);
                    e.Graphics.DrawString("S 키를 눌러 메인화면으로", DefaultFont, Brushes.White, player.X - player.width / 2, player.Y + 30);
                }
                e.Graphics.DrawImage(GUI, 84, 0);
               
            }
            else
            {
                e.Graphics.DrawImage(mainBack, -imageSize, -imageSize, 1000 + imageSize, 700 + imageSize);
                e.Graphics.DrawImage(buttonStart, startButton);
                e.Graphics.DrawImage(buttonExit, exitButton);
            }
        }

        // 타이머 동작
        private void gameEngine(object sender, EventArgs e)
        {
            if (gameStart)
            {
                if (gameStart && onePlay == 0)
                {
                    onePlay = 1;
                    stageSound1.Play();
                    
                }
                //else

                if (player.IsAlive())
                {

                    //timer = transTimer / 100;

                    player.Move();


                    foreach (var shot in shot.Where(x => 0 < x.Life))
                        shot.Move();

                    BulletDelete();

                    if (!bossStage)
                        EnemySpawn();

                    if (count < 2)
                        KeyPressing();

                    if (count > 4)
                        count = 0;
                    count++;




                    switch (level)
                    {
                        case 0:
                            enemySpawn_01stage();
                            break;

                        case 1:
                            enemySpawn_01stage();
                            break;


                        default:
                            enemySpawn_01stage();
                            break;
                    }

                    if (timer > 134)
                    {
                        level++;
                        timer = 0;
                        transTimer = 0;
                        stageSound2.Play();
                    }

                    Invalidate();
                }
                else
                {
                    gameOver = true;
                    Invalidate();
                    //timer1.Stop();
                }
            }
        }

        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            if (gameStart)
            {
                SetTime();
                transTimer++;
            }
            else
            {
                if (imageSize < 0)
                    loop = false;
                if (imageSize < 200 && !loop)
                    imageSize += 0.1f;
                else
                {
                    loop = true;
                    imageSize -= 0.1f;
                }
                Invalidate();
            }
        }
        private void SetTime()
        {
            if (DateTime.Now.Millisecond < 100 && !timeCheck)
                timeCheck = true;

            if (DateTime.Now.Millisecond > 950 && timeCheck)
            {
                timeCheck = false;
                timer++;
            }
        }

        private void eMouseDown(object sender, MouseEventArgs e)
        {
            //if (e.X > panelWidth / 2 - buttonStart.Width / 4 && e.X < panelWidth / 2 + buttonStart.Width / 4 && e.Y > 380 && e.Y < 380 + buttonStart.Height / 2)
            if(startButton.Contains(e.X, e.Y))
            { // e.Clicks.Equals(panelWidth / 2 - buttonStart.Width / 2, 380){

                MessageBox.Show("게임 시작");
                mainSound.Stop();
                gameStart = true;

                Invalidate();

            }
            if(exitButton.Contains(e.X, e.Y))
            { 
                MessageBox.Show("게임 종료");

                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
