using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLives : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Image blackScreen;
    public TMP_Text livesUI;
    public GameObject spawnPoint;
    public int cameraID;
    public Animator animator;
    public CharacterController controller;
    public CharacterMov movementScript;

    [Space]
    public DataCompilator compilator;

    [Space]
    [Header("Values")]
    public int maxLives;
    public bool dead = false;
    public float respawnTime = 6.0f;
    public float invulnerabilityFrameTimer = 1.5f;

    [Space]
    [Header("Mesh")]
    public GameObject hair;
    public GameObject head;
    public GameObject torso;
    public GameObject legs;
    public GameObject sword;

    [HideInInspector]
    [SerializeField]
    bool activeMesh;
    public int lives;
    public float invulnerableTimer = 0.0f;
    public float deathCounter = -1.0f;

    int isHurtHash;
    int isHurtHardHash;
    void Start()
    {
        lives = maxLives;

        isHurtHash = Animator.StringToHash("isHurt");
        isHurtHardHash = Animator.StringToHash("isHurtHard");
    }

    // Update is called once per frame
    void Update()
    {
        livesUI.text = "Lives: " + lives;

        if (invulnerableTimer > 0.0f)
        {
            invulnerableTimer -= Time.deltaTime;
            activeMesh = !activeMesh;
            if(activeMesh)
            {
                hair.SetActive(true);
                head.SetActive(true);
                torso.SetActive(true);
                legs.SetActive(true);
                sword.SetActive(true);
            }
            else
            {
                hair.SetActive(false);
                head.SetActive(false);
                torso.SetActive(false);
                legs.SetActive(false);
                sword.SetActive(false);
            }
        }
        else if (invulnerableTimer < 0.0f)
        {
            invulnerableTimer = 0.0f;
            hair.SetActive(true);
            head.SetActive(true);
            torso.SetActive(true);
            legs.SetActive(true);
            sword.SetActive(true);

            movementScript.numberClicks = 0;
            movementScript.animator.SetBool("attackStart", false);
            movementScript.animator.SetInteger("hitCount", movementScript.numberClicks);
            movementScript.canAttack = true;
        }

        if (dead)
        {
            Debug.Log("Dead!");
            deathCounter += Time.deltaTime;
            if (deathCounter >= (respawnTime/2.0f))
            {
                //Reset & Respawn
                controller.enabled = false;
                transform.SetPositionAndRotation(spawnPoint.transform.position, Quaternion.LookRotation(-spawnPoint.transform.forward));
                controller.enabled = true;
                cameraID = spawnPoint.GetComponent<CheckpointID>().cameraId;
                lives = maxLives;

                Color tmp = blackScreen.color;
                float curr = deathCounter - (respawnTime / 2.0f);
                tmp.a = 1.0f - (curr / (respawnTime / 2.0f));
                blackScreen.color = tmp;
            }
            else
            {
                Color tmp = blackScreen.color;
                tmp.a = (deathCounter / (respawnTime / 2.0f));
                blackScreen.color = tmp;
            }

            if (deathCounter > respawnTime)
            {
                dead = false;
                deathCounter = -1.0f;
            }
        }

        animator.SetFloat("deadCounter", deathCounter);
    }
    public void Hit()
    {
        if (invulnerableTimer <= 0 && lives > 0)
        {
            lives--;
            if (compilator != null)
            {
                compilator.RegisterRecieveDamage();
            }
        }

        movementScript.canAttack = false;
        movementScript.numberClicks = 0;
        movementScript.VerifyCombo();

        if (lives <= 0)
        {
            if (!dead)
            {
                dead = true;
                if (compilator != null)
                {
                    compilator.RegisterDeath();
                }
                deathCounter = 0.12f;
            }
        }
        else
        {
            bool isHurt = animator.GetBool(isHurtHash);
            invulnerableTimer = invulnerabilityFrameTimer;
            if (!isHurt)
            {
                animator.SetBool("isHurt", true);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            Debug.Log("Checkpoint!");
            spawnPoint = other.gameObject;
        }
    }

    public void HitHard()
    {
        if (invulnerableTimer <= 0)
        {
            lives--;
            if (compilator != null)
            {
                compilator.RegisterRecieveDamage();
            }
        }

        movementScript.canAttack = false;
        movementScript.numberClicks = 0;
        movementScript.VerifyCombo();

        if (lives <= 0)
        {
            if (!dead)
            {
                dead = true;
                deathCounter = 0.12f;
                if (compilator != null)
                {
                    compilator.RegisterDeath();
                }
            }
        }
        else
        {
            bool isHurt = animator.GetBool(isHurtHardHash);
            invulnerableTimer = invulnerabilityFrameTimer;
            if (!isHurt)
            {
                animator.SetBool("isHurtHard", true);
            }
        }
    }

    //For animation
    public void HasBeenHurt()
    {
        bool isHurt = animator.GetBool(isHurtHash);
        if (isHurt)
        {
            animator.SetBool("isHurt", false);
        }
    }

    public void HasBeenHurtHard()
    {
        bool isHurtHard = animator.GetBool(isHurtHardHash);
        if (isHurtHard)
        {
            animator.SetBool("isHurtHard", false);
        }
    }
}
