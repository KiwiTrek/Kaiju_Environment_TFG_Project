using Cinemachine;
using UnityEngine;

public enum SoundType
{
    Jump,
    Land,
    GruntSwing,
    Sword,
    GruntSwingBig,
    Hurt,
    Thud,
    Death,
    GetUp
}
//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class CharacterMov : MonoBehaviour {

    [Header("Values")]
    [Range(0.0f, 30.0f)]
    public float velocity = 10.0f;
    [Range(1, 5)]
    public float sprintMult = 2;
    [Range(1, 10)]
    public int maxJumpCount = 2;
    [Range(0.0f, 30.0f)]
    public float jumpHeight = 15.0f;
    [Range(-30.0f, 30.0f)]
    public float gravity = 15.0f;
    [Range(0.0f, 0.5f)]
    public float groundDistance = 0.5f;

    [Space]
    [Header("Components")]
    public AudioSource sfx;
    public GameObject[] slashes;
    public Camera cam;
    public CinemachineFreeLook playerCam;
    public CharacterController controller;
    public Collider swordCollider;
    public CharacterLives health;
    public Animator animator;
    public Transform initialPosition;
    public Transform groundChecker;
    public Transform jumpToTrunkFinalPos;
    public Transform cam2dPos;
    public Transform bossIntroPos;
    public Transform birdBossPos;
    public LayerMask groundMask;
    public LayerMask groundMask2;

    [Space]
    [Header("Sound Clip")]
    public AudioClip[] swordSFX;
    public AudioClip[] jumpsSFX;
    public AudioClip[] hurtSFX;
    public AudioClip[] gruntSwingSFX;
    public AudioClip[] getUpSFX;
    public AudioClip gruntSwingBigSFX;
    public AudioClip deathSFX;
    public AudioClip thudSFX;

    [Space]
    public DataCompilator compilator;

    [Space]
    [Header("Debug values")]
    public int currentCameraId;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed = 0.1f;
    public bool isGrounded;
    public bool canAttack = true;
    public int numberClicks = 0;

    public Vector3 verticalMov;
    public int currentJumpCount;
    public bool jumpPressed;
    private int jumpHash;
    Vector3 lastP;
    Vector2 mouseSensibility;
    float timerToSpareJump = 0.0f;
    [SerializeField] private bool setInitialPosition = false;

    int isHurtHardHash;
    int isMovingHash;
    bool sprintPressed;
    int isSprintingHash;
    int attackStartHash;

    // Use this for initialization
    void Start ()
    {
        cam = Camera.main;
        controller = this.GetComponent<CharacterController> ();

        isHurtHardHash = Animator.StringToHash("isHurtHard");
        isMovingHash = Animator.StringToHash("isMoving");
        isSprintingHash = Animator.StringToHash("isSprinting");
        jumpHash = Animator.StringToHash("jump");
        attackStartHash = Animator.StringToHash("attackStart");
        mouseSensibility = new Vector2(playerCam.m_XAxis.m_MaxSpeed, playerCam.m_YAxis.m_MaxSpeed);

        currentCameraId = 0; 
        Cursor.lockState = CursorLockMode.Locked;
        timerToSpareJump = 0.0f;
        
        if (setInitialPosition)
        {
            controller.enabled = false;
            transform.position = initialPosition.position;
            controller.enabled = true;
        }
    }
    
    // Update is called once per frame
    void Update ()
    {
        lastP = transform.position;
        if (numberClicks == 0)
        {
            canAttack = true;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            controller.enabled = false;
            transform.position = birdBossPos.position;
            controller.enabled = true;
        }
#endif

        if (GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            playerCam.m_XAxis.m_MaxSpeed = mouseSensibility.x;
            playerCam.m_YAxis.m_MaxSpeed = mouseSensibility.y;

            InputX = 0.0f;
            InputZ = 0.0f;
            if (!health.dead)
            {
                InputMagnitude();

                //Physically move player
                PlayerMoveAndRotation();
            }
        }
        else
        {
            playerCam.m_XAxis.m_MaxSpeed = 0.0f;
            playerCam.m_YAxis.m_MaxSpeed = 0.0f;
        }

        isGrounded = (Physics.CheckSphere(groundChecker.position, groundDistance, groundMask) || Physics.CheckSphere(groundChecker.position, groundDistance, groundMask2));
        animator.SetBool("grounded", isGrounded);
        if (isGrounded & verticalMov.y < 0)
        {
            verticalMov.y = -0.01f;
            currentJumpCount = maxJumpCount;
        }

        animator.SetBool("jump", jumpPressed);
        if (jumpPressed)
        {
            jumpPressed = false;
            PlaySound(SoundType.Jump);
            verticalMov.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            canAttack = true;
            numberClicks = 0;
            animator.SetBool("attackStart", false);
            animator.SetInteger("hitCount", 0);
        }

        if (GameplayDirector.cutsceneMode == CutsceneType.JumpToBoss)
        {
            controller.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, jumpToTrunkFinalPos.position, Time.deltaTime * 10.0f);
            controller.enabled = true;
            if (Vector3.Distance(transform.position, jumpToTrunkFinalPos.position) <= 0.1f || timerToSpareJump >= 3.5f)
            {
                GameplayDirector.cutsceneMode = CutsceneType.None;
            }
            timerToSpareJump += Time.deltaTime;
        }
        else
        {
            timerToSpareJump = 0.0f;
        }
        
        if (GameplayDirector.cutsceneMode != CutsceneType.None)
        {
            controller.enabled = false;
            if (GameplayDirector.cutsceneMode == CutsceneType.BossIntro)
            {
                transform.SetPositionAndRotation(bossIntroPos.position, bossIntroPos.rotation);
            }
            else if (GameplayDirector.cutsceneMode == CutsceneType.BirdIntro || GameplayDirector.cutsceneMode == CutsceneType.BirdEnd)
            {
                transform.position = birdBossPos.position;
            }
            else if (GameplayDirector.cutsceneMode != CutsceneType.JumpToBoss)
            {
                transform.position = transform.position;
            }
            controller.enabled = true;

            if (GameplayDirector.cutsceneMode != CutsceneType.JumpToBoss)
            {
                animator.SetBool("forceIdle", true);
                animator.SetBool("grounded", true);
            }
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("forceIdle", false);
        }

        verticalMov.y += gravity * Time.deltaTime;
        controller.Move(verticalMov * Time.deltaTime);
        float verticalVelocity = (lastP.y - transform.position.y) / Time.deltaTime;
        animator.SetFloat("jumpVelocity", verticalVelocity);
    }
    void InputMagnitude()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        if (Input.GetButton("Sprint"))
        {
            sprintPressed = true;
        }
        else
        {
            sprintPressed = false;
        }

        if (Input.GetButtonDown("Jump") && currentJumpCount != 0)
        {
            if (compilator != null)
            {
                compilator.RegisterCharacterJump();
            }
            Debug.Log("Jump!");
            currentJumpCount--;
            jumpPressed = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (compilator != null)
            {
                compilator.RegisterAttack();
            }
            if (canAttack && numberClicks < 3)
            {
                numberClicks++;
                if (numberClicks == 1)
                {
                    animator.SetBool("attackStart", true);
                    animator.SetInteger("hitCount", numberClicks);
                }
                canAttack = false;
            }
        }
    }

    //For animation
    public void VerifyCombo()
    {
        if (canAttack)
        {
            numberClicks = 0;
            canAttack = false;
            animator.SetInteger("hitCount", numberClicks);
        }
        else
        {
            if (numberClicks > 1) animator.SetInteger("hitCount", numberClicks);
        }
    }
    public void CanAttackToTrue()
    {
        canAttack = true;
    }
    public void CanAttackToFalse()
    {
        canAttack = false;
    }
    public void ActivateCollider()
    {
        swordCollider.enabled = true;
    }
    public void DeactivateCollider()
    {
        swordCollider.enabled = false;
    }
    public void AttackStarted()
    {
        bool attackStart = animator.GetBool(attackStartHash);
        if (attackStart)
        {
            animator.SetBool("attackStart", false);
        }
    }
    public void InvertCamX(bool value)
    {
        playerCam.m_XAxis.m_InvertInput = value;
    }
    public void InvertCamY(bool value)
    {
        playerCam.m_YAxis.m_InvertInput = !value;
    }
    public void SwitchSensitivity(float sensitivity)
    {
        playerCam.m_XAxis.m_MaxSpeed = 2.0f * sensitivity;
        playerCam.m_YAxis.m_MaxSpeed = sensitivity / 33;
        mouseSensibility = new Vector2(playerCam.m_XAxis.m_MaxSpeed, playerCam.m_YAxis.m_MaxSpeed);
    }
    void PlayerMoveAndRotation()
    {
        bool isMoving = animator.GetBool(isMovingHash);
        bool isSprinting = animator.GetBool(isSprintingHash);

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize ();
        right.Normalize ();

        if (InputX != 0.0f || InputZ != 0.0f)
        {
            if (!isMoving) animator.SetBool("isMoving", true);

            if (sprintPressed)
            {
                if (!isSprinting) animator.SetBool("isSprinting", true);
                InputX *= sprintMult;
                InputZ *= sprintMult;
            }
            else
            {
                if (isSprinting) animator.SetBool("isSprinting", false);
            }
            desiredMoveDirection = forward * InputZ + right * InputX;
            controller.Move(Time.deltaTime * velocity * desiredMoveDirection);
        }
        else if (GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            if (isMoving) animator.SetBool("isMoving", false);
            if (isSprinting) animator.SetBool("isSprinting", false);

            if (playerCam.enabled) desiredMoveDirection = forward;
        }

        //Position corrector 2D level
        if (currentCameraId == 1 && GameplayDirector.cutsceneMode != CutsceneType.JumpToBoss)
        {
            float currentDistance = Mathf.Abs(Vector3.Distance(this.transform.position, cam2dPos.transform.position));
            if (currentDistance >= 16.0f)
            {
                Vector3 dir = cam2dPos.transform.position - this.transform.position;
                controller.Move(dir * Time.deltaTime);
            }
        }
        
        transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }

    public void SlashActivate(int i)
    {
        slashes[i].SetActive(true);
    }

    public void ResetSlashes()
    {
        foreach(var slash in slashes)
        {
            slash.SetActive(false);
        }
    }

    public void PlaySound(SoundType type)
    {
        sfx.pitch = Random.Range(0.90f, 1.1f);
        switch (type)
        {
            case SoundType.Jump:
                {
                    sfx.PlayOneShot(jumpsSFX[Random.Range(0, jumpsSFX.Length)]);
                    break;
                }
            case SoundType.Land:
                {
                    sfx.pitch = Random.Range(0.95f, 1.05f);
                    sfx.PlayOneShot(hurtSFX[Random.Range(1, hurtSFX.Length)]);
                    break;
                }
            case SoundType.GruntSwing:
                {
                    sfx.pitch = Random.Range(0.95f, 1.05f);
                    sfx.PlayOneShot(gruntSwingSFX[Random.Range(0, gruntSwingSFX.Length)]);
                    break;
                }
            case SoundType.Sword:
                {
                    sfx.PlayOneShot(swordSFX[Random.Range(0, swordSFX.Length)]);
                    break;
                }
            case SoundType.GruntSwingBig:
                {
                    sfx.PlayOneShot(gruntSwingBigSFX);
                    break;
                }
            case SoundType.Hurt:
                {
                    sfx.pitch = Random.Range(0.95f, 1.05f);
                    sfx.PlayOneShot(hurtSFX[Random.Range(0, hurtSFX.Length)]);
                    break;
                }
            case SoundType.Thud:
                {
                    sfx.pitch = Random.Range(0.85f, 1.00f);
                    sfx.PlayOneShot(thudSFX);
                    break;
                }
            case SoundType.Death:
                {
                    sfx.pitch = Random.Range(0.95f, 1.05f);
                    sfx.PlayOneShot(deathSFX);
                    break;
                }
            case SoundType.GetUp:
                {
                    sfx.pitch = Random.Range(0.95f, 1.05f);
                    sfx.PlayOneShot(getUpSFX[Random.Range(0, getUpSFX.Length)]);
                    break;
                }
            default:
                break;
        }
    }
}
