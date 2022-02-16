using System;


namespace src.util
{
  internal static class RandomGenerator
  {
    private static readonly Random Random = new();

    /// Returns a random float between minValue (inclusive) and maxValue (exclusive)
    internal static float RandomFloat(float minValue, float maxValue) =>
            ((float) RandomGenerator.Random.NextDouble() * (maxValue - minValue)) + minValue;

    /// Returns a random bool based on the probability it is true
    /// <param name="probabilityOfBeingTrue">1.0 always returns true and 0.0 always returns false. 0.5 returns true half of the time.</param>
    internal static bool RandomBool(double probabilityOfBeingTrue) =>
            RandomGenerator.Random.NextDouble() < probabilityOfBeingTrue;

    /// Returns true half of the time.
    internal static bool RandomBool() => RandomGenerator.RandomBool(0.5);
  }
}
