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
    public Animator animatorBoss = null;
    public GameObject spikeShield = null;
    public BossHitbox otherLeg = null;
    public Collider hitbox = null;

    [Space]
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
            this.hitbox.enabled = false;
            spikeShield.SetActive(true);
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
        currentHits += player.numberClicks;
    }
}
