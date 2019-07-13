using System;

namespace GTA_SA_Chaos.util
{
    public class RandomHandler
    {
        private static Random random = new Random();

        public static void SetSeed(string seed)
        {
            if (string.IsNullOrEmpty(seed))
            {
                random = new Random();
            }
            else
            {
                random = new Random(seed.GetHashCode());
            }
        }

        public static int Next()
        {
            return random.Next();
        }

        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static void NextBytes(byte[] buffer)
        {
            random.NextBytes(buffer);
        }
    }
}
