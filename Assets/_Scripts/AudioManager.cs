using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioSource ballExpansion = null;

    [SerializeField]
    private AudioSource blockBreak = null;

    public AudioSource BallExpansion
    {
        get { return ballExpansion; }
    }

    public AudioSource BlockBreak
    {
        get { return blockBreak; }
    }
}
