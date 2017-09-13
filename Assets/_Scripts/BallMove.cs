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
        // キーボード操作
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // 加速度センサーによる操作
        x += Input.acceleration.x;
        y += Input.acceleration.y;
        var dir = new Vector2(x, y).normalized; // 斜め移動の速度が変わらないように正規化
        rig.velocity = dir * power;
    }
}
