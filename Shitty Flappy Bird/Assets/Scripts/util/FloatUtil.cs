using UnityEngine;


namespace src.util
{
  internal static class FloatUtil
  {
    internal static bool EqualWithinErrorThreshold(float x, float y, float threshold) => Mathf.Abs(x - y) <= threshold;

    internal static bool EqualWithinErrorThreshold(float x, float y) => FloatUtil.EqualWithinErrorThreshold(x, y, 1);
  }
}
