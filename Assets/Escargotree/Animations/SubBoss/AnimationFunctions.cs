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
                voiceSFX.clip = hurt[Random.Range(0, hurt.Length)];
                voiceSFX.Play();
                break;
            case SoundTypeBird.Thrust:
                hitSFX.clip = thrustSFX;
                hitSFX.Play();
                break;
            case SoundTypeBird.Shockwave:
                voiceSFX.clip = shockwaveSFX;
                voiceSFX.Play();
                break;
            case SoundTypeBird.EggPop:
                voiceSFX.clip = eggPopSFX[Random.Range(0, eggPopSFX.Length)];
                voiceSFX.Play();
                break;
            default:
                break;
        }
    }

    public void PlaySoundDelayed(SoundTypeBird type, float delayTime = 0.0f)
    {
        normalSFX.pitch = Random.Range(0.9f, 1.1f);
        voiceSFX.pitch = Random.Range(0.95f, 1.05f);
        hitSFX.pitch = Random.Range(0.9f, 1.1f);

        switch (type)
        {
            case SoundTypeBird.Flapping:
                normalSFX.clip = flapping[Random.Range(0, flapping.Length)];
                normalSFX.PlayDelayed(delayTime);
                break;
            case SoundTypeBird.HitBloody:
                hitSFX.clip = hitBloody[Random.Range(0, flapping.Length)];
                hitSFX.PlayDelayed(delayTime);
                break;
            case SoundTypeBird.Hurt:
                voiceSFX.clip = hurt[Random.Range(0, hurt.Length)];
                voiceSFX.PlayDelayed(delayTime);
                break;
            case SoundTypeBird.Thrust:
                voiceSFX.clip = thrustSFX;
                voiceSFX.PlayDelayed(delayTime);
                break;
            case SoundTypeBird.Shockwave:
                voiceSFX.clip = shockwaveSFX;
                voiceSFX.PlayDelayed(delayTime);
                break;
            case SoundTypeBird.EggPop:
                hitSFX.clip = eggPopSFX[Random.Range(0, eggPopSFX.Length)];
                hitSFX.PlayDelayed(delayTime);
                break;
            default:
                break;
        }
    }

    public void PlaySoundMinion(SoundTypeMinion type)
    {
        normalSFX.pitch = Random.Range(0.9f, 1.1f);

        switch (type)
        {
            case SoundTypeMinion.Flapping:
                normalSFX.clip = flapping[Random.Range(0, flapping.Length)];
                normalSFX.Play();
                break;
            case SoundTypeMinion.Beeping:
                voiceSFX.clip = beeping;
                voiceSFX.Play();
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
