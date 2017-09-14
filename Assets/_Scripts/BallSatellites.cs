using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class BallSatellites : MonoBehaviour
{
    [SerializeField]
    private int queueLength = 100;

    private static Queue<GameObject> satellites = null;

    private Rigidbody2D rig = null;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        satellites = new Queue<GameObject>();
    }

    public void Enqueue(GameObject block)
    {
        if (satellites.Count == queueLength)
        {
            // 古いブロックをキューから外す
            GameObject obj = satellites.Dequeue();
            // 外されたブロックへのジョイントをボールに付け替える
            List<FixedJoint2D> joints = obj.GetComponent<BlockDurability>().JointedObjects;
            joints.ForEach(j => j.connectedBody = rig);
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
