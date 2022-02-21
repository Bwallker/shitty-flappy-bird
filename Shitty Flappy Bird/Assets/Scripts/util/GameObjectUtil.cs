using UnityEngine;


namespace src.util
{
  internal static class GameObjectUtil
  {
    internal static void ToggleGameObjectEnabled(GameObject obj, bool shouldBeEnabled)
    {
      var scripts = obj.GetComponentsInChildren<MonoBehaviour>(true);

      foreach (var script in scripts)
      {
        if (script.name == "Tags")
        {
          script.enabled = true;

          continue;
        }

        script.enabled = shouldBeEnabled;
      }

      var rb = obj.GetComponent<Rigidbody2D>();

      if (rb == null || shouldBeEnabled)
      {
        return;
      }

      rb.velocity        = VectorUtil.ZeroVector;
      rb.angularDrag     = 0;
      rb.angularVelocity = 0;
      rb.gravityScale    = 0;
    }
  }
}
