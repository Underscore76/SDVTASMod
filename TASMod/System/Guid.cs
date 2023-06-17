using System;

namespace TASMod.System
{
	public class TASGuid
	{

        public static int GuidSeed = 0;
        private static Random random = new Random(GuidSeed);

        public static void Reset()
        {
            random = new Random(GuidSeed);
        }

        public static Guid NewGuid()
        {
            // Every time a guid gets created (NetCollections adds, object sorting in some contexts), 
            // it's uncontrollable random as that call uses a system function for generating guids
            // best solution I can come up with is to generate 
            // This is reproducible, and doesn't impact normal RNG manip that a person might do.
            Guid ret = Guid.Parse(string.Format("{0:X4}{1:X4}-{2:X4}-{3:X4}-{4:X4}-{5:X4}{6:X4}{7:X4}",
                random.Next(0, 0xffff), random.Next(0, 0xffff),
                random.Next(0, 0xffff),
                random.Next(0, 0xffff) | 0x4000,
                random.Next(0, 0x3fff) | 0x8000,
                random.Next(0, 0xffff), random.Next(0, 0xffff), random.Next(0, 0xffff))
                );
            return ret;
        }
    }
}

