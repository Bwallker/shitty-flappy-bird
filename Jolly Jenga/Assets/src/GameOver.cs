using src.util;

using UnityEngine;


namespace src
{
  public sealed class GameOver : MonoBehaviour
  {
    public void Start()
    {
      GameObjectUtil.ToggleGameObjectEnabled(this.gameObject, false);
    }

    public void Update()
    {
      if (!Input.GetKey(KeyCode.Space) &&
          Input.touches.Length == 0)
      {
        return;
      }

      Debug.Log("Made it here!");
      Application.Quit();
    }
  }
}
