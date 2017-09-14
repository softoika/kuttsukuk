using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 壊したブロックの数によってボールが大きくなる
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BallMass : MonoBehaviour
{
    [SerializeField]
    private float[] ballSizes = null;    // レベルごとの膨張サイズ

    [SerializeField]
    private int[] necessaryFeeds = null; // レベルごとの必要feed数

    [SerializeField]
    private Text ballFeedText = null;    // 現在のfeed数を表示するテキスト

    [SerializeField]
    private float expansionSpeed = 1;    // 膨張速度

    [SerializeField]
    private int initFeed = 5;            // 初期feed数

    private int currentFeed;
    private int currentLevel = 0;
    private bool growing = false;
    private CameraFollow cameraFollow = null;
    private Rigidbody2D rig = null;

    public List<float> BallSizes
    {
        private set;
        get;
    }

    public List<int> NecessaryFeeds
    {
        private set;
        get;
    }

    public int CurrentFeed
    {
        get
        {
            return currentFeed;
        }
        set
        {
            currentFeed = value;
        }
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
    }

    private void Start()
    {
        currentFeed = initFeed;
        // ゲーム開始時の膨張サイズを現在のx座標軸のスケールとする
        BallSizes = new List<float>(ballSizes);
        BallSizes.Insert(0, transform.localScale.x);

        // ゲーム開始時の必要feed数を0とする
        NecessaryFeeds = new List<int>(necessaryFeeds);
        NecessaryFeeds.Insert(0, 0);

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ballFeedText.text = currentFeed.ToString();
        int nextLevel = currentLevel + 1;
        if (!growing &&                                // 膨張中はGrowUpを実行しない
            nextLevel < NecessaryFeeds.Count &&        // 設定したレベル以内で
            currentFeed >= NecessaryFeeds[nextLevel])  // 次のレベルの必要数のfeedを獲得したら
        {
            StartCoroutine(GrowUp(nextLevel)); // 膨張処理を実行する
        }
    }

    /// <summary>
    /// 膨張処理。膨張処理時に重複して膨張処理が呼ばれないようにするため、
    /// growingを用いて処理をロックしている。
    /// 膨張処理実行中は毎フレームexpansionSpeedだけスケールを大きくして、
    /// 指定する次のレベルの大きさまで大きくなる。
    /// 膨張処理の終了時にカメラをズームアウトして現在のレベルを
    /// 指定した次のレベルまで増やす。
    /// レベルを指定できるのでレベルを２つ飛ばしに膨張することも可能
    /// </summary>
    /// <returns>IEnumerator</returns>
    /// <param name="nextLevel">指定する次のレベル</param>
    private IEnumerator GrowUp(int nextLevel)
    {
        growing = true;
        AudioPlayer.PlayNonOverwrapping(AudioManager.Instance.BallExpansion);
        yield return null;
        while (transform.localScale.x < BallSizes[nextLevel])
        {
            float size = transform.localScale.x + expansionSpeed;
            transform.localScale = new Vector3(size, size, 1);
            yield return null;
        }
        cameraFollow.ZoomOut(transform.localScale.x);

        currentLevel = nextLevel;
        growing = false;
    }

    public void AddFeed(int feed)
    {
        currentFeed += feed;
    }

    public void AddMass(float mass)
    {
        rig.mass += mass;
    }
}