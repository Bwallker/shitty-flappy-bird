using UnityEngine;


namespace src.util
{
  internal static class GameObjectUtil
  {
    internal static void ToggleGameObjectEnabled(GameObject obj, bool shouldBeEnabled)
    {
      var scripts = obj.GetComponentsInChildren<MonoBehaviour>();

      foreach (var script in scripts)
      {
        script.enabled = shouldBeEnabled;
      }

      var rb = obj.GetComponent<Rigidbody2D>();

      if (rb == null)
      {
        return;
      }

      rb.velocity        = new(0, 0);
      rb.angularDrag     = 0;
      rb.angularVelocity = 0;
      rb.gravityScale    = 0;
    }
  }
}
