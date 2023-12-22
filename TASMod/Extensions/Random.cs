using System;
using System.Runtime.CompilerServices;


namespace TASMod.Extensions
{
    public static class Random_Extensions
    {
        // counts the number of times an RNG instance was called
        internal class Holder
        {
            public int Index = 0;
            public int Seed = 0;
        }
        internal static ConditionalWeakTable<Random, Holder> Counters = new();

        // allows copying of random objects
        public static Random Copy(this Random currRandom)
        {
            Random newRandom = new Random(0);
            // dupe the underlying data state
            Reflector.SetValue(newRandom, "_inextp", (int)Reflector.GetValue(currRandom, "_inextp"));
            Reflector.SetValue(newRandom, "_inext", (int)Reflector.GetValue(currRandom, "_inext"));
            Reflector.SetValue(newRandom, "_seedArray", ((int[])Reflector.GetValue(currRandom, "_seedArray")).Clone());

            // dupe the virtual properties
            newRandom.set_Index(currRandom.get_Index());
            newRandom.set_Seed(currRandom.get_Seed());

            return newRandom;
        }

        public static void CloneOver(this Random random, Random other)
        {
            Reflector.SetValue(other, "_inextp", (int)Reflector.GetValue(random, "_inextp"));
            Reflector.SetValue(other, "_inext", (int)Reflector.GetValue(random, "_inext"));
            Reflector.SetValue(other, "_seedArray", ((int[])Reflector.GetValue(random, "_seedArray")).Clone());
            other.set_Index(random.get_Index());
            other.set_Seed(random.get_Seed());
        }

        // allows peeking the next returned value from a random object
        public static int Peek(this Random random)
        {
            Random r = random.Copy();
            return r.Next();
        }

        // update the stored value for the counter
        public static void set_Index(this Random random, int index)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            counter.Index = index;
        }

        // 
        public static int get_Index(this Random random)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            return counter.Index;
        }

        public static int IncrementCounter(this Random random)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            return ++counter.Index;
        }
        public static int DecrementCounter(this Random random)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            return --counter.Index;
        }

        public static void set_Seed(this Random random, int seed)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            counter.Seed = seed;
        }

        // 
        public static int get_Seed(this Random random)
        {
            Holder counter = Counters.GetOrCreateValue(random);
            return counter.Seed;
        }
    }
}

