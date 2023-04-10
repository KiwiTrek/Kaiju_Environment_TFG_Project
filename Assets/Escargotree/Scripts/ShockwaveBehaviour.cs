using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwaveBehaviour : MonoBehaviour
{
    // Minion Behaviour Stuff
    public float lifeTimeTotal = 0.0f;
    float lifeTimeRemaining = 0.0f;

    public float expansionRate = 1.0f;
    float currentSize = 0.0f;

    private void Start()
    {
        lifeTimeRemaining = 0.0f;
    }
    public void Update()
    {
        currentSize += expansionRate * Time.deltaTime;
        Vector3 scale = new Vector3(
            currentSize,
            currentSize,
            transform.localScale.z
            );
        transform.localScale = scale;


        lifeTimeRemaining += Time.deltaTime;
        if (lifeTimeRemaining >= lifeTimeTotal)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider other)
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
    private void Die()
    {
        //Instantiate(explosionPrefab, this.position, this.rotation);
        Destroy(gameObject);
    }
}
