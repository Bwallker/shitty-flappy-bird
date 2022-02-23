using System;

using components;

using UnityEngine;


namespace src
{
  public sealed class ObstaclePart : MonoBehaviour
  {
    private void OnCollisionEnter2D(Collision2D collision)
    {
      var obj = collision.gameObject!;

      if (!Tags.HasTag(obj, "Bird"))
      {
        return;
      }

      UI.UI.Instance!.GameOver();
    }
  }
}
