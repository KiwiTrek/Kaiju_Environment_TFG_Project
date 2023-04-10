using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class BossSubHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxLives = 18;
    public Slider healthBar;
    public List<GameObject> body = new List<GameObject>();
    public CharacterMov player = null;
    public BossSubBehaviour behaviour = null;

    bool activeMesh = true;
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
        healthBar.value = maxLives - currentHits;

        if (currentHits >= maxLives)
        {
            Die();
        }

        if (timerHit < maxHitTime)
        {
            timerHit += Time.deltaTime;
            activeMesh = !activeMesh;
            if (activeMesh)
            {
                foreach (GameObject go in body)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject go in body)
                {
                    go.SetActive(false);
                }
            }
        }
        else
        {
            foreach (GameObject go in body)
            {
                go.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (behaviour.status != BossStatus.Stuck)
        {
            CharacterLives lives = other.GetComponent<CharacterLives>();
            if (lives != null)
            {
                lives.HitHard();
            }
        }
        else
        {
            if (other.gameObject.tag == "Sword")
            {
                Hit();
            }
        }
    }

    void Hit()
    {
        timerHit = 0.0f;
        //Play sound
        //Play fx
        Debug.Log(player.numberClicks);
        currentHits += player.numberClicks;
    }

    private void Die()
    {
        //Instantiate(explosionPrefab, this.position, this.rotation);
        //PlaySound
        Destroy(gameObject);
    }
}
