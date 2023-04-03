using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShooter : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform barrel;
    public GameObject minionPrefab;
    public bool playerControlled = false;
    public float maxFrequencyOfUpdate = 1.5f;

    float frequency;
    GameObject target;
    private bool foundTarget;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControlled)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnMinion();
            }
        }
        else
        {
            frequency += Time.deltaTime;
            if (frequency >= maxFrequencyOfUpdate)
            {
                if (foundTarget)
                {
                    frequency = 0.0f;
                    SpawnMinion();
                }
            }
        }
    }
    void SpawnMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, barrel.position + new Vector3(-3.0f, 0, 0), Quaternion.LookRotation(barrel.up));
        MinionHitbox minionHitbox = minionInstance.GetComponentInChildren<MinionHitbox>();
        minionHitbox.isMinion = true;

        MinionBehaviour minionBehaviour = minionInstance.GetComponent<MinionBehaviour>();
        minionBehaviour.direction = minionInstance.transform.position - barrel.position;
        minionBehaviour.direction.Normalize();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On enemy radius");
        if (other.gameObject == target)
        {
            Debug.Log("Target nearby");
            foundTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Outside enemy radius");
        if (other.gameObject == target)
        {
            Debug.Log("Target has left radious");
            foundTarget = false;
        }
    }
}
