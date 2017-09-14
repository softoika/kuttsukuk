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

    private static Queue<GameObject> satellites = null; // 

    private Rigidbody2D _rigidbody = null;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        satellites = new Queue<GameObject>();
    }

    public void Enqueue(GameObject block)
    {
        if (satellites.Count == maxQueueLength)
        {
            // 古いブロックをキューから外す
            GameObject obj = satellites.Dequeue();
            // 外されたブロックへのジョイントをボールに付け替える
            List<FixedJoint2D> joints = obj.GetComponent<BlockDurability>().JointedObjects;
            joints.ForEach(j => j.connectedBody = _rigidbody);
            // 外されたブロックを消す
            obj.SetActive(false);
        }
        satellites.Enqueue(block);
    }

    public void ResetQueue()
    {
        foreach (var block in satellites)
        {
            block.SetActive(false);
        }
        satellites.Clear();
    }
}
