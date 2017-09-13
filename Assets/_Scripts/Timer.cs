using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// C#6の書き方をしているのでコンパイルが通らない場合Unityの設定を帰る必要がある
/// http://qiita.com/divideby_zero/items/71a38acdbaa55e88e2d9
/// </summary>
public class Timer : SingletonMonoBehaviour<Timer>
{
    [SerializeField]
    private Text timerMessage = null;
    //[SerializeField]
    //private int maxDigit = 5;

    private float currentTime = 0;
    private IEnumerator timer;
	private float? countStop = null;

    private void Start()
    {
        //countStop = Mathf.Pow(10, maxDigit) - 1;
        countStop = 99999;
        timer = StartTimer();
        StartCoroutine(timer);
    }

    private void Update()
    {
        timerMessage.text = $"じかん：{(int)currentTime:D5}";
    }

    private IEnumerator StartTimer()
    {
        float timeStart = Time.time;
        yield return null;
        while (currentTime <= countStop)
        {
            currentTime = Time.time - timeStart;
            yield return null;
        }
    }

    public void PauseTimer()
    {
        StopCoroutine(timer);
    }

    public void ResumeTimer()
    {
        StartCoroutine(timer);
    }
}
