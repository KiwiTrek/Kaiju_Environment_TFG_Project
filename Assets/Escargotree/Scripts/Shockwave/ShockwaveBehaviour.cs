using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwaveBehaviour : MonoBehaviour
{
    // Minion Behaviour Stuff
    public CapsuleCollider safeZone = null;
    public ShockwaveSafezone safeTrigger = null;
    public float lifeTimeTotal = 0.0f;
    float lifeTimeRemaining = 0.0f;

    public bool expand = true;
    public float lockedSize = 1.0f;
    public float expansionRate = 1.0f;
    float currentSize = 0.0f;

    private void Start()
    {
        lifeTimeRemaining = 0.0f;
    }
    public void Update()
    {
        if (expand)
        {
            currentSize += expansionRate * Time.deltaTime;
            Vector3 scale = new Vector3(
                currentSize,
                currentSize,
                transform.localScale.z
                );
            transform.localScale = scale;
            safeZone.height += currentSize * 100;

            lifeTimeRemaining += Time.deltaTime;
            if (lifeTimeRemaining >= lifeTimeTotal)
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
                transform.localScale.z
                );
            transform.localScale = scale;
            safeZone.height = currentSize * 100;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!safeTrigger.isSafe)
        {
            CharacterLives lives = other.gameObject.GetComponent<CharacterLives>();
            if (lives != null)
            {
                lives.HitHard();
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
        Destroy(gameObject);
    }
}
