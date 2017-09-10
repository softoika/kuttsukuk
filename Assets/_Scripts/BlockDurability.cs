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
    private float scatterImpact = 10;

    [SerializeField]
    private Color hitColor = Color.red;

    [SerializeField]
    private float shockInterval = 0.1f;

    [SerializeField]
    private int ballFeed = 1;

    private Rigidbody2D rig;
    private bool stickable = false;
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
                StartCoroutine(ChangeColor());
            }else if(durability <= 0 && joint.connectedBody == null){
                // ボールにくっつく
                joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                rig.constraints = RigidbodyConstraints2D.None;
                gameObject.tag = "BallSatellites";
                ballMass.Feed(ballFeed, gameObject);
            }
        }

    }

    private IEnumerator ChangeColor(){
        texture.material.color = hitColor;
        AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockBreak);
        yield return new WaitForSeconds(shockInterval);
        texture.material.color = initialColor;
    }

    // ランダムな方向にブロックに力がかかる
    private void Scatter(){
        if (!stickable)
        {
            float rad = Random.Range(0.0f, 360.0f) / (Mathf.PI * 180);
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            rig.AddForce(direction * scatterImpact, ForceMode2D.Impulse);
            stickable = true;
        }
    }
}
