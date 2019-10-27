using System;
using System.Threading.Tasks;
using Concurrency.C;
using Concurrency.F;

namespace Concurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            //+ Danger Shared Variable + Closure
            //var task = new Task(() => ConcurrencyC.DangerSharedVariableClosure());
            //task.RunSynchronously();

            //Console.WriteLine($"{Environment.NewLine}*************************************{Environment.NewLine}");

            //// Overcoming this in F#
            //task = new Task(() => { _ = F_Library.DoSharedVariableClosureCorrectly; });
            //task.RunSynchronously();

            // +Danger deadlocking
            //var deadlockAgent = new MyFirstDeadlock();
            //deadlockAgent.threadWriter1.Start();
            //deadlockAgent.threadWriter2.Start();



            Console.ReadLine();

        }
    }
}
