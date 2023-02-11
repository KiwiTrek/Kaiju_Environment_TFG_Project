using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword")
        {
            Debug.Log("Enemy death! No damage.");
        }
        else
        {
            Debug.Log("Enemy has done some damage. Oops!");
            other.GetComponent<CharacterLives>().Hit();
        }

        //Instantiate(explosionPrefab, this.position, this.rotation);
        Destroy(transform.parent.gameObject);
    }
}
