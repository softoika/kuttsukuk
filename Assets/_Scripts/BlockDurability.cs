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
    private float invisibleTime = 0.4f;

    [SerializeField]
    private int ballFeed = 1;

    private Rigidbody2D rig;
    private Color initialColor;
    private Renderer texture;
    private FixedJoint2D joint;
    private BallMass ballMass;
    private BallSatellites ballSatellites;
    private bool invisible = false;
    private readonly List<FixedJoint2D> jointedObjects = new List<FixedJoint2D>();

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

    public List<FixedJoint2D> JointedObjects{
        get { return jointedObjects; }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.constraints = RigidbodyConstraints2D.FreezePosition;
        rig.Sleep();
        rig.simulated = false;
        texture = GetComponent<Renderer>();
        joint = GetComponent<FixedJoint2D>();
        initialColor = texture.material.color;
        GameObject ball = GameObject.FindWithTag("Ball");
        ballMass = ball.GetComponent<BallMass>();
        ballSatellites = ball.GetComponent<BallSatellites>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!invisible // 無敵時間中は衝突判定を行わない
            && (collision.gameObject.tag == "Ball"
                || collision.gameObject.tag == "BallSatellites"))
        {

            if (durability > 0)
            {
                durability -= ballMass.CurrentFeed;
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockBreak);
                StartCoroutine(ChangeColor());
            }
            else if (durability <= 0 && joint.connectedBody == null)
            {
                // ボール、もしくはサテライトブロックにくっつく
                joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                // くっつく先がブロックなら、自分をくっつく先に記憶させる
                if (collision.gameObject.tag == "BallSatellites")
                {
                    collision.gameObject.GetComponent<BlockDurability>().MemoryJointedObject(joint);
                }
                ballSatellites.Enqueue(gameObject);

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
        invisible = true;
        texture.material.color = hitColor;
        yield return new WaitForSeconds(shockInterval);
        texture.material.color = initialColor;
        yield return new WaitForSeconds(invisibleTime);
        invisible = false;
    }

    public void MemoryJointedObject(FixedJoint2D joint){
        jointedObjects.Add(joint);
    }

    private void OnBecameVisible()
    {
        rig.simulated = true;
    }
    private void OnWillRenderObject()
    {
        rig.simulated = true;
    }
    private void OnBecameInvisible()
    {
        if (gameObject.tag == "FixedBlock")
        {
            rig.simulated = false;
        }
    }
}
