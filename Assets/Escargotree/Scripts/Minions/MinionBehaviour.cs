using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum SoundTypeMinion
{
    Flapping,
    Beeping
}

public class MinionBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Parameters")]
    public bool canFollow = true;
    public bool reverse = false;
    public bool stop;
    public float maxVelocity;
    public float multiplier;
    public float turnSpeed;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 direction = Vector3.zero;

    [Space]
    [Header("Components")]
    public Animator animator;
    public AnimationFunctions animationFunctions;

    [Space]
    public GameObject target = null;
    public bool foundTarget = false;
    float velocity;

    private void Start()
    {
        foundTarget = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < player.transform.childCount; i++)
        {
            GameObject tmp = player.transform.GetChild(i).gameObject;
            if (tmp.CompareTag("Target"))
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

        velocity = maxVelocity;
    }
    // Update is called once per frame
    void Update()
    {
        if (!stop && GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            if (target == null)
            {
                Debug.Log("No target assigned.");
                return;
            }

            direction = target.transform.position - transform.position;

            if (foundTarget && canFollow)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
                transform.position += multiplier * Time.deltaTime * velocity * transform.forward;
                velocity = (maxVelocity * (Vector3.Distance(this.transform.position, target.transform.position) / 12)) + 3.0f;
                if (animator != null)
                {
                    animator.SetBool("foundTarget", foundTarget);
                }
            }
            else
            {
                velocity = maxVelocity + 3.0f;
                transform.position += Time.deltaTime * velocity * transform.forward;
                if (!canFollow)
                {
                    if (reverse)
                    {
                        direction = transform.forward + new Vector3(0, 0, -turnSpeed * Time.deltaTime);
                    }
                    else
                    {
                        direction = transform.forward + new Vector3(0, 0, turnSpeed * Time.deltaTime);
                    }
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On enemy radius");
        if (other.gameObject == target)
        {
            Debug.Log("Target nearby");
            foundTarget = true;
            if (canFollow)
            {
                animationFunctions.PlaySoundMinion(SoundTypeMinion.Beeping);
                animationFunctions.voiceSFX.loop = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Outside enemy radius");
        if (other.gameObject == target)
        {
            Debug.Log("Target has left radious");
            foundTarget = false;
            if (canFollow)
            {
                animationFunctions.voiceSFX.Stop();
                animationFunctions.voiceSFX.loop = false;
            }
        }
    }
}
