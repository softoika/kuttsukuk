using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioPlayer
{
    /// <summary>
    /// 同じAudio Sourceは重複して鳴らさないようにSEを鳴らす
    /// </summary>
    /// <param name="audioSource">Audio source.</param>
    public static void PlayNonOverwrapping(AudioSource audioSource)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}