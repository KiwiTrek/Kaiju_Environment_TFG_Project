using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public DataCompilator compilator = null;
    public Renderer legRenderer = null;
    public Material legMaterial = null;
    public Material legDamagedMaterial = null;
    public CharacterMov player = null;
    public Collider hitbox = null;
    public GameObject spikeShield = null;
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

            if (spikeShield!= null)
            {
                spikeShield.SetActive(true);
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
                legRenderer.material = legMaterial;
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
