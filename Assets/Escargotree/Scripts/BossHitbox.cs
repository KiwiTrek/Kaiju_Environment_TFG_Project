using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Renderer legRenderer = null;
    public GameObject legBrokenDecals = null;

    [Space(10)]
    public DataCompilator compilator = null;
    public CharacterMov player = null;
    public Collider hitbox = null;

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

            if (legBrokenDecals != null)
            {
                legBrokenDecals.SetActive(true);
            }

            currentHits = maxLives;
        }
        else
        {
            if (timerHit < maxHitTime)
            {
                timerHit += Time.deltaTime;
                legRenderer.material.color = Color.red;
            }
            else
            {
                legRenderer.material.color = Color.white;
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
