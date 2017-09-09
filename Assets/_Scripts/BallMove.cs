using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボールの移動
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BallMove : MonoBehaviour
{

    [SerializeField]
    private float power = 50;

    private Rigidbody2D rig;

    // Use this for initialization
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rig.velocity = new Vector2(x, y) * power;
    }
}
