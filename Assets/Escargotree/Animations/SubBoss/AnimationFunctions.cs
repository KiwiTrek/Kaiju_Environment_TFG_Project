using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    [SerializeField] private BossSubBehaviour behaviour;
    
    [Space]
    [Header ("Components")]
    public AudioSource sfx = null;
    public AudioSource beepingSFX = null;

    [Space]
    [Header("Audio Clips Both")]
    public AudioClip[] flapping = null;
    public AudioClip beeping = null;

    [Space]
    [Header("Audio Clips SubBoss")]
    public AudioClip[] hitBloody = null;
    public AudioClip[] hurt = null;
    public AudioClip[] eggPopSFX = null;
    public AudioClip shockwaveSFX = null;
    public AudioClip thrustSFX = null;

    [Space]
    public bool gravity = false;
    public bool startFadeOutDeath = false;

    public void ReturnToInitial()
    {
        behaviour.canReturn = true;
    }

    public void PlaySound(SoundTypeBird type)
    {
        sfx.pitch = Random.Range(0.9f, 1.1f);
        switch (type)
        {
            case SoundTypeBird.Flapping:
                sfx.PlayOneShot(flapping[Random.Range(0, flapping.Length)]);
                break;
            case SoundTypeBird.HitBloody:
                sfx.PlayOneShot(hitBloody[Random.Range(0, flapping.Length)]);
                break;
            case SoundTypeBird.Hurt:
                sfx.PlayOneShot(hurt[Random.Range(0, hurt.Length)]);
                break;
            case SoundTypeBird.Thrust:
                sfx.PlayOneShot(thrustSFX);
                break;
            case SoundTypeBird.Shockwave:
                sfx.pitch = Random.Range(0.95f, 1.05f);
                sfx.PlayOneShot(shockwaveSFX);
                break;
            case SoundTypeBird.EggPop:
                sfx.pitch = Random.Range(0.95f, 1.05f);
                sfx.PlayOneShot(eggPopSFX[Random.Range(0, eggPopSFX.Length)]);
                break;
            default:
                break;
        }
    }
    public void PlaySoundMinion(SoundTypeMinion type)
    {
        sfx.pitch = Random.Range(0.9f, 1.1f);

        switch (type)
        {
            case SoundTypeMinion.Flapping:
                sfx.PlayOneShot(flapping[Random.Range(0, flapping.Length)]);
                break;
            case SoundTypeMinion.Beeping:
                beepingSFX.Play();
                break;
            default:
                break;
        }
    }

    public void GoDownDeath()
    {
        gravity = true;
    }

    public void StartFadeOut()
    {
        startFadeOutDeath = true;
    }
}
