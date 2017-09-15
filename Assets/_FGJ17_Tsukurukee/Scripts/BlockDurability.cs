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
    private int durability = 10; // ブロックのもつ耐久度

    [SerializeField]
    private Color hitColor = Color.red; // ボールがブロックに当たったときに変わる色

    [SerializeField]
    private float shockInterval = 0.1f; // ボールがブロックに当たったとき色が変わる時間(秒)

    [SerializeField]
    private float invisibleTime = 0.4f; // ブロックがダメージを受けて次にダメージを受けるまでの無敵時間(秒)

    [SerializeField]
    private int ballFeed = 1; // ブロックがボールにくっついたときにボールに加算されるfeed値(攻撃力)

    private Rigidbody2D _rigidbody;
    private Color initialColor;
    private Renderer _renderer;
    private FixedJoint2D joint = null;
    private BallMass ballMass;
    private BallSatellites ballSatellites;
    private bool invisible = false;
    // このブロックを参照しているジョイントリスト
    private readonly List<FixedJoint2D> jointedObjects = new List<FixedJoint2D>();
    // このブロックまでの(ジョイントでつながれたブロックを木とみなした)枝の長さ
    [SerializeField]
    private int branchLengh = 1;

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

    public int BranchLengh
    {
        get { return branchLengh; }
        set { branchLengh = value; }
    }

    public FixedJoint2D Joint
    {
        get { return joint; }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        _rigidbody.simulated = false;
        _renderer = GetComponent<Renderer>();
        joint = GetComponent<FixedJoint2D>();
        initialColor = _renderer.material.color;
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
            else if (durability <= 0 && joint.connectedBody == null) // 耐久値0以下でまだ何にもくっついていなければ
            {
                // くっつく先がサテライトブロックなら、
                if (collision.gameObject.tag == "BallSatellites")
                {
                    var colBlock = collision.gameObject.GetComponent<BlockDurability>();
                    // 自分をくっつく先に記憶させる
                    colBlock.MemoryJointedObject(joint);
                    // 自分の枝の長さにくっつく先のもつ枝の長さを加算する
                    branchLengh += colBlock.BranchLengh;
                }
				// ボール、もしくはサテライトブロックにくっつく
				joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();

                ballSatellites.Enqueue(this);

                _rigidbody.constraints = RigidbodyConstraints2D.None;
                gameObject.tag = "BallSatellites";
                ballMass.AddFeed(ballFeed);
                // ボールの自体の重さも増やしてバランスを取れるようにする
                ballMass.AddMass(_rigidbody.mass);
                // くっつくときの音
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockSticking);
            }
        }

    }

    private IEnumerator ChangeColor() // 実質無敵時間も兼ねている
    {
        invisible = true;
        _renderer.material.color = hitColor;
        yield return new WaitForSeconds(shockInterval);
        _renderer.material.color = initialColor;
        yield return new WaitForSeconds(invisibleTime);
        invisible = false;
    }

    public void MemoryJointedObject(FixedJoint2D joint){
        jointedObjects.Add(joint);
    }

    private void OnBecameVisible()
    {
        _rigidbody.simulated = true;
    }

    private void OnBecameInvisible()
    {
        if (gameObject.tag == "FixedBlock")
        {
            _rigidbody.simulated = false;
        }
    }
}
