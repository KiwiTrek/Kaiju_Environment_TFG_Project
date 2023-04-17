using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public DataCompilator compilator = null;
    public Material legMaterial = null;
    public CharacterMov player = null;
    public Animator animatorBoss = null;
    public GameObject spikeShield = null;
    public Collider hitbox = null;

    [Space]
    public int maxLives = 12;
    Color colorInit = Color.gray;
    public int currentHits = 0;
    float timerHit = 0.0f;
    float maxHitTime = 0.15f;
    void Start()
    {
        colorInit = legMaterial.color;
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
                legMaterial.SetColor("_Color", Color.red);
            }
            else
            {
                legMaterial.color = colorInit;
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
        //Play sound
        //Play fx
        currentHits += player.numberClicks;
    }
}
