using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isMinion = true;
    private void OnTriggerEnter(Collider other)
    {
        CharacterLives lives = other.GetComponent<CharacterLives>();
        if (lives != null)
        {
            lives.Hit();
        }

        if (isMinion)
        {
            if (other.gameObject.tag == "Sword")
            {
                Debug.Log("Enemy death! No damage.");
            }
            
            //Instantiate(explosionPrefab, this.position, this.rotation);
            Destroy(transform.parent.gameObject);
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
}
