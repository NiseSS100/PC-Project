using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_CP
{
    public enum Agents { Confidential, Secret, TopSecret };

    class Agent
    {
        public static Mutex mutex = new Mutex();

        TextWriter tw;

        public string agentName;

        public int brUsed;

        public Elevator elevator;
        public Floor button;

        Random rand = new Random();

        public Floor floor;

        ManualResetEvent eventAtHome = new ManualResetEvent(false);

        public Agents securityLevel;

        public AutoResetEvent signal = new AutoResetEvent(false);

        public Agent(TextWriter tw, Elevator elevator, string agentName, Agents securityLevel)
        {
            this.tw = tw;
            this.elevator = elevator;
            this.agentName = agentName;
            this.securityLevel = securityLevel;
            brUsed = rand.Next(5, 7);
        }

        public Floor GetRandomChoiceAction()
        {
            var availableButtons = elevator.availableFloors();
            int index = rand.Next(100) % 3;
            return availableButtons[index];
        }

        public void GoToWork()
        {
            tw.WriteLine(agentName + " goes to work.");
            floor = Floor.G;
        }


        public void EnterElevator()
        {
            tw.WriteLine(agentName + " waits in line for the elevator.");
            elevator.Add(this);
            signal.WaitOne();
            //Thread.Sleep(500);
            bool flag = true;
            while (flag)
            {
                Agent agent;
                if (floor == Floor.G)
                {
                    elevator.firstQueue.TryPeek(out agent);
                    if (agent.Equals(this))
                    {
                        flag = false;
                    }
                }else if (floor == Floor.S)
                {
                    elevator.secondQueue.TryPeek(out agent);
                    if (agent.Equals(this))
                    {
                        flag = false;
                    }
                }else if (floor == Floor.T1)
                {
                    elevator.thirdQueue.TryPeek(out agent);
                    if (agent.Equals(this))
                    {
                        flag = false;
                    }
                }else if (floor == Floor.T2)
                {
                    elevator.lastQueue.TryPeek(out agent);
                    if (agent.Equals(this))
                    {
                        flag = false;
                    }
                }
            }
            tw.WriteLine(agentName + " calls the elevator.");
            mutex.WaitOne();
            elevator.Moving(floor);
            elevator.ElevatorWork(this);
            tw.WriteLine(agentName + " entered the elevator.");
            elevator.Remove(floor);
            while (true)
            {
                button = GetRandomChoiceAction();
                switch (button)
                {
                    case Floor.G:
                        elevator.Moving(Floor.G);
                        tw.WriteLine(agentName + " is at ground floor.");
                        floor = Floor.G;
                        elevator.InElevator(Floor.G);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.S:
                        elevator.Moving(Floor.S);
                        tw.WriteLine(agentName + " is at secret floor with nuclear weapons.");
                        floor = Floor.S;
                        elevator.InElevator(Floor.S);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.T1:
                        elevator.Moving(Floor.T1);
                        tw.WriteLine(agentName + " is at secret floor with experimental weapons.");
                        floor = Floor.T1;
                        elevator.InElevator(Floor.T1);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.T2:
                        elevator.Moving(Floor.T2);
                        tw.WriteLine(agentName + " is at top-secret floor that stores alien remains.");
                        floor = Floor.T2;
                        elevator.InElevator(Floor.T2);
                        if (elevator.canExit) goto outside;
                        break;
                    default:
                        throw new ArgumentException(button + " action is not supported.");
                }
            }
        outside:
            return;
        }

        public void LeaveElevator()
        {
            // Simulate some work.
            //Thread.Sleep(500);

            tw.WriteLine(agentName + " is leaving the elevator.");
            floor = elevator.floor;
            
            // Release the Mutex.
            mutex.ReleaseMutex();
        }

        private void WorkingProcess()
        {
            GoToWork();
            while (brUsed!=0)
            {
                EnterElevator();
                LeaveElevator();
                //Thread.Sleep(500);
                brUsed--;
            }
            tw.WriteLine($"{agentName} has finished work.");
            eventAtHome.Set();
        }

        public void Working()
        {
            new Thread(WorkingProcess).Start();
        }

        public bool AtHome
        {
            get
            {
                return eventAtHome.WaitOne(0);
            }
        }
    }
}
