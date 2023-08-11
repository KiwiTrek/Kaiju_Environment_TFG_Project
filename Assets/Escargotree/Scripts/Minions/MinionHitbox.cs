using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject explosionVFX;
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

        if (GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            lifeTimeRemaining += Time.deltaTime;
            if (lifeTimeRemaining >= lifeTimeTotal)
            {
                Die();
            }
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
        GameObject explosion = Instantiate(explosionVFX, transform.position, transform.rotation);
        Destroy(explosion, 2.0f);
        //PlaySound
        Destroy(transform.parent.gameObject);
    }

    public void SwitchCollider()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = !collider.enabled;
    }
}
