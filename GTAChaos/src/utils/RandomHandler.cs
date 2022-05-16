// Copyright (c) 2019 Lordmau5
using System;

namespace GTAChaos.Utils
{
    public class RandomHandler
    {
        private static Random random = new();

        public static void SetSeed(string seed) => random = string.IsNullOrEmpty(seed) ? new Random() : new Random(seed.GetHashCode());

        public static int Next() => random.Next();

        public static int Next(int maxValue) => random.Next(maxValue);

        public static int Next(int minValue, int maxValue) => random.Next(minValue, maxValue + 1);

        public static double NextDouble() => random.NextDouble();

        public static void NextBytes(byte[] buffer) => random.NextBytes(buffer);
    }
}
