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
            //+ 1a) Danger: Shared Variable + Closure in C#. 
            //var task = new Task(() => ConcurrencyC.DangerSharedVariableClosure());
            //task.RunSynchronously();

            //Console.WriteLine($"{Environment.NewLine}*************************************{Environment.NewLine}");

            //+ 1b) Overcoming this problem in F#
            //var task = new Task(() => { _ = F_Library.DoSharedVariableClosureCorrectly; });
            //task.RunSynchronously();
            //! Although the code looks structurally the same, F# guarantees immutability.
            // So, in the case of variable closures, they cannot be mutated by subsequent iterations

            //+ 2) Danger: deadlocking
            //var deadlockAgent = new MyFirstDeadlock();
            //deadlockAgent.threadWriter1.Start();
            //deadlockAgent.threadWriter2.Start();

            //+ 3) Danger: Parallelising over shared variables 
            //double isPrimeSum;
            //var maxNum = 10000000;
            //Stopwatch sw = new Stopwatch();

            //sw.Start();

            //isPrimeSum = ConcurrencyC.SumFtw(maxNum);
            //Console.WriteLine($"Sequential sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            ////!Notice non - determinism when re-run and by comparing with sequential sum
            //isPrimeSum = ConcurrencyC.ParalleliseFtw(maxNum);
            //Console.WriteLine($"ParalleliseFtw sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            ////! Overcoming this problem with a global lock 
            //isPrimeSum = ConcurrencyC.ParalliliseWithGlobalLockOnSharedMutableVariable(maxNum);
            //Console.WriteLine($"ParalleliseWithGlobalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //sw.Restart();
            ////! Overcoming this problem with a local lock 
            //isPrimeSum = ConcurrencyC.ParalliliseWithSharedMutableVariableAndLocalLock(maxNum);
            //Console.WriteLine($"ParalliliseWithSharedMutableVariableAndLocalLock sum = {isPrimeSum}, calculated in {sw.ElapsedMilliseconds}");

            //+ 4) Dangers of Unknown variable sharing
            //! Even if sum is not done in parallel, the danger of using mutating data is still there if another part of the app is writing on that data
            //var data = Enumerable.Range(1, 1000).ToArray();
            //var sum = Task.Run(() => ConcurrencyC.NonThreadSafeSum(data));
            //var modifyData = Task.Run(() => ConcurrencyC.ModifyData(data));
            //Task.WaitAll(sum, modifyData);
            //Console.WriteLine("The sum of prime numbers is " + sum.Result);

            //! The reason this produces deterministic results is because the functional programming style uses a immutable copy of the original data
            //var data = Enumerable.Range(1, 1000).ToArray();
            //var sum = Task.Run(() => ConcurrencyC.ThreadSafeSum(data));
            //var modifyData = Task.Run(() => ConcurrencyC.ModifyData(data));
            //Task.WaitAll(sum, modifyData);
            //Console.WriteLine("The sum of prime numbers is " + sum.Result);

            //+ 5) Partitioning - parrallelisaiton speed-up
            //var data = Enumerable.Range(1, 9000000).Select(Convert.ToDouble).ToArray();
            //var sw = new Stopwatch();

            //sw.Start();
            //double sum = ConcurrencyC.PlinqSum(data);
            //Console.WriteLine($"Plinq without partitioner sum equals to {sum}, calck'd in {sw.ElapsedMilliseconds} miliseconds");

            //sw.Restart();
            //sum = ConcurrencyC.PlinqPartitionerSum(data);
            //Console.WriteLine($"Plinq with partitioner sum equals to {sum}, calck'd in {sw.ElapsedMilliseconds} miliseconds");

            //...
            //Partitioner.Create method, which takes as an argument the data source and a flag 
            //indicating which strategy to use, either dynamic or static. 
            //When the flag is true, the partitioner strategy is dynamic, and static otherwise.
            //Static partitioning often provides speedup on a multicore computer with a small number of cores(two or four).
            //Dynamic partitioning aims to load balance the work between tasks by assigning an arbitrary size of chunks 
            //and then incrementally expanding the length after each iteration.
            //It’s possible to build sophisticated partitioners(http://mng.bz/48UP) with complex strategies. 

            //+ 6) Task and Functional Composition - Printing whether number is prime - Using Task composition and F#
            //! This is an over-engineered algorithm which prints whether a number is prime.
            //ConcurrencyC.FunctionalTaskExample(1023943);

            Console.ReadLine();

        }
    }
}
