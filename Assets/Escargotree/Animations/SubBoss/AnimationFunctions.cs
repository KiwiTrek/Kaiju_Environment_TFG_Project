using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    [SerializeField] private BossSubBehaviour behaviour;
    
    [Space]
    [Header ("Components")]
    public AudioSource normalSFX = null;
    public AudioSource hitSFX = null;
    public AudioSource voiceSFX = null;

    [Space]
    [Header("Audio Clips")]
    public AudioClip[] flapping = null;
    public AudioClip[] hitBloody = null;
    public AudioClip[] hurt = null;
    public void ReturnToInitial()
    {
        behaviour.canReturn = true;
    }

    public void PlaySound(SoundTypeBird type)
    {
        normalSFX.pitch = Random.Range(0.9f, 1.1f);
        voiceSFX.pitch = Random.Range(0.95f, 1.05f);
        hitSFX.pitch = Random.Range(0.9f, 1.1f);

        switch (type)
        {
            case SoundTypeBird.Flapping:
                normalSFX.clip = flapping[Random.Range(0, flapping.Length)];
                normalSFX.Play();
                break;
            case SoundTypeBird.HitBloody:
                hitSFX.clip = hitBloody[Random.Range(0, flapping.Length)];
                hitSFX.Play();
                break;
            case SoundTypeBird.Hurt:
                break;
            default:
                break;
        }
    }
}
