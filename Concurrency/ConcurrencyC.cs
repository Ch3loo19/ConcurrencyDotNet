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
                //Thread.Sleep(2000);
                tasks[iteration] = Task.Run(() => Console.WriteLine("{0} - {1}", Thread.CurrentThread.ManagedThreadId, iteration));
            }
            Task.WaitAll(tasks);
        }


        public static IDictionary<string, Person> GetMeADictionaryPlease()
        {
            var aDictionary = new Dictionary<string, Person>().ToImmutableDictionary();
            return aDictionary;
        }


        /// <summary>
        /// Parallel for with a shared, mutable variable, 'total' -> define a tlsValue that is used by each thread and aggregate at and in interlocked
        /// </summary>
        public static void DoSomething()
        {
            long total = 0;
            int len = 1000000;
            Parallel.For(0, len,
                () => 0,
                (int i, ParallelLoopState loopState, long tlsValue) =>
                {
                    return IsPrime(i) ? tlsValue += i : tlsValue;
                },
                value => Interlocked.Add(ref total, value));


        }

        private static bool IsPrime(int i)
        {
            return true;
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
