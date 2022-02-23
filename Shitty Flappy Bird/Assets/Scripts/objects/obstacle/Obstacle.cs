using components;

using UnityEngine;


namespace src
{
  public sealed class Obstacle : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D collider)
    {
      var obj = collider.gameObject;

      if (!Tags.HasTag(obj, "Bird"))
      {
        return;
      }

      UI.UI.Instance!.Score();
    }
  }
}
