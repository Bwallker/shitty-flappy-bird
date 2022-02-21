using components;

using UnityEngine;


namespace src
{
  public sealed class ObstaclePart : MonoBehaviour
  {
    private void OnTriggerEnter(Collider other)
    {
      var obj = other.gameObject;

      if (!Tags.HasTag(obj, "Bird"))
      {
        return;
      }

      UI.UI.Instance!.GameOver();
    }
  }
}
