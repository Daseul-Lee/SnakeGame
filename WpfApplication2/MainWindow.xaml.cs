using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading; // timer
using System.Diagnostics;
using System.Windows.Media.Animation;   // StopWatch

namespace snake
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : Window
    {
        Random r = new Random();

        Image Blood2=new Image();
        BitmapImage bitmap = new BitmapImage();
        private int Size = 50;      // Egg와 Body의 사이즈
        private int visibleCount = 5;
        private int SnakeSize = 101;
        private double x=0.1;
        Ellipse[] Snakes = new Ellipse[101];
        private string move = "";
        private string tmpmove = "";
        private int eaten = 0;
        private double score = 0;
        private int dead = 0;
        private Point tmp;
        private double TimeScore = 100;
        private double y;
        DispatcherTimer timer = new DispatcherTimer();
        DoubleAnimation Da = new DoubleAnimation();
        DoubleAnimation DaEnd = new DoubleAnimation();
        RotateTransform rt = new RotateTransform();
        RotateTransform rt2 = new RotateTransform();
        public Window1()
        {
            InitializeComponent();
            InitSnake();
            InitEgg();
            SnakeIMG.Opacity = 1;
            Egg.Opacity = 1;

            Da.From = 0;
            Da.To = 360;
            DaEnd.From = 0;
            DaEnd.To = 0;
            Da.RepeatBehavior = new RepeatBehavior(1);
 
            Da.Duration = new Duration(TimeSpan.FromSeconds(2));
            timer.Interval = new TimeSpan(0, 0, 0, 0, 150);
            timer.Tick += new EventHandler(timer_Tick);
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            isDeadSnakes();
            if (move != ""&&dead==0)
            {
                score = score + x;
                DrawSnakes();
                eatEgg();   // 알을 먹었는지 체크
                txtScore.Text = String.Format("{0:0.0}", (eaten * eaten + score * eaten * TimeScore/100));
                TimeScore = TimeScore * y;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (dead != 1 && (e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Up || e.Key == Key.Down))
                timer.IsEnabled = true;

            if (e.Key == Key.Right&&tmpmove!="Left")
                move = "Right";
            else if (e.Key == Key.Left && tmpmove != "Right")
                move = "Left";
            else if (e.Key == Key.Up&& tmpmove != "Down")
                move = "Up";
            else if (e.Key == Key.Down && tmpmove != "Up")
                move = "Down";
            else if (e.Key == Key.Escape)
            {
                timer.IsEnabled = false;
            }
            else if (e.Key == Key.R)
                BtnRe_Click(sender, e);
        }

        private void eatEgg()
        {
            Point pS = (Point)Snakes[0].Tag;
            Point pE = (Point)Egg.Tag;

            if (pS.X == pE.X && pS.Y == pE.Y)
            {
                visibleCount++;
                Snakes[visibleCount - 1].Visibility = Visibility.Visible;   // 꼬리를 하나 늘림
                EggScore.Text =(++eaten).ToString();

                if (visibleCount == SnakeSize/2)
                {
                    timer.IsEnabled = false;
                    MessageBox.Show("Clear!\nScore : " + txtScore.Text);
                    this.Close();
                    return;
                }

                InitEgg();
            }
        }

        private void isDeadSnakes()
        {
            Point ph = (Point)Snakes[0].Tag;
            tmp = ph;
            if (move == "Right")
            {
                ph = new Point(ph.X + Size, ph.Y);
                tmpmove = "Right";
            }
                
            else if (move == "Left")
            {
                ph = new Point(ph.X - Size, ph.Y);
                tmpmove = "Left";
            }
            else if (move == "Up")
            {
                ph = new Point(ph.X, ph.Y - Size);
                tmpmove = "Up";
            }
                
            else if (move == "Down")
            {
                ph = new Point(ph.X, ph.Y + Size);
                tmpmove = "Down";
            }

            if (ph.X < 0 || ph.X >= 500 || ph.Y < 0 || ph.Y >= 500)
            {
                dead = 1;
            }
                

            for (int i = 1; i < visibleCount; i++)
            {
                Point p = (Point)Snakes[i].Tag;
                if(ph.X == p.X && ph.Y == p.Y)
                {
                    dead = 1;
                }
            }

            if (dead == 1)
                DeadSnakes();
            tmp = ph;
        }

        private void DeadSnakes()
        {
            timer.IsEnabled = false;
            Canvas.SetLeft(Blood, tmp.X - 50);
            Canvas.SetTop(Blood, tmp.Y - 75);

            DoubleAnimation BloodShow = new DoubleAnimation();
            BloodShow.To = 1.0;
            BloodShow.From = 0.0;
            BloodShow.RepeatBehavior = new RepeatBehavior(1);
            BloodShow.BeginTime = TimeSpan.Parse("0:0:0.1");
            Blood.BeginAnimation(Image.OpacityProperty, BloodShow);
            MessageBox.Show("Dead!\nScore : " + txtScore.Text);
        }

        private void DrawSnakes()
        {
            
            for (int i = visibleCount; i > 0; i--)
            {
                Point p = (Point)Snakes[i-1].Tag;
                Snakes[i].Tag = new Point(p.X, p.Y);
                Canvas.SetLeft(Snakes[i], p.X);
                Canvas.SetTop(Snakes[i], p.Y);
            }
            Snakes[0].Tag = new Point(tmp.X, tmp.Y);
            Canvas.SetLeft(Snakes[0], tmp.X);
            Canvas.SetTop(Snakes[0], tmp.Y);
        }

        private void InitEgg()
        {
            Egg.Width = Size;
            Egg.Height = Size;

            Egg.Tag = new Point(r.Next(0, 500 / Size) * Size, r.Next(0, 500 / Size) * Size);
            Point p = (Point)Egg.Tag;
            for (int i = 0; i < visibleCount; i++)
            {
                Point ps = (Point)Snakes[i].Tag;
                if (ps.X == p.X && ps.Y == p.Y)
                {
                    InitEgg();
                    return;
                }
            }
            Canvas.SetLeft(Egg, p.X);
            Canvas.SetTop(Egg, p.Y);
        }

        private void InitSnake()
        {
            int x = r.Next(1, 500/ Size) * Size;
            int y = r.Next(1, (500 - 5 * Size) / Size) * Size;

            for (int i = SnakeSize-1; i >= 0; i--)
            {
                Snakes[i] = new Ellipse();
                Snakes[i].Width = Size;
                Snakes[i].Height = Size;
 
                if (i%2==1)
                    Snakes[i].Fill = Brushes.SkyBlue; // 2번째 마디마다 색깔 변경
                else
                    Snakes[i].Fill = Brushes.Blue;
                Snakes[i].Stroke = Brushes.Black;
                Canvas1.Children.Add(Snakes[i]);
            }

            for (int i = visibleCount; i < SnakeSize; i++)
            {
                Snakes[i].Visibility = Visibility.Hidden;
            }
            Snakes[0].Visibility = Visibility.Hidden;
            Snakes[0] = SnakeIMG;
            CreateSnake(x, y);
        }

        private void CreateSnake(int x, int y)
        {
            for (int i = 0; i < visibleCount; i++)
            {
                Snakes[i].Tag = new Point(x, y + i * Size);
                Canvas.SetLeft(Snakes[i], x);
                Canvas.SetTop(Snakes[i], y + i * Size);
            }
        }

        private void BtnRe_Click(object sender, RoutedEventArgs e)
        {
            int x = r.Next(1, 500 / Size) * Size;
            int y = r.Next(1, (500 - 5 * Size) / Size) * Size;
            CreateSnake(x, y);
            InitEgg();
            eaten = 0;
            EggScore.Text = "0";
            score = 0;
            
            move = "";
            visibleCount = 5;
            timer.IsEnabled = false;
            txtScore.Text = "0.0";
            TimeScore = 100;
            DoubleAnimation BloodShow = new DoubleAnimation();
            BloodShow.To = 0.0;
            BloodShow.From = Blood.Opacity;
            BloodShow.BeginTime = TimeSpan.Parse("0:0:0.1");
            BloodShow.RepeatBehavior = new RepeatBehavior(1);
            Blood.BeginAnimation(Image.OpacityProperty, BloodShow);
            
            dead = 0;

            for (int i = visibleCount; i < SnakeSize; i++)
            {
                Snakes[i].Visibility = Visibility.Hidden;
            }
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            if(dead==0)
                timer.IsEnabled = !timer.IsEnabled;
        }

        private void rbEeay_Checked(object sender, RoutedEventArgs e)
        {
            if(timer.IsEnabled==false)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                x = 0.05;
                y = 0.99;
            }
        }

        private void rbNormal_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled == false)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 150);
                x = 0.1;
                y = 0.995;
            }   
        }

        private void rbHard_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled == false)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                x = 0.15;
                y = 0.9955;
            }
                
        }

        private void rbHell_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled == false)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                x = 0.2;
                y = 0.99555;
            }
        }

        private void btnOnOffImg_MouseEnter(object sender, MouseEventArgs e)
        {/*
            if(!timer.IsEnabled)
            {
                btnOnOffImg.RenderTransform = rt;
                btnOnOffImg.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, Da);
            }
            */
        }

        private void btnReImg_MouseEnter(object sender, MouseEventArgs e)
        {
            /*if (!timer.IsEnabled)
            {
                btnReImg.RenderTransform = rt2;
                btnReImg.RenderTransformOrigin = new Point(0.5, 0.5);
                rt2.BeginAnimation(RotateTransform.AngleProperty, Da);
            }*/
        }

        private void btnOnOffImg_MouseLeave(object sender, MouseEventArgs e)
        {
            /*
            btnOnOffImg.RenderTransform = rt;
            btnOnOffImg.RenderTransformOrigin = new Point(0.5, 0.5);
            rt.BeginAnimation(RotateTransform.AngleProperty, DaEnd);*/
        }
        private void btnReImg_MouseLeave(object sender, MouseEventArgs e)
        {
            /*
             btnReImg.RenderTransform = rt2;
             btnReImg.RenderTransformOrigin = new Point(0.5, 0.5);
             rt2.BeginAnimation(RotateTransform.AngleProperty, DaEnd);*/
             
        }
    }
}
