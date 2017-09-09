using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioPlayer
{
    public static void PlayNonOverwrapping(AudioSource audioSource)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}