using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity;
    public float multiplier;
    public float turnSpeed;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 direction = Vector3.zero;

    public GameObject target = null;
    bool foundTarget = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < player.transform.childCount; i++)
        {
            GameObject tmp = player.transform.GetChild(i).gameObject;
            if (tmp.tag == "Target")
            {
                Debug.Log("Found target");
                target = tmp;
                break;
            }
        }

        if (target == null)
        {
            Debug.Log("Target not found. Player set instead");
            target = player;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.Log("No target assigned.");
            return;
        }

        if (Vector3.Angle(transform.forward, direction) <= 2.0f)
        {
            direction = target.transform.position - transform.position;
        }

        if (foundTarget)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * velocity * multiplier * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * velocity * Time.deltaTime;
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
        else
        {
            foundTarget = false;
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
