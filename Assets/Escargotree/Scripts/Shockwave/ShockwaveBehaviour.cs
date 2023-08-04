using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwaveBehaviour : MonoBehaviour
{
    // Minion Behaviour Stuff
    [Header("Components")]
    public CapsuleCollider safeZone = null;
    public ShockwaveSafezone safeTrigger = null;

    [Header("Parameters")]
    public float lifeTimeTotal = 0.0f;
    public float speed = 1.0f;
    public float finalHeightMultiplier = 0.666666667f;
    public bool expand = true;
    public bool canHurt = true;
    public float lockedSize = 1.0f;
    public float expansionRate = 1.0f;
    
    float lifeTimeRemaining = 0.0f;
    float currentSize = 0.0f;

    private void Start()
    {
        lifeTimeRemaining = 0.0f;
    }
    public void Update()
    {
        if (expand)
        {
            lifeTimeRemaining += Time.deltaTime;

            if (lifeTimeRemaining < lifeTimeTotal)
            {
                currentSize += expansionRate * Time.deltaTime * speed;

                if (Time.timeScale > 0.0f)
                {
                    Vector3 scale = new Vector3(
                        currentSize,
                        currentSize,
                        transform.parent.localScale.z + currentSize * (finalHeightMultiplier / 100.0f)
                        );
                    transform.parent.localScale = scale;
                    safeZone.height += currentSize * 100;
                }
            }
            else
            {
                Die();
            }
        }
        else
        {
            currentSize = lockedSize;
            Vector3 scale = new Vector3(
                currentSize,
                currentSize,
                transform.parent.localScale.z
                );
            transform.parent.localScale = scale;
            safeZone.height = currentSize * 100;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!safeTrigger.isSafe)
        {
            if (canHurt)
            {
                CharacterLives lives = other.gameObject.GetComponent<CharacterLives>();
                if (lives != null)
                {
                    lives.HitHard();
                }
            }

            if (other.gameObject.tag == "Player")
            {
                Die();
            }
        }
    }
    private void Die()
    {
        //Instantiate(explosionPrefab, this.position, this.rotation);
        Destroy(transform.parent.gameObject);
    }
}
