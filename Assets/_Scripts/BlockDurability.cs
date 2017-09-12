﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブロックの耐久値がなくなるとボールにくっつく
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(FixedJoint2D))]
public class BlockDurability : MonoBehaviour
{

    [SerializeField]
    private int durability = 10;

    [SerializeField]
    private Color hitColor = Color.red;

    [SerializeField]
    private float shockInterval = 0.1f;

    [SerializeField]
    private int ballFeed = 1;

    private Rigidbody2D rig;
    private Color initialColor;
    private Renderer texture;
    private FixedJoint2D joint;
    private BallMass ballMass;

    public int BallFeed
    {
        get
        {
            return ballFeed;
        }
        set
        {
            ballFeed = value;
        }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.constraints = RigidbodyConstraints2D.FreezePosition;
        rig.Sleep();
        texture = GetComponent<Renderer>();
        joint = GetComponent<FixedJoint2D>();
        initialColor = texture.material.color;
        GameObject ball = GameObject.FindWithTag("Ball");
        ballMass = ball.GetComponent<BallMass>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball"
            || collision.gameObject.tag == "BallSatellites")
        {

            if (durability > 0)
            {
                durability -= ballMass.CurrentFeed;
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockBreak);
                StartCoroutine(ChangeColor());
            }
            else if (durability <= 0 && joint.connectedBody == null)
            {
                // ボールにくっつく
                joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                rig.constraints = RigidbodyConstraints2D.None;
                gameObject.tag = "BallSatellites";
                ballMass.Feed(ballFeed, gameObject);
                // ボールの自体の重さも増やしてバランスを取れるようにする
                ballMass.AddMass(rig.mass);
                // TODO:くっつくとき用のSEに差し替える
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockBreak);
            }
        }

    }

    private IEnumerator ChangeColor()
    {
        texture.material.color = hitColor;
        yield return new WaitForSeconds(shockInterval);
        texture.material.color = initialColor;
    }
}
