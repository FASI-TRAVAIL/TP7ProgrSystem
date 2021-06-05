using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DiningPhilosophers
{
    class Philosopher
    {
        private const int STEPS = 20;
        private int philoIndice;        
        private Semaphore rightStick;
        private Semaphore leftStick;
       

        private Form form;

        public Philosopher(int philo,Semaphore right, Semaphore left, Form form)
        {
            this.percentageTakenLeft = 0;
            this.percentageTakenRight = 0;
            this.philoIndice = philo;
            this.rightStick = right;
            this.leftStick = left;
            this.form = form;
        }

        public enum state { Thinking, Hungry, Eating, LeftStickTaken, RightStickTaken };
        private state philstate;
        public state State
        {
            get
            {
                return philstate;
            }
            set
            {
                philstate = value;
                form.Invalidate();
            }
        }

        private double percentageTakenLeft;
        public double PercentageTakenLeft
        {
            get
            {
                return percentageTakenLeft;
            }
            set
            {
                percentageTakenLeft = value;
                form.Invalidate();
            }
        }

        private double percentageTakenRight;
        public double PercentageTakenRight
        {
            get
            {
                return percentageTakenRight;
            }
            set
            {
                percentageTakenRight = value;
                form.Invalidate();
            }
        }

        public void Run()
        {
            while (true)
            {
                // Think for a few seconds
                State = state.Thinking;
                Thread.Sleep(2000);

                // Stop thinking, get hungry
                State = state.Hungry;
                if(philoIndice == DiningPhilosophersForm.NR)
                {
                    // Take right stick
                    rightStick.WaitOne();
                    State = state.RightStickTaken;
                    //
                    TakeRightStick();

                    // Take left stick
                    leftStick.WaitOne();
                    State = state.Eating;
                    //State = state.LeftStickTaken;
                    TakeLeftStick();

                }
                else
                {
                    // Take left stick
                    leftStick.WaitOne();
                    State = state.LeftStickTaken;
                    TakeLeftStick();

                    // Take right stick
                    rightStick.WaitOne();
                    State = state.Eating;
                    TakeRightStick();
                }
                

                // Eat for a few seconds
                Thread.Sleep(2000);

                // Return left stick
                ReturnLeftStick();
                leftStick.Release();

                // Return right stick
                ReturnRightStick();
                rightStick.Release();
            }
        }

        void TakeLeftStick()
        {
            for (int i = 0; i < STEPS; i++)
            {
                Thread.Sleep(100);
                PercentageTakenLeft += 1.0 / STEPS;
            }
        }

        void TakeRightStick()
        {
            for (int i = 0; i < STEPS; i++)
            {
                Thread.Sleep(100);
                PercentageTakenRight += 1.0 / STEPS;
            }
        }

        void ReturnLeftStick()
        {
            for (int i = 0; i < STEPS; i++)
            {
                Thread.Sleep(100);
                PercentageTakenLeft -= 1.0 / STEPS;
            }
        }

        void ReturnRightStick()
        {
            for (int i = 0; i < STEPS; i++)
            {
                Thread.Sleep(100);
                PercentageTakenRight -= 1.0 / STEPS;
            }
        }
    }
}
