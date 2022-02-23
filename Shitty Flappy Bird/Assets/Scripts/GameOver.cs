using src.util;

using UnityEngine;
using UnityEngine.SceneManagement;


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
          Input.touches!.Length == 0)
      {
        return;
      }

      SceneManager.LoadScene("MainMenuScene");
    }
  }
}
