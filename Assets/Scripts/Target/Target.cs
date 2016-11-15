using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

    public Vector2 velocity { get; set; }

    public void Start()
    {
        velocity = new Vector2(1, 1);
    }


    public float Speed()
    {
        return velocity.magnitude;
    }
}
