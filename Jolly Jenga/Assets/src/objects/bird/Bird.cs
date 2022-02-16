using System;

using UnityEngine;


namespace src.objects.bird
{
  public sealed class Bird : MonoBehaviour
  {
    public void FixedUpdate()
    {
      if (Input.GetKey(KeyCode.Space))
      {
        Debug.Log("SPACE!!!!!!!!!!!!!!!!");
      }
    }
  }
}
