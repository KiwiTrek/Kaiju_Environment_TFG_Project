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
    public bool isReverse = false;

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
        Vector3 minionFinalPos = Vector3.zero;
        if (isReverse)
        {
            minionFinalPos = barrel.position + new Vector3(4.0f, 0, 0);
        }
        else
        {
            minionFinalPos = barrel.position + new Vector3(-4.0f, 0, 0);
        }
        GameObject minionInstance = Instantiate(minionPrefab, minionFinalPos, Quaternion.LookRotation(barrel.up));
        MinionHitbox minionHitbox = minionInstance.GetComponentInChildren<MinionHitbox>();
        if (minionHitbox != null) minionHitbox.isMinion = true;

        MinionBehaviour minionBehaviour = minionInstance.GetComponent<MinionBehaviour>();
        if (minionBehaviour != null)
        {
            if (isReverse)
            {
                minionBehaviour.direction = barrel.position - minionInstance.transform.position;
            }
            else
            {
                minionBehaviour.direction = minionInstance.transform.position - barrel.position;
            }
            minionBehaviour.direction.Normalize();
        }
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
