using src.util;

using System.Collections.Generic;

using UnityEngine;

using Random = System.Random;


public sealed class MyScript : MonoBehaviour
{
  private const int NumberOfObstacles = 50;

  private const ulong PeriodForObstacleSpawning = 10;

  private readonly HashSet<GameObject> _obstacles;

  private readonly Random _random;

  private readonly Dictionary<int, Rigidbody2D> _rigidBodies;

  private ulong _currentPeriod;

  private HashSet<GameObject>.Enumerator? _obstaclesEnumerator;

  public MyScript()
  {
    this._random      = new();
    this._obstacles   = new(MyScript.NumberOfObstacles);
    this._rigidBodies = new();
  }

  public void Start()
  {
    this.InitObstacles();
  }

  public void FixedUpdate()
  {
    if (this.NextPeriod() == 0)
    {
      this.NextMove();
    }
  }

  private void InitObstacles()
  {
    var parent = GameObject.Find("Obstacle");
    this._obstacles.Add(parent);

    for (var _ = 0; _ < MyScript.NumberOfObstacles - 1; ++_)
    {
      var copy = Object.Instantiate(parent);
      this._obstacles.Add(copy);
    }

    foreach (var obstacle in this._obstacles)
    {
      this._rigidBodies[obstacle.GetInstanceID()] = obstacle.GetComponent<Rigidbody2D>()!;
      this.SetPos(obstacle, 100, 100);
      var rb = obstacle.GetComponent<Rigidbody2D>()!;

      rb.AddForce(new(-10, 0), ForceMode2D.Impulse);
    }

    this._obstaclesEnumerator = this._obstacles.GetEnumerator();
    Debug.Log("Logging enumerator contents");

    while (this._obstaclesEnumerator.Value.MoveNext())
    {
      Debug.Log(this._obstaclesEnumerator.Value.Current);
    }

    this._obstaclesEnumerator = this._obstacles.GetEnumerator();
  }

  private bool ComesFromBottom() => this._random.Next(0, 2) == 0;

  private void NextMove()
  {
    var obstacle = this.NextObstacle();
    this.SetPos(obstacle, this.NextCoordinates());
  }

  private Vector3 NextCoordinates()
  {
    const float xOffset = 6;
    const float yOffset = 3;

    if (this.ComesFromBottom())
    {
      return new(
                 15,
                 RandomGenerator.RandomFloat(-xOffset, -yOffset),
                 0
                );
    }

    return new(
               15,
               RandomGenerator.RandomFloat(xOffset, yOffset),
               0
              );
  }

  private GameObject NextObstacle()
  {
    Debug.Log(this._obstaclesEnumerator.HasValue);

    this._obstaclesEnumerator ??= this._obstacles.GetEnumerator();

    if (!this._obstaclesEnumerator.Value.MoveNext())
    {
      this._obstaclesEnumerator = this._obstacles.GetEnumerator();
      this._obstaclesEnumerator.Value.MoveNext();
    }

    var curr = this._obstaclesEnumerator.Value.Current;

    if (curr is null)
    {
      throw new("BIG BAD!!!!!!!!!!");
    }

    return curr;
  }

  private void SetPos(Object o, float x, float y)
  {
    var rb = this._rigidBodies[o.GetInstanceID()]!;
    rb.MovePosition(new(x, y));
  }

  private void SetPos(
          Object  o,
          Vector3 coords
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
