using System;
using System.Threading.Tasks;
using Concurrency.C;
using Concurrency.F;
using System.Diagnostics;
using System.Linq;
using System.Collections.Concurrent;

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

            //+ Parallelising over shared variables 
            //double isPrimeSum;
            //var maxNum = 10000000;
            //Stopwatch sw = new Stopwatch();

            //sw.Start();

            //isPrimeSum = ConcurrencyC.SumFtw(maxNum);
            //Console.WriteLine($"Sequential sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            //isPrimeSum = ConcurrencyC.ParalleliseFtw(maxNum);
            //Console.WriteLine($"ParalleliseFtw sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            //isPrimeSum = ConcurrencyC.ParalliliseWithGlobalLockOnSharedMutableVariable(maxNum);
            //Console.WriteLine($"ParalleliseWithGlobalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            //isPrimeSum = ConcurrencyC.ParalliliseWithSharedMutableVariableAndLocalLock(maxNum);
            //Console.WriteLine($"ParalliliseWithSharedMutableVariableAndLocalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //+ Dangers of Unknown variable sharing
            //var data = Enumerable.Range(1, 1000).ToArray();

            //var sum = Task.Run(() => ConcurrencyC.NonThreadSafeSum(data));
            //var modifyData = Task.Run(() => ConcurrencyC.ModifyData(data));
            //Task.WaitAll(sum, modifyData);
            //Console.WriteLine("The sum of prime numbers is " + sum.Result);

            //var data = Enumerable.Range(1, 1000).ToArray();

            //var sum = Task.Run(() => ConcurrencyC.ThreadSafeSum(data));
            //var modifyData = Task.Run(() => ConcurrencyC.ModifyData(data));
            //Task.WaitAll(sum, modifyData);
            //Console.WriteLine("The sum of prime numbers is " + sum.Result);

            //+ Partitioning
            //var data = Enumerable.Range(1, 2500000).Select(Convert.ToDouble).ToArray();
            //var sw = new Stopwatch();

            //sw.Start();
            //double sum = ConcurrencyC.PlinqSum(data);
            //Console.WriteLine($"Sum equals to {sum}, calck'd in {sw.ElapsedMilliseconds} miliseconds");

            //sw.Restart();
            //sum = ConcurrencyC.PlinqPartitionerSum(data);
            //Console.WriteLine($"Sum equals to {sum}, calck'd in {sw.ElapsedMilliseconds} miliseconds");

            //...
            //Partitioner.Create method, which takes as an argument the data source and a flag 
            //indicating which strategy to use, either dynamic or static. 
            //When the flag is true, the partitioner strategy is dynamic, and static otherwise.
            //Static partitioning often provides speedup on a multicore computer with a small number of cores(two or four).
            //Dynamic partitioning aims to load balance the work between tasks by assigning an arbitrary size of chunks 
            //and then incrementally expanding the length after each iteration.
            //It’s possible to build sophisticated partitioners(http://mng.bz/48UP) with complex strategies. 

            //+ Task and Functional Composition
            //ConcurrencyC.FunctionalTaskExample(1023941);
            
            Console.ReadLine();

        }
    }
}
