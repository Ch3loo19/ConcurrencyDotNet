using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Concurrency.F;

namespace Concurrency.C
{
    public static class ConcurrencyC
    {

        public static void DangerSharedVariableClosure()
        {
            var tasks = new Task[10];
            for (int iteration = 0; iteration < 10; iteration++)
            {
                // Wrong solution to make it behave...
               //Thread.Sleep(1000);
                tasks[iteration] = Task.Run(() => Console.WriteLine("{0} - {1}", Thread.CurrentThread.ManagedThreadId, iteration));
            }
            Task.WaitAll(tasks);
        }


        public static IDictionary<string, Person> GetMeADictionaryPlease()
        {
            var aDictionary = new Dictionary<string, Person>().ToImmutableDictionary();
            return aDictionary;
        }


        public static double SumFtw(int len)
        {
            long total = 0;

            for (int i = 1; i < len; i++)
            {
                total = IsPrime(i) ? total + i : total;
            }

            return total;
        }

        public static double ParalleliseFtw(int len)
        {
            long total = 0;
            Parallel.For(1, len, i =>
             {
                 total += IsPrime(i) ?  i : 0;
             });
            return total;
        }

        public static double ParalliliseWithGlobalLockOnSharedMutableVariable(int len)
        {
            long total = 0;
            Parallel.For(1, len, i =>
            {
                Interlocked.Add(ref total, IsPrime(i) ? i : 0);
            });
            return total;

        }

        public static double ParalliliseWithSharedMutableVariableAndLocalLock(int len)
        {
            long total = 0;
            Parallel.For(1, len,
               () => 0,
               (int i, ParallelLoopState loopState, long tlsValue) => IsPrime(i) ? tlsValue + i : tlsValue,
               value => Interlocked.Add(ref total, value));
            return total;

        }

        private static bool IsPrime(double n)
        {
            if (n == 1) return false;
            if (n == 2) return true;
            var boundary = (int)Math.Floor(Math.Sqrt(n));
            for (int i = 2; i <= boundary; ++i)
                if (n % i == 0) return false;
            return true;
        }

        private static bool IsPrime(int n)
        {
            if (n == 1) return false;
            if (n == 2) return true;
            var boundary = (int)Math.Floor(Math.Sqrt(n));
            for (int i = 2; i <= boundary; ++i)
                if (n % i == 0) return false;
            return true;
        }

        public static double NonThreadSafeSum(int[] range)
        {
            double total = 0;
            int length = range.Count();

            for (int i = 0; i < length; i++)
            {
                total += IsPrime(range[i]) ? range[i] : 0;
            }

            return total;
        }

        public static double ThreadSafeSum(int[] range) => range.Where(o => IsPrime(o)).Sum();

        public static int[] ModifyData(int[] range)
        {
            int length = range.Count();
            for (int i = 0; i < length; i++)
            {
                range[i] = IsPrime(range[i]) ? range[i] : 2;
            }
            return range;

        }

        public static double PlinqSum(double[] data) => data.AsParallel().Where(IsPrime).Sum();

        public static double PlinqPartitionerSum(double[] data)
        {
            var partitioner = Partitioner.Create(data, true);
            return partitioner.AsParallel().Where(IsPrime).Sum();
        }


        public static Unit FunctionalTaskExample(int notionalVal)
        {
            var notionalTask = notionalVal.ReturnTask();
            var checkNumberIsPrimeFunc = new Func<int, Task<bool>>(o => Task.Run(() => IsPrime(o)));

            // here just to showcase
            var checkNumberIsPrime = notionalTask.Bind(checkNumberIsPrimeFunc);
            checkNumberIsPrime.Wait();

            var writeWhetherNumberIsPrimeFunc = new Func<bool, Task<Unit>>(o => Task.Run(() => PrintPrimalityOfNumber(o)));
            var printWhetherNumberIsPrime = checkNumberIsPrimeFunc.Compose(writeWhetherNumberIsPrimeFunc);

            var writeTask = printWhetherNumberIsPrime(notionalVal);
            writeTask.Wait();
            
            return writeTask.Result;
        }




        public static Task<T> ReturnTask<T>(this T val) => F_Library.Return(val);

        public static Task<R> Bind<T, R>(this Task<T> t, Func<T, Task<R>> k) => F_Library.Bind(t, k);

        public static Func<A, Task<C>> Compose<A, B, C>(this Func<A, Task<B>> f, Func<B, Task<C>> g) => F_Library.Compose(f, g);

        static Unit PrintPrimalityOfNumber(bool isPrime)
        {
            string message = isPrime ? "The number is Prime" : "The number ins't prime";
            Console.WriteLine(message);

            return Unit.Default;
        }


        static Task ForEachAsync<T>(this IEnumerable<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(maxDegreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }



    }

    public class MyFirstDeadlock
    {
        private object _lock1 = new object();
        private object _lock2 = new object();

        public Thread threadWriter1 => new Thread(() =>
        {
            lock (_lock1)
            {
                Thread.Sleep(1000);
                lock (_lock2)
                {
                    Console.WriteLine("This was thread1");
                }
            }
        });

        public Thread threadWriter2 => new Thread(() =>
        {
            lock (_lock2)
            {
                Thread.Sleep(1000);
                lock (_lock1)
                {
                    Console.WriteLine("This was thread2");
                }
            }
        });
    }

    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Default = new Unit();
        public override int GetHashCode() => 0;
        public override bool Equals(object obj) => obj is Unit;
        public override string ToString() => "()";

        public bool Equals(Unit other) => true;
        public static bool operator ==(Unit lhs, Unit rhs) => true;
        public static bool operator !=(Unit lhs, Unit rhs) => false;
    }


    //To showcase ImmutableInterlocked.TryAdd works - p.67

    // CAS (compare and swap) mechanism
    // Implementing atomisation; T needs to immutable. e.g. Immutable List or Dictionary
    // The compiler, the runtime system, and even hardware may rearrange reads and writes to memory locations for performance reasons. 
    // Fields that are declared volatile are not subject to these optimizations. 
    // Adding the volatile modifier ensures that all threads will observe volatile writes performed by any other thread in the order in which they were performed.
    public sealed class Atom<T> where T : class
    {
        public Atom(T value)
        {
            _value = value;
        }


        private volatile T _value;
        public T Value => _value;

        // This is a function that either returns the original T, or un updated, new T with new information (as T is immutable)
        // This is how ImmutableInterlocked.TryAdd works
        public T Swap(Func<T, T> factory)
        {
            T original, temp;
            do
            {
                original = _value;
                temp = factory(original);
            }
            // if the original is the same as temp, that means no new T was created by another thread, so you can safely return it
            // exmample when you try to update an Atom<ImmutableDictionary<TKey,Tval>>. p.66

            while (Interlocked.CompareExchange(ref _value, temp, original) != original);
            return original;

        }
    }

}
