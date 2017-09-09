﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 壊したブロックの数によってボールが大きくなる
/// </summary>
public class BallMass : MonoBehaviour
{
    [SerializeField]
    private AudioSource expansionAudio = null;

    [SerializeField]
    private AudioSource breakAudio = null;

    [SerializeField]
    private float[] ballSizes = null;

    [SerializeField]
    private int[] necessaryFeeds = null;

    [SerializeField]
    private Text ballFeedText = null;

    [SerializeField]
    private float expansionSpeed = 1;

    [SerializeField]
    private int initFeed = 5;

    private int currentFeed;
    private int currentLevel = 0;
    private List<GameObject> jointedBlocks = new List<GameObject>();
	private bool growing = false;
	private CameraFollow cameraFollow = null;

    public List<float> BallSizes{
        private set;
        get;
    }

    public List<int> NecessaryFeeds{
        private set;
        get;
    }

    public int CurrentFeed{
        get{
            return currentFeed;
        }
        set{
            currentFeed = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        currentFeed = initFeed;
        BallSizes = new List<float>(ballSizes);
        BallSizes.Insert(0, transform.localScale.x);
        NecessaryFeeds = new List<int>(necessaryFeeds);
        NecessaryFeeds.Insert(0, 0);
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        ballFeedText.text = currentFeed.ToString();
        int nextLevel = currentLevel + 1;
        if (!growing &&
            nextLevel < NecessaryFeeds.Count &&
            currentFeed >= NecessaryFeeds[nextLevel])
        {
            StartCoroutine(GrowUp(nextLevel));
        }
    }

    private IEnumerator GrowUp(int nextLevel)
    {
        growing = true;
        //jointedBlocks.ForEach((b => b.SetActive(false)));
        PlayBallBig();
        yield return null;
        while (transform.localScale.x < BallSizes[nextLevel])
        {
            float size = transform.localScale.x + expansionSpeed;
            transform.localScale = new Vector3(size, size, 1);
            yield return null;
        }
        cameraFollow.ZoomOut(transform.localScale.x);

        currentLevel += 1;
        growing = false;
    }

    public void Feed(int feed, GameObject block){
        currentFeed += feed;
        jointedBlocks.Add(block);
    }
    public int GetcurrentLevel() {
        return currentLevel;
    }

    public void PlayBallBig(){
        if(expansionAudio != null && !expansionAudio.isPlaying){
            expansionAudio.Play();
        }
    }

    public void PlayBlockBreak(){
        if(breakAudio != null && !breakAudio.isPlaying){
            breakAudio.Play();
        }
    }
}
