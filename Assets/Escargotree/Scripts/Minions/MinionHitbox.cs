using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isMinion = true;

    // Minion Behaviour Stuff
    public float lifeTimeTotal = 0.0f;
    MinionBehaviour behaviour;
    float lifeTimeRemaining = 0.0f;

    private void Start()
    {
        if (isMinion)
        {
            lifeTimeRemaining = 0.0f;
            behaviour = transform.parent.GetComponent<MinionBehaviour>();
        }
    }
    public void Update()
    {
        if (!isMinion)
        {
            return;
        }

        lifeTimeRemaining += Time.deltaTime;
        if (lifeTimeRemaining >= lifeTimeTotal)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CharacterLives lives = other.GetComponent<CharacterLives>();
        if (lives != null)
        {
            lives.Hit();
        }

        if (isMinion)
        {
            if (behaviour.canFollow)
            {
                if (other.gameObject.tag == "Player"
                || other.gameObject.tag == "Sword"
                || other.gameObject.tag == "Environment")
                {
                    Debug.Log("Enemy death! No damage.");
                    Die();
                }
            }
            else
            {
                if (other.gameObject.tag == "Player"
                || other.gameObject.tag == "Sword"
                || other.gameObject.tag == "EnvironmentIgnoreCam")
                {
                    Debug.Log("Enemy death! No damage.");
                    Die();
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        CharacterLives lives = other.GetComponent<CharacterLives>();
        if (lives != null)
        {
            if (lives.invulnerableTimer <= 0 && lives.lives > 0)
            {
                lives.Hit();
            }
        }
    }
    private void Die()
    {
        //Instantiate(explosionPrefab, this.position, this.rotation);
        //PlaySound
        Destroy(transform.parent.gameObject);
    }
}
