using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public enum SoundTypeBird
{
    Flapping,
    HitBloody,
    Hurt,
    Thrust,
    Shockwave,
    EggPop
}
public class BossSubHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public DataCompilator compilator;
    public Slider healthBar;
    public List<GameObject> body = new List<GameObject>();
    public CharacterMov player = null;
    public GameObject featherVFX = null;
    public BossSubBehaviour behaviour = null;
    public AnimationFunctions animationFunctions = null;

    [Space]
    [Header("Parameters")]
    public int maxLives = 18;
    public int currentHits = 0;
    bool activeMesh = true;
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
        if (compilator != null)
        {
            compilator.RegisterHitEnemy();
        }
        timerHit = 0.0f;

        animationFunctions.PlaySound(SoundTypeBird.Hurt);
        animationFunctions.PlaySound(SoundTypeBird.HitBloody);

        GameObject vfx = Instantiate(featherVFX, transform);
        Destroy(vfx, 1.0f);
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
