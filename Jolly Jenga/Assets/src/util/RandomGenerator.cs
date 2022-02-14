using System;


namespace src.util
{
  internal static class RandomGenerator
  {
    private static readonly Random Random = new();

    internal static float RandomFloat(float minValue, float maxValue) =>
            ((float) RandomGenerator.Random.NextDouble() * (maxValue - minValue)) + minValue;

    internal static bool RandomBool(double probabilityOfBeingTrue) =>
            RandomGenerator.Random.NextDouble() < probabilityOfBeingTrue;

    internal static bool RandomBool() => RandomGenerator.RandomBool(0.5);
  }
}
