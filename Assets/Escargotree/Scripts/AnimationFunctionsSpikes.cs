using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunctionsSpikes : MonoBehaviour
{
    public AudioSource sfx = null;
    public AudioClip clip = null;
    public void PlaySound(float vol)
    {
        sfx.pitch = Random.Range(0.9f, 1.1f);
        sfx.volume = vol;
        sfx.PlayOneShot(clip);
    }
}
