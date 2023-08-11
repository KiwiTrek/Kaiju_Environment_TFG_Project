using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShooter : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Animator animator;
    public AudioSource audioSource;
    public MeshRenderer cannonMesh;
    public Transform barrel;
    public GameObject minionPrefab;
    public GameObject spawnPosition;
    public GameObject explosionGO;

    [Space]
    [Header("Audio Clips")]
    public AudioClip[] eggPopSFX;

    [Space]
    [Header("Parameters")]
    public int materialIndex;
    public bool playerControlled = false;
    public float maxFrequencyOfUpdate = 1.5f;
    public bool reverse = false;
    float frequency;
    GameObject target;
    CharacterMov targetMov;
    private bool foundTarget;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        targetMov = target.GetComponent<CharacterMov>();
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
        else if (GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            frequency += Time.deltaTime;
            if (frequency >= maxFrequencyOfUpdate)
            {
                frequency = maxFrequencyOfUpdate;
                animator.SetFloat("cannonDuration", 1.0f);
                cannonMesh.materials[materialIndex].SetFloat("_Fill", 1.0f);
                if (foundTarget && targetMov.currentCameraId == 1)
                {
                    frequency = 0.0f;
                    animator.SetFloat("cannonDuration", -1.0f);
                    cannonMesh.materials[materialIndex].SetFloat("_Fill", 0.0f);
                    SpawnMinion();
                }
            }
            else
            {
                float t = frequency / maxFrequencyOfUpdate;
                animator.SetFloat("cannonDuration", t);
                cannonMesh.materials[materialIndex].SetFloat("_Fill", t);
                animator.Play("Rolling", 0, t);
            }
        }
    }
    void SpawnMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, spawnPosition.transform.position, Quaternion.LookRotation(spawnPosition.transform.forward));
        MinionHitbox minionHitbox = minionInstance.GetComponentInChildren<MinionHitbox>();
        if (minionHitbox != null) minionHitbox.isMinion = true;

        MinionBehaviour minionBehaviour = minionInstance.GetComponent<MinionBehaviour>();
        if (minionBehaviour != null) minionBehaviour.reverse = reverse;

        GameObject explosionVFX = Instantiate(explosionGO, spawnPosition.transform.position, Quaternion.identity);
        Destroy(explosionVFX, 0.75f);

        audioSource.clip = eggPopSFX[Random.Range(0, eggPopSFX.Length)];
        audioSource.pitch = Random.Range(0.85f, 1.15f);
        audioSource.Play();
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
