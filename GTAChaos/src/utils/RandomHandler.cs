// Copyright (c) 2019 Lordmau5
using System;

namespace GTAChaos.Utils
{
    public class RandomHandler
    {
        public static Random Random = new();

        public static void SetSeed(string seed) => Random = string.IsNullOrEmpty(seed) ? new Random() : new Random(seed.GetHashCode());

        public static int Next() => Random.Next();

        public static int Next(int maxValue) => Random.Next(maxValue);

        public static int Next(int minValue, int maxValue) => Random.Next(minValue, maxValue + 1);

        public static double NextDouble() => Random.NextDouble();

        public static void NextBytes(byte[] buffer) => Random.NextBytes(buffer);
    }
}
