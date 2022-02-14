using src.util;

using System.Collections.Generic;

using UnityEngine;


public sealed class MyScript : MonoBehaviour
{
  private const int NumberOfObstacles = 100;

  private const ulong PeriodForObstacleSpawning = 1;

  private readonly List<GameObject> _obstacles;

  private readonly Dictionary<int, Rigidbody2D> _rigidBodies;

  private int _currentObstacleIndex;

  private ulong _currentPeriod;

  private readonly Vector2 _noVelocity = new(0, 0);

  public MyScript()
  {
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
      var rb = obstacle.GetComponent<Rigidbody2D>()!;

      this._rigidBodies[obstacle.GetInstanceID()] = rb;
      this.SetPos(obstacle, 100, 100);
    }
  }

  private void NextMove()
  {
    var obstacle = this.NextObstacle();
    this.SetPos(obstacle, MyScript.NextCoordinates());
    var rb = this.GetRigidBody(obstacle);
    rb.AddForce(new(-100, 0), ForceMode2D.Impulse);
  }

  private static Vector3 NextCoordinates()
  {
    const float xOffset = 6;
    const float yOffset = 3;

    if (RandomGenerator.RandomBool())
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
    var curr = this._obstacles[this._currentObstacleIndex];
    this._currentObstacleIndex++;

    if (this._currentObstacleIndex == this._obstacles.Count - 1)
    {
      this._currentObstacleIndex = 0;
    }

    return curr;
  }

  private void SetPos(Object o, float x, float y)
  {
    var rb = this.GetRigidBody(o);
    rb.MovePosition(new(x, y));
    this.ResetRigidBody(o);
  }

  private void SetPos(
          Object  o,
          Vector3 coords
  )
  {
    this.SetPos(o, coords.x, coords.y);
  }

  private void ResetRigidBody(Object o)
  {
    var rb = this.GetRigidBody(o);
    rb.rotation        = 0;
    rb.angularDrag     = 0;
    rb.angularVelocity = 0;
    rb.velocity        = this._noVelocity;
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

  private Rigidbody2D GetRigidBody(Object o) => this._rigidBodies[o.GetInstanceID()]!;
}
