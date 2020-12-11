using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_CP
{
    class Program
    {
        static void Main(string[] args)
        {

            using (StreamWriter tw = new StreamWriter(@"D:\Uni\Избираема\3 курс 1\Project\CP_Project\CP_Project\Text.txt"))
            {
                var syncWriter = StreamWriter.Synchronized(tw);                
                
                Elevator elev = new Elevator(syncWriter);
                List<Agent> agents = new List<Agent>() {
                    new Agent(syncWriter, elev,"SEO",Agents.TopSecret),
                    new Agent(syncWriter, elev, "Manager", Agents.Secret),
                    new Agent(syncWriter, elev, "Intern", Agents.Confidential)          
                    };
                
                foreach (var agent in agents)
                {
                    agent.Working();
                }

                while (agents.Any(s => !s.AtHome))
                {

                }
                syncWriter.WriteLine("Work day is over.");
            }
        }
    }
}
