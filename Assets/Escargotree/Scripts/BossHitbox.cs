using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LegType
{
    FrontLeft,
    BackLeft,
    BackRight,
}
public class BossHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Renderer legRenderer = null;
    public LegType legType = LegType.FrontLeft;
    public Material legMaterial = null;
    public Material legDamagedMaterial = null;
    public Material legDestroyedMaterial = null;
    public Material legDestroyedMaterialLeft = null;
    public Material legDestroyedMaterialRight = null;
    public Material legDestroyedMaterialBoth = null;

    [Space(10)]
    public DataCompilator compilator = null;
    public CharacterMov player = null;
    public Collider hitbox = null;
    public BossHitbox otherLeg = null;

    [Space]
    public bool isDummy = false;
    public int maxLives = 12;
    public int currentHits = 0;
    float timerHit = 0.0f;
    float maxHitTime = 0.15f;
    void Start()
    {
        timerHit = maxHitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHits >= maxLives)
        {
            if (isDummy)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.hitbox.enabled = false;
            }

            switch (legType)
            {
                case LegType.FrontLeft:
                    legRenderer.material = legDestroyedMaterial;
                    break;
                case LegType.BackLeft:
                    if (otherLeg.currentHits == otherLeg.maxLives)
                    {
                        legRenderer.material = legDestroyedMaterialBoth;
                    }
                    else
                    {
                        legRenderer.material = legDestroyedMaterialLeft;
                    }
                    break;
                case LegType.BackRight:
                    if (otherLeg.currentHits == otherLeg.maxLives)
                    {
                        legRenderer.material = legDestroyedMaterialBoth;
                    }
                    else
                    {
                        legRenderer.material = legDestroyedMaterialRight;
                    }
                    break;
                default:
                    break;
            }
            currentHits = maxLives;
        }
        else
        {
            if (timerHit < maxHitTime)
            {
                timerHit += Time.deltaTime;
                legRenderer.material = legDamagedMaterial;
            }
            else
            {
                switch (legType)
                {
                    case LegType.FrontLeft:
                        legRenderer.material = legMaterial;
                        break;
                    case LegType.BackLeft:
                        if (otherLeg.currentHits == otherLeg.maxLives)
                        {
                            legRenderer.material = legDestroyedMaterialRight;
                        }
                        else
                        {
                            legRenderer.material = legMaterial;
                        }
                        break;
                    case LegType.BackRight:
                        if (otherLeg.currentHits == otherLeg.maxLives)
                        {
                            legRenderer.material = legDestroyedMaterialLeft;
                        }
                        else
                        {
                            legRenderer.material = legMaterial;
                        }
                        break;
                    default:
                        break;
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword")
        {
            Hit();
        }
    }

    void Hit()
    {
        if (compilator != null)
        {
            compilator.RegisterHitEnemy();
        }
        timerHit = 0.0f;
        if (otherLeg != null)
        {
            otherLeg.timerHit = 0.0f;
        }
        //Play sound
        //Play fx
        if (isDummy)
        {
            currentHits = player.numberClicks;
        }
        else
        {
            currentHits += player.numberClicks;
        }
    }
}
