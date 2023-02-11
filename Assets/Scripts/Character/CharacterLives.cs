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
    public Transform spawnPoint;
    public Animator animator;
    public CharacterController controller;
    public CharacterMov movementScript;

    [Space]
    [Header("Values")]
    public int maxLives;
    public bool dead = false;
    public float respawnTime = 6.0f;

    int lives;
    float deathCounter = -1.0f;

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
        if (dead)
        {
            Debug.Log("Dead!");
            deathCounter += Time.deltaTime;
            if (deathCounter >= (respawnTime/2.0f))
            {
                //Reset & Respawn
                controller.enabled = false;
                transform.SetPositionAndRotation(spawnPoint.transform.position, Quaternion.LookRotation(-spawnPoint.forward));
                controller.enabled = true;
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
        lives--;

        movementScript.canAttack = false;
        movementScript.numberClicks = 0;
        movementScript.VerifyCombo();

        bool isHurt = animator.GetBool(isHurtHash);

        if (lives <= 0)
        {
            dead = true;
            deathCounter = 0.12f;
        }
        else
        {
            if (!isHurt)
            {
                animator.SetBool("isHurt", true);
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
