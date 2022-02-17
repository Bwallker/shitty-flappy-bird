using TMPro;

using UnityEngine;
using UnityEngine.UI;


namespace src.UI
{
  public sealed class UI : MonoBehaviour
  {
    private TextMeshProUGUI? _score;

    private int _scoreCount;

    public static UI instance { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
      this._score = Object.FindObjectOfType<Canvas>().GetComponentInChildren<TextMeshProUGUI>();
    }

    public void GameOver()
    {
      this._score!.text = $"Game over! you scored: {this._scoreCount}";
      Application.Quit();
    }

    public void Score()
    {
      this._score!.text = this._scoreCount++.ToString();
    }
  }
}
