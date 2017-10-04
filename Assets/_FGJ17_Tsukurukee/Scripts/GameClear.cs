using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゲームクリア条件
/// ボールのレベルが一定に達するとクリアメッセージを出す
/// </summary>
public class GameClear : SingletonMonoBehaviour<GameClear>
{
    [SerializeField]
    private int clearLevel = 3;
    [SerializeField]
    private float messageDurability = 3; // 「ゲームクリア！」の部分が表示される時間
    [SerializeField]
    private GameObject ball = null;
    [SerializeField]
    private GameObject gameClearText = null;

    private BallMass ballMass = null;
    private TextMeshProUGUI gameClearMessage = null;

	void Start ()
	{
        ballMass = ball.GetComponent<BallMass>();
        gameClearMessage = gameClearText.GetComponent<TextMeshProUGUI>();
        gameClearText.SetActive(false);
	}
	
	void Update ()
	{
        if (ballMass.CurrentLevel == clearLevel)
        {
            Timer.Instance.PauseTimer();
            StartCoroutine(ShowClearMessage());
        }
        else if (ballMass.CurrentLevel == clearLevel + 1){
            BallSatellites.ResetQueue();
        }
	}

    public IEnumerator ShowClearMessage()
    {
        gameClearText.SetActive(true);
        yield return new WaitForSeconds(messageDurability);
        gameClearMessage.enabled = false;
    }
}
