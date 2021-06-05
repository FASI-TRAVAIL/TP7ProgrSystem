using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace DiningPhilosophers
{
    public partial class DiningPhilosophersForm : Form
    {
        private Graphics g;

        private const int NR = 5;
        private int philo = 0;
        private const int TABLERADIUS = 200;
        private const int PLATERADIUS = 40;
        private const int PHILRADIUS = 50;
        private const int STICKLENGTH = 75;

        private Philosopher[] phils = new Philosopher[NR];
        private Semaphore[] sticks = new Semaphore[NR];
        private Semaphore pickUpForks = new Semaphore(4,4);
        private Color[] colors = new Color[NR];

        public DiningPhilosophersForm()
        {
            InitializeComponent();

            for (int i = 0; i < NR; i++)
                sticks[i] = new Semaphore(1, 1);

            for (int i = 0; i < NR; i++)
            {
                philo = i + 1;
                phils[i] = new Philosopher(pickUpForks, sticks[i], sticks[(i + NR - 1) % NR], this);
                Thread t = new Thread(phils[i].Run);
                t.IsBackground = true;
                t.Start();
            }

            colors[0] = Color.Green;
            colors[1] = Color.Red;
            colors[2] = Color.Blue;
            colors[3] = Color.Yellow;
            colors[4] = Color.Magenta;
            //colors[5] = Color.Cyan;
            //colors[6] = Color.Pink;
        }

        private void DiningPhilosophersForm_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;
            // Gets a reference to the current BufferedGraphicsContext
            currentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with Form1, and with 
            // dimensions the same size as the drawing surface of Form1.
            myBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

            g = myBuffer.Graphics;
            DrawTable();
            DrawPhilosophers();
            DrawSticks();

            // Renders the contents of the buffer to the specified drawing surface.
            myBuffer.Render(e.Graphics);
        }

        private void DrawTable()
        {
            g.FillEllipse(new SolidBrush(Color.Bisque), 100, 100, 2 * TABLERADIUS, 2 * TABLERADIUS);

            g.FillEllipse(new SolidBrush(Color.White), 250, 250, 100, 100);

            for (int i = 0; i < NR; i++)
            {
                int cx = (int)(300 + 150 * Math.Sin(i * 2 * Math.PI / NR));
                int cy = (int)(300 + 150 * Math.Cos(i * 2 * Math.PI / NR));
                g.FillEllipse(new SolidBrush(Color.LightBlue), cx - PLATERADIUS, cy - PLATERADIUS, 2 * PLATERADIUS, 2 * PLATERADIUS);
            }
        }

        private void DrawPhilosophers()
        {
            for (int i = 0; i < NR; i++)
            {
                int cx = (int)(300 + 250 * Math.Sin(i * 2 * Math.PI / NR));
                int cy = (int)(300 + 250 * Math.Cos(i * 2 * Math.PI / NR));
                float MaxPercentage = (float)Math.Max(phils[i].PercentageTakenLeft, phils[i].PercentageTakenRight);
                float MinPercentage = (float)Math.Min(phils[i].PercentageTakenLeft, phils[i].PercentageTakenRight);



                // Creation du rectangle exterieur
                Pen pe = new Pen(colors[i]);
                Rectangle rect = new Rectangle(cx + 30 - PHILRADIUS, cy + 20 - PHILRADIUS, 60, 80);
                g.FillRectangle(new SolidBrush(Color.White), rect);

                
                // Ronds vide
                g.DrawEllipse(new Pen(Color.Gray), cx - PHILRADIUS, cy - PHILRADIUS, 2 * PHILRADIUS, 2 * PHILRADIUS);

                if (phils[i].State == Philosopher.state.Eating)
                {
                    if (MinPercentage >= 0.9)
                    {
                        g.FillEllipse(new SolidBrush(colors[i]), cx - PHILRADIUS, cy - PHILRADIUS, 2 * PHILRADIUS, 2 * PHILRADIUS);

                    }

                    else
                    {

                        g.DrawEllipse(new Pen(colors[i], 10), cx - MinPercentage * PHILRADIUS, cy - MinPercentage * PHILRADIUS, 2 * MinPercentage * PHILRADIUS, 2 * MinPercentage * PHILRADIUS);
                        g.DrawEllipse(new Pen(colors[i], 10), cx - MaxPercentage * PHILRADIUS, cy - MaxPercentage * PHILRADIUS, 2 * MaxPercentage * PHILRADIUS, 2 * MaxPercentage * PHILRADIUS);

                    }


                }
                else if (phils[i].State == Philosopher.state.Hungry)
                {

                    g.DrawEllipse(new Pen(colors[i], 10), cx - 0.2f * PHILRADIUS, cy - 0.2f * PHILRADIUS, 2 * 0.2f * PHILRADIUS, 2 * 0.2f * PHILRADIUS);
                }
                else if (phils[i].State != Philosopher.state.Thinking)
                {

                    g.DrawEllipse(new Pen(colors[i], 10), cx - MinPercentage * PHILRADIUS, cy - MinPercentage * PHILRADIUS, 2 * MinPercentage * PHILRADIUS, 2 * MinPercentage * PHILRADIUS);
                    g.DrawEllipse(new Pen(colors[i], 10), cx - MaxPercentage * PHILRADIUS, cy - MaxPercentage * PHILRADIUS, 2 * MaxPercentage * PHILRADIUS, 2 * MaxPercentage * PHILRADIUS);

                }

                // Affichage du texte dans les rectangles exterieurs
                Font font = new Font(this.Font, FontStyle.Bold);
                PointF p = new PointF(cx + 30 - PHILRADIUS, cy + 50 - PHILRADIUS);
                Brush b = new SolidBrush(Color.Black);
                g.DrawString("PHILO " + i, font, b, p);
            }
        }

        private void DrawSticks()
        {
            int x1, y1, x2, y2;
            int StickRadius, StickRadiusDelta;
            double Percentage;
            double StickAngle, StickAngleDelta;
            double StickAngleStart;
            int StickRadiusStart = 100;
            for (int i = 0; i < NR; i++)
            {
                StickAngleStart = Math.PI / NR + i * 2 * Math.PI / NR;
                if (phils[i].State == Philosopher.state.RightStickTaken || phils[i].State == Philosopher.state.Eating)
                {
                    StickRadiusDelta = 100;
                    StickAngleDelta = -Math.PI / (2 * NR);
                    Percentage = phils[i].PercentageTakenRight;
                }
                else if (phils[(i + 1) % NR].State == Philosopher.state.LeftStickTaken || phils[(i + 1) % NR].State == Philosopher.state.Eating)
                {
                    StickRadiusDelta = 100;
                    StickAngleDelta = Math.PI / (2 * NR);
                    Percentage = phils[(i + 1) % NR].PercentageTakenLeft;
                }
                else
                {
                    StickRadiusDelta = 0;
                    StickAngleDelta = 0;
                    Percentage = 0;
                }

                StickRadius = (int)(StickRadiusStart + Percentage * StickRadiusDelta);
                StickAngle = StickAngleStart + Percentage * StickAngleDelta;
                x1 = (int)(300 + StickRadius * Math.Sin(StickAngle));
                y1 = (int)(300 + StickRadius * Math.Cos(StickAngle));
                x2 = (int)(300 + (StickRadius + STICKLENGTH) * Math.Sin(StickAngle));
                y2 = (int)(300 + (StickRadius + STICKLENGTH) * Math.Cos(StickAngle));
                g.DrawLine(new Pen(Color.LightBlue, 10), x1, y1, x2, y2);


                // Creation du rectangle exterieur
                Pen pe = new Pen(colors[i]);
                Rectangle rect = new Rectangle(x2, y2, 60, 30);
                g.FillRectangle(new SolidBrush(Color.White), rect);

                // Affichage du texte dans les rectangles exterieurs john doit rester
                Font font = new Font(this.Font, FontStyle.Bold);
                PointF p = new PointF(x2, y2);
                Brush b = new SolidBrush(Color.Black);
                g.DrawString("STICK " + i, font, b, p);

            }
        }

        private void DiningPhilosophersForm_Load(object sender, EventArgs e)
        {

        }
    }
}