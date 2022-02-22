using System.Collections.Generic;

using src.util;

using UnityEngine;
using UnityEngine.Assertions;


namespace src
{
  public sealed class Obstacles : MonoBehaviour
  {
    private const int NumberOfObstacles = 50;

    private const int PeriodForObstacleSpawning = 20;

    private const float InitialSizeOfOpening = 3;

    private const float ShrinkPerTick = 0.001f;

    private const float SmallestOpeningSize = 2;

    [SerializeField]
    public PhysicsMaterial2D? material;

    private readonly List<GameObject> _obstacles;

    private readonly Dictionary<int, Rigidbody2D> _rigidBodies;

    private int _currentObstacleIndex;

    private ulong _currentPeriod;

    public Obstacles()
    {
      this._obstacles   = new(Obstacles.NumberOfObstacles);
      this._rigidBodies = new();
    }

    public void Start()
    {
      if (Obstacles.NumberOfObstacles == 0)
      {
        return;
      }

      this.InitObstacles();
    }

    public void FixedUpdate()
    {
      if (Obstacles.NumberOfObstacles == 0)
      {
        return;
      }

      if (this.NextPeriod() == 0)
      {
        this.NextMove();
      }
    }

    private void InitObstacles()
    {
      var parent            = GameObject.Find("Obstacle");
      var transformOfParent = parent!.GetComponent<Transform>();
      Assert.IsNotNull(transformOfParent);

      var bottom = transformOfParent!.Find("Bottom Part")!.gameObject;
      var top    = Object.Instantiate(bottom, transformOfParent, true);
      top!.name = "Top Part";

      var parentRb = parent.AddComponent<Rigidbody2D>();
      parentRb!.drag           = 0;
      parentRb.inertia         = 0;
      parentRb.useAutoMass     = true;
      parentRb.angularDrag     = 0;
      parentRb.velocity        = VectorUtil.ZeroVector;
      parentRb.angularVelocity = 0;
      parentRb.gravityScale    = 0;
      parentRb.sharedMaterial  = this.material;
      parentRb.constraints     = RigidbodyConstraints2D.FreezeRotation;

      var bottomBc   = bottom.GetComponent<BoxCollider2D>()!;
      var localScale = bottom.transform.localScale;

      var bc = parent.AddComponent<BoxCollider2D>();

      bc!.size = new(bottomBc.size.x, Obstacles.InitialSizeOfOpening);

      var offset = bc.offset;
      offset.x  = 0;
      offset.y  = (Obstacles.InitialSizeOfOpening * 0.5f) + (localScale.y * 0.5f);
      bc.offset = offset;

      var oldPos = bottom.transform.position;

      var newPos = new Vector3(
                               oldPos.x,
                               oldPos.y + Obstacles.InitialSizeOfOpening + localScale.y,
                               oldPos.z
                              );

      top.transform.position = newPos;
      this._obstacles.Add(parent);

      for (var _ = 0; _ < Obstacles.NumberOfObstacles - 1; ++_)
      {
        var copy = Object.Instantiate(parent);
        this._obstacles.Add(copy);
      }

      foreach (var obstacle in this._obstacles)
      {
        var rb = obstacle.GetComponent<Rigidbody2D>()!;

        this._rigidBodies[obstacle.GetInstanceID()] = rb;
        this.SetPos(obstacle, RandomGenerator.RandomFloat(100, 10000), 100);
      }
    }

    private void NextMove()
    {
      var obstacle = this.NextObstacle();
      this.SetPos(obstacle, Obstacles.NextCoordinates());
      var rb = this.GetRigidBody(obstacle);
      rb.AddForce(new(-5, 0), ForceMode2D.Impulse);
    }

    private static Vector3 NextCoordinates()
    {
      const float xOffset = 15;
      var         yOffset = RandomGenerator.RandomFloat(0, 5);

      if (RandomGenerator.RandomBool())
      {
        return new(
                   xOffset,
                   yOffset,
                   0
                  );
      }

      return new(
                 xOffset,
                 -yOffset,
                 0
                );
    }

    private GameObject NextObstacle()
    {
      var curr = this._obstacles[this._currentObstacleIndex];

      this._currentObstacleIndex = (this._currentObstacleIndex + 1) % this._obstacles.Count;

      return curr;
    }

    private void SetPos(Object o, float x, float y)
    {
      var rb = this.GetRigidBody(o);
      rb.position = new(x, y);
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
      rb.velocity        = VectorUtil.ZeroVector;
    }

    private ulong NextPeriod()
    {
      var ret = this._currentPeriod;

      this._currentPeriod = (this._currentPeriod + 1) % Obstacles.PeriodForObstacleSpawning;

      return ret;
    }

    private Rigidbody2D GetRigidBody(Object o) => this._rigidBodies[o.GetInstanceID()]!;
  }
}
