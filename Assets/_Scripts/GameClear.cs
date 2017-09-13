using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameClear : SingletonMonoBehaviour<GameClear>
{
    [SerializeField]
    private int clearLevel = 3;
    [SerializeField]
    private GameObject ball = null;
    [SerializeField]
    private GameObject gameClearText = null;

    private BallMass ballMass;

	void Start ()
	{
        ballMass = ball.GetComponent<BallMass>();
        gameClearText.SetActive(false);
	}
	
	void Update ()
	{
        if(ballMass.CurrentLevel >= clearLevel){
            gameClearText.SetActive(true);
            Timer.Instance.PauseTimer();
        }
	}
}
