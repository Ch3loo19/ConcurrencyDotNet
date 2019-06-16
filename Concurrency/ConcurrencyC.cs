using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Concurrency.F;

namespace Concurrency.C
{
    public static class ConcurrencyC
    {
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
            long  total = 0;
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
