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
    private float invincibleTime = 0.4f; // ブロックがダメージを受けて次にダメージを受けるまでの無敵時間(秒)

    [SerializeField]
    private int ballFeed = 1; // ブロックがボールにくっついたときにボールに加算されるfeed値(攻撃力)

    private Rigidbody2D _rigidbody;
    private Color initialColor;
    private Renderer _renderer;
    private FixedJoint2D joint = null;
    private BallMass ballMass;
    private BallSatellites ballSatellites;
    // ブロックがダメージを受けたら一定時間ダメージを受けない無敵時間の状態を表す
    private bool invincible = false;
    // このブロックを参照しているジョイントリスト
    private readonly List<FixedJoint2D> jointedObjects = new List<FixedJoint2D>();
    // このブロックまでの(ジョイントでつながれたブロックを木とみなした)枝の長さ
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

    /// <summary>
    /// ボール(もしくはボールの周囲にくっついているブロック[衛星]）に当たると
    /// 耐久値が0より大きければ耐久値を減らす(ダメージ後一定の無敵時間あり)
    /// 耐久値が0以下ならボールもしくは衛星にくっつく
    /// </summary>
    /// <param name="col">Collision2D</param>
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (IsDamageable(col))
        {
            if (durability > 0)
            {
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockBreak);
                ReduceDurability();
            }
            else if (durability <= 0 && joint.connectedBody == null)
            {
                AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BlockSticking);
                Stick(col);
            }
        }
    }

    private bool IsDamageable(Collision2D col)
    {
        return !invincible &&
               (col.gameObject.tag == "Ball" ||
                col.gameObject.tag == "BallSatellites");
    }

    private void ReduceDurability()
    {
        durability -= ballMass.CurrentFeed;
        StartCoroutine(ToInvincible());
    }

    private void Stick(Collision2D col)
    {
        _rigidbody.constraints = RigidbodyConstraints2D.None;
		if (col.gameObject.tag == "BallSatellites")
		{
			var colBlock = col.gameObject.GetComponent<BlockDurability>();
			colBlock.MemoryJointedObject(joint); // 自分をくっつく先に記憶させる
            branchLengh += colBlock.BranchLengh;  // 自分の枝の長さにくっつく先のもつ枝の長さを加算する
		}
		joint.connectedBody = col.gameObject.GetComponent<Rigidbody2D>();

        gameObject.tag = "BallSatellites";
		ballSatellites.Enqueue(this);

		ballMass.AddFeed(ballFeed);
		ballMass.AddMass(_rigidbody.mass); // ボールの自体の重さも増やしてバランスを取れるようにする
	}

    private IEnumerator ToInvincible()
    {
		invincible = true;
        yield return ChangeColor();
        yield return new WaitForSeconds(invincibleTime - shockInterval); // shockIntervalは無敵時間に含まれるので
		invincible = false;
    }

    private IEnumerator ChangeColor() 
    {
        _renderer.material.color = hitColor;
        yield return new WaitForSeconds(shockInterval);
        _renderer.material.color = initialColor;
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
