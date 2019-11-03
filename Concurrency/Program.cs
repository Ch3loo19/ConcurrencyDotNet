using System;
using System.Threading.Tasks;
using Concurrency.C;
using Concurrency.F;
using System.Diagnostics;

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

            //+ Danger deadlocking
            //var deadlockAgent = new MyFirstDeadlock();
            //deadlockAgent.threadWriter1.Start();
            //deadlockAgent.threadWriter2.Start();

            //+ Parallelising over shared variables - Also PLINQ
            //double isPrimeSum;
            //Stopwatch sw = new Stopwatch();

            //sw.Start();
            //isPrimeSum = ConcurrencyC.SumFtw(30000000);
            //Console.WriteLine($"Sequential sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            //isPrimeSum = ConcurrencyC.ParalleliseFtw(30000000);
            //Console.WriteLine($"ParalleliseFtw sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            ////sw.Restart();
            ////isPrimeSum = ConcurrencyC.ParalliliseWithGlobalLockOnSharedMutableVariable(30000000);
            ////Console.WriteLine($"ParalleliseWithGlobalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            ////sw.Restart();
            ////isPrimeSum = ConcurrencyC.ParalliliseWithSharedMutableVariableAndLocalLock(30000000);
            ////Console.WriteLine($"ParalliliseWithSharedMutableVariableAndLocalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //+ Dangers of Unknown 
            var data = Enumerable.Range(1.10000000);
            var sum = Task.Run(() => Sum(data));
            var modifyData = Task.Run(() => modifyData(data));
            Task.WaitAll(sum, modifyData);
            Console.WriteLine(sum);



            Console.ReadLine();

        }
    }
}
