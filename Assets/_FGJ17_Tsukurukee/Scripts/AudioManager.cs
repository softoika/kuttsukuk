using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Audio Sourceをこのクラスのフィールドとプロパティに追加して参照するように使う
/// SingletonMonoBehaviourを継承しているので
/// AudioManager.Instance.BallExpansion のようにして参照することができる
/// </summary>
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
