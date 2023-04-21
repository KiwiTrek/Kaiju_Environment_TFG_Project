using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShooter : MonoBehaviour
{
    // Start is called before the first frame update
    public Renderer render;
    public Transform barrel;
    public GameObject minionPrefab;
    public GameObject spawnPosition;
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
                frequency = maxFrequencyOfUpdate;
                if (foundTarget)
                {
                    frequency = 0.0f;
                    render.material.color = Color.yellow;
                    SpawnMinion();
                }
            }
            float t = frequency / maxFrequencyOfUpdate;
            render.material.color = Color.Lerp(Color.yellow, Color.red, t);

        }
    }
    void SpawnMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, spawnPosition.transform.position, Quaternion.LookRotation(spawnPosition.transform.forward));
        MinionHitbox minionHitbox = minionInstance.GetComponentInChildren<MinionHitbox>();
        if (minionHitbox != null) minionHitbox.isMinion = true;
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
