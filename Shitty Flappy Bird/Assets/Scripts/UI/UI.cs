using components;

using src.util;

using TMPro;

using UnityEngine;


namespace src.UI
{
  public sealed class UI : MonoBehaviour
  {
    private TextMeshProUGUI? _score;

    private int _scoreCount;

    // Start is called before the first frame update

    public UI() => UI.Instance = this;

    internal static UI? Instance
    {
      get;

      private set;
    }

    private void Start()
    {
      this._score = Object.FindObjectOfType<Canvas>()!.GetComponentInChildren<TextMeshProUGUI>();
    }

    internal void GameOver()
    {
      this._score!.text = $"Game over! you scored: {this._scoreCount}\r\nPress space or tap on the screen to exit";
      var allObjects = Object.FindObjectsOfType<GameObject>();

      foreach (var obj in allObjects!)
      {
        var cond = Tags.HasTag(obj, "GameOverPersistent");
        GameObjectUtil.ToggleGameObjectEnabled(obj, cond);
      }
    }

    internal void Score()
    {
      this._score!.text = this._scoreCount++.ToString();
    }
  }
}
