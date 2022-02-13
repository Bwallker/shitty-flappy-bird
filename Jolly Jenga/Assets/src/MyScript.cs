#nullable enable
public sealed class MyScript : UnityEngine.MonoBehaviour
{
  // Start is called before the first frame update

  private readonly System.Collections.Generic.HashSet<UnityEngine.GameObject> _obstacles;

  private readonly System.Collections.Generic.Dictionary<int, UnityEngine.Rigidbody2D> _rigidBodies;

  private System.Collections.Generic.HashSet<UnityEngine.GameObject>.Enumerator _obstaclesEnumerator;

  private readonly System.Random _random;

  private const int NumberOfObstacles = 50;

  private const ulong PeriodForObstacleSpawning = 10;

  private ulong _currentPeriod;

  public MyScript()
  {
    this._random      = new();
    this._obstacles   = new(MyScript.NumberOfObstacles);
    this._rigidBodies = new();
  }

  public void FixedUpdate()
  {
    if (this.NextPeriod() == 0)
    {
      this.NextMove();
    }
  }

  public void Start()
  {
    this.InitObstacles();
  }

  private void InitObstacles()
  {
    var parent = UnityEngine.GameObject.Find("Obstacle");
    this._obstacles.Add(parent);

    for (var _ = 0; _ < MyScript.NumberOfObstacles - 1; ++_)
    {
      var copy = UnityEngine.Object.Instantiate(parent);
      this._obstacles.Add(copy);
    }

    this._obstaclesEnumerator = this._obstacles.GetEnumerator();
  #if UNITY_EDITOR
    UnityEngine.Debug.Log(this._obstacles);
  #endif

    foreach (var obstacle in this._obstacles)
    {
      this._rigidBodies[obstacle.GetInstanceID()] = obstacle.GetComponent<UnityEngine.Rigidbody2D>();
      this.SetPos(obstacle, 100, 100);
      var rb = obstacle.GetComponent<UnityEngine.Rigidbody2D>()!;

      rb.AddForce(new(-10, 0), UnityEngine.ForceMode2D.Impulse);
    }
  }

  private bool ComesFromBottom() => this._random.Next(0, 2) == 0;

  private void NextMove()
  {
    var obstacle = this.NextObstacle();
    this.SetPos(obstacle, this.NextCoordinates());
  }

  private UnityEngine.Vector3 NextCoordinates()
  {
    const float xOffset = 6;
    const float yOffset = 3;

    if (this.ComesFromBottom())
    {
      return new(
                 15,
                 src.util.RandomGenerator.RandomFloat(-xOffset, -yOffset),
                 0
                );
    }

    return new(
               15,
               src.util.RandomGenerator.RandomFloat(xOffset, yOffset),
               0
              );
  }

  private UnityEngine.GameObject NextObstacle()
  {
    if (!this._obstaclesEnumerator.MoveNext())

    {
      this._obstaclesEnumerator = this._obstacles.GetEnumerator();
      this._obstaclesEnumerator.MoveNext();
    }

    var curr = this._obstaclesEnumerator.Current!;

    return curr;
  }

  private void SetPos(UnityEngine.Object o, float x, float y)
  {
    var rb = this._rigidBodies[o.GetInstanceID()]!;

    rb.MovePosition(new(x, y));
  }

  private void SetPos(
          UnityEngine.Object  o,
          UnityEngine.Vector3 coords
  )
  {
    this.SetPos(o, coords.x, coords.y);
  }

  private ulong NextPeriod()
  {
    var ret = this._currentPeriod;
    this._currentPeriod++;

    if (this._currentPeriod >= MyScript.PeriodForObstacleSpawning)
    {
      this._currentPeriod = 0;
    }

    return ret;
  }
}
