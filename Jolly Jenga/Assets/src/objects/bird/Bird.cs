#region

using UnityEngine;

#endregion


namespace src.objects.bird
{
  public sealed class Bird : MonoBehaviour
  {
    private GameObject? _bird;

    private const float StrengthOfJump = 250;

    private Rigidbody2D? _birdRb;

    public void Start()
    {
      this._bird   = GameObject.Find("Bird");
      this._birdRb = this._bird.GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
      if (!Input.GetKeyDown(KeyCode.Space) &&
          Input.touches.Length == 0)
      {
        return;
      }

      this._birdRb!.AddForce(new(0, Bird.StrengthOfJump));
    }

    public void FixedUpdate()
    {
      var    pos = this._birdRb!.position;
      Coords changedCoord;
      (pos, changedCoord) = Bird.LimitVector(pos, 3, Coords.Y);

      if (changedCoord is Coords.Y or Coords.Both)
      {
        this._birdRb.velocity = new(0, 0);
      }

      this._birdRb.position = pos;

      var closeObjects = Physics2D.OverlapCircleAll(pos, 5);

      foreach (var obj in closeObjects)
      {
        if (obj.name != "Obstacle")
        {
          continue;
        }

        var objPos = obj.GetComponent<Rigidbody2D>().position;

        if (!Mathf.Approximately(objPos.y, this._birdRb.position.y))
        {
          continue;
        }

        if (Mathf.Approximately(objPos.x, this._birdRb.position.x))
        {
          UI.UI.instance.GameOver();
        }

        UI.UI.instance.Score();
      }
    }

    private static (Vector2, Coords ) LimitVector(Vector2 vec, float limit, Coords coordinate)
    {
      var x = vec.x;
      var y = vec.y;

      return coordinate switch
             {
                     Coords.X => (new(x = Bird.RestrictToLimit(x, limit), y),
                                  Mathf.Approximately(x, vec.x) ? Coords.None : Coords.X),
                     Coords.Y => (new(x, y = Bird.RestrictToLimit(y, limit)),
                                  Mathf.Approximately(y, vec.y) ? Coords.None : Coords.Y),
                     Coords.Both => Bird.LimitVector(vec, limit),
                     var _       => (vec, Coords.None),
             };
    }

    private static (Vector2, Coords ) LimitVector(Vector2 vec, float limit)
    {
      var x = vec.x;
      var y = vec.y;

      Bird.RestrictToLimit(ref x, limit);

      Bird.RestrictToLimit(ref y, limit);

      var affectedCoords = (Mathf.Approximately(x, vec.x), Mathf.Approximately(y, vec.y)) switch
                           {
                                   (true, true)   => Coords.Both,
                                   (true, false)  => Coords.X,
                                   (false, true)  => Coords.Y,
                                   (false, false) => Coords.None,
                           };

      return (new(x, y), affectedCoords);
    }

    private static float RestrictToLimit(float x, float limit) => x < 0 ? Mathf.Max(x, -limit) : Mathf.Min(x, limit);

    private static void RestrictToLimit(ref float x, float limit) => x = Bird.RestrictToLimit(x, limit);

    private enum Coords { X, Y, Both, None, }
  }
}
