using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボールにくっついているブロックの管理
/// あまりに多くのブロックがボールにくっつきすぎると物理演算の挙動がおかしくなってしまう
/// そのため、くっつくブロックの数を管理し、決められた数以上のブロックがくっつくときに
/// 古いブロックを消してからくっつけるようにする
/// ただし、古いブロック消える時、そのブロックへのジョイントの参照も失ってしまうため、
/// 参照をボール本体に付け替えてから消すようにしている
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BallSatellites : MonoBehaviour
{
    [SerializeField]
    private int maxQueueLength = 100; // キューの最大長

    [SerializeField]
    private int maxBranchLength = 5; // 数珠つなぎになっているブロックの枝の最大長

    private static Queue<BlockDurability> satellites = null; // 

    private Rigidbody2D _rigidbody = null;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        satellites = new Queue<BlockDurability>();
    }

    public void Enqueue(BlockDurability block)
    {
        if (satellites.Count == maxQueueLength)
        {
            // 古いブロックをキューから外す
            BlockDurability item = satellites.Dequeue();
            // 外されたブロックへのジョイントをボールに付け替える
            List<FixedJoint2D> joints = item.JointedObjects;
            joints.ForEach(j => j.connectedBody = _rigidbody);
            // 外されたブロックを消す
            item.gameObject.SetActive(false);
        }
        satellites.Enqueue(block);
        TrimMaxOverBranch(block);
    }

    /// <summary>
    /// maxBranchLenghを超える長さの枝になる場合
    /// ボール自体にくっつくようにする
    /// ただし、キューの古いブロックが外されてもその先についてるブロックの枝の長さは更新されないので
    /// 実際よりも短い長さで切れるようになってしまう。(別にこの仕様でも困らないので見て見ぬふり)
    /// </summary>
    /// <param name="block">新しくくっつくブロック</param>
    private void TrimMaxOverBranch(BlockDurability block)
    {
        if (block.BranchLengh > maxBranchLength)
        {
            block.Joint.connectedBody = _rigidbody;
            block.BranchLengh = 1;
        }
    }

    public static void ResetQueue()
    {
        foreach (var block in satellites)
        {
            block.gameObject.SetActive(false);
        }
        satellites.Clear();
    }
}
