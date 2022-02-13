#nullable enable
namespace src.util
{
  internal static class RandomGenerator
  {
    private static readonly System.Random Random = new();

    internal static float RandomFloat(float minValue, float maxValue) =>
            ((float) RandomGenerator.Random.NextDouble() * (maxValue - minValue)) + minValue;
  }
}
