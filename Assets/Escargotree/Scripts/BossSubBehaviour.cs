using Cinemachine;
using UnityEngine;
public enum BossStatus
{
    Idle,
    Thrust,
    Stuck,
    Invulnerable
}
public enum InvulnerablePhase
{
    Returning,
    Charging,
    Thrusting
}
public enum CurrentAnimation
{
    Idle = 0,
    Readying,
    Thrust,
    HitGround,
    Struggling,
    Release,
    Tantrum,
    Death
}
public class BossSubBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Parameters")]
    public Transform initialPosition = null;
    [Range(1.0f, 50.0f)]
    public float speed = 1.0f;
    public Vector2 timeBeforeStrikeRange = new(0.0f, 1.0f);
    public float timeStunned = 5.0f;
    public float sizeReduce = 200.0f;
    public float distanceBeforeImpact = 2.35f;
    public float height = 0.0f;
    public float height2 = 0.0f;
    public float finalHeightMultiplier = 0.1f;
    public BossStatus status = BossStatus.Idle;

    [Header("Components")]
    public Animator animator;
    public AnimationFunctions animationFunctions = null;
    public BossMov mainBoss = null;
    public GameObject motionVFX = null;
    public GameObject shockwavePrefab = null;
    public GameObject particlesPrefab = null;
    public GameObject minionPrefab = null;
    public GameObject minionVFX = null;
    public BossSubHitbox subHitbox;
    public CinemachineVirtualCamera bossCamera;
    public GameObject playerTarget;
    public GameObject preemptiveShadow;
    public GameObject healthBarUI;
    public CapsuleCollider capsule;
    public BoxCollider box;

    [Space(10)]
    public bool canReturn = false;
    float timeBeforeStrike = 0.0f;
    bool minionAttackType = true;
    Vector3 thrustObjective = Vector3.zero;
    Quaternion currentRotation = Quaternion.identity;
    float timeCountStruggle = 0.0f;
    float currentAngle = 0.0f;
    float currentTimeStrike = 0.0f;
    float currentTimeStuck = 0.0f;
    float currentTimeInvulnerable = 0.0f;
    readonly float timeBetweenBirds = 0.5f;
    int numberThrust = 0;
    InvulnerablePhase invulnerablePhase = InvulnerablePhase.Thrusting;
    void Start()
    {
        timeBeforeStrike = Random.Range(timeBeforeStrikeRange.x, timeBeforeStrikeRange.y);
        status = BossStatus.Idle;
        healthBarUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossCamera.enabled)
        {
            status = BossStatus.Idle;
            SwitchAnimation(CurrentAnimation.Idle);
            preemptiveShadow.SetActive(false);
            if (Vector3.Distance(transform.position, initialPosition.position) >= 0.01f)
            {
                transform.LookAt(initialPosition);
                transform.position = Vector3.MoveTowards(transform.position, initialPosition.position, Time.deltaTime * speed / 1.5f);
            }
            else
            {
                transform.LookAt(playerTarget.transform.position);
            }
            box.enabled = false;
            capsule.enabled = true;
            currentRotation = Quaternion.identity;
            currentTimeStrike = 0.0f;
            currentTimeStuck = 0.0f;
            currentTimeInvulnerable = 0.0f;
            timeCountStruggle = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            healthBarUI.SetActive(false);
            minionAttackType = false;
            canReturn = false;
            animator.SetFloat("speedIdle", 1.0f);
            animator.SetFloat("speedReadying", 1.0f);
            motionVFX.SetActive(false);
            return;
        }

        if (GameplayDirector.cutsceneMode == CutsceneType.None)
        {
            healthBarUI.SetActive(true);
            switch (status)
            {
                case BossStatus.Idle:
                    {
                        IdleState();
                    }
                    break;
                case BossStatus.Thrust:
                    {
                        ThrustState();
                    }
                    break;
                case BossStatus.Stuck:
                    {
                        StuckState();
                    }
                    break;
                case BossStatus.Invulnerable:
                    {
                        if (minionAttackType)
                        {
                            MinionState();
                        }
                        else
                        {
                            InvulnerableState();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            healthBarUI.SetActive(false);
            if (GameplayDirector.cutsceneMode == CutsceneType.BirdEnd)
            {
                SwitchAnimation(CurrentAnimation.Death);
                if (animationFunctions.gravity)
                {
                    transform.transform.position = Vector3.MoveTowards(transform.position, preemptiveShadow.transform.position, Time.deltaTime * 9.8f);
                }
                else
                {
                    transform.SetPositionAndRotation(
                        Vector3.MoveTowards(transform.position, initialPosition.position, Time.deltaTime * speed / 1.5f),
                        Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mainBoss.transform.forward), Time.deltaTime * 10.0f));

                    preemptiveShadow.transform.position = transform.position;
                    Vector3 positionCorrector = new(preemptiveShadow.transform.localPosition.x, height + 0.05f, preemptiveShadow.transform.localPosition.z);
                    preemptiveShadow.transform.localPosition = positionCorrector;
                }
            }
        }
    }

    private void IdleState()
    {
        currentTimeStrike += Time.deltaTime;
        animator.SetFloat("speedReadying", 1.0f);
        motionVFX.SetActive(false);

        preemptiveShadow.SetActive(true);
        preemptiveShadow.transform.position = playerTarget.transform.position;
        Vector3 positionCorrector = new(preemptiveShadow.transform.localPosition.x, height, preemptiveShadow.transform.localPosition.z);
        preemptiveShadow.transform.localPosition = positionCorrector;

        transform.LookAt(preemptiveShadow.transform.position);

        if (currentTimeStrike >= (timeBeforeStrike - 2.5f))
        {
            animator.SetFloat("speedIdle", 1.5f);
            currentAngle += Time.deltaTime * 10.0f;
            if (Time.timeScale > 0.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosition.position - (this.transform.forward * 2.0f), Time.deltaTime * 2.5f);
                Vector3 scale = new
                    ((Mathf.Sin(currentAngle) / sizeReduce) + preemptiveShadow.transform.localScale.x,
                    (Mathf.Sin(currentAngle) / sizeReduce) + preemptiveShadow.transform.localScale.y,
                    preemptiveShadow.transform.localScale.z);
                preemptiveShadow.transform.localScale = scale;
            }
        }
        else
        {
            animator.SetFloat("speedIdle", 1.0f);
        }

        if (currentTimeStrike >= (timeBeforeStrike - 1.2f))
        {
            SwitchAnimation(CurrentAnimation.Readying);
        }
        else
        {
            SwitchAnimation(CurrentAnimation.Idle);
        }

        if (currentTimeStrike >= timeBeforeStrike)
        {
            SwitchAnimation(CurrentAnimation.Thrust);
            status = BossStatus.Thrust;
            timeBeforeStrike = Random.Range(timeBeforeStrikeRange.x, timeBeforeStrikeRange.y);
            currentTimeStrike = 0.0f;
            currentAngle = 0.0f;
            box.enabled = false;
            capsule.enabled = true;
            animationFunctions.PlaySound(SoundTypeBird.Thrust);
        }
    }
    private void ThrustState()
    {
        if (preemptiveShadow.activeSelf)
        {
            thrustObjective = preemptiveShadow.transform.position;
            preemptiveShadow.SetActive(false);
            motionVFX.SetActive(true);
        }

        if (Vector3.Distance(thrustObjective, transform.position) >= distanceBeforeImpact)
        {
            currentRotation = transform.rotation;
            transform.position = Vector3.MoveTowards(transform.position, thrustObjective, Time.deltaTime * speed);
        }
        else
        {
            motionVFX.SetActive(false);
            SwitchAnimation(CurrentAnimation.HitGround);
            timeCountStruggle = 0.0f;
            SpawnShockwave();
            status = BossStatus.Stuck;
            box.enabled = true;
            capsule.enabled = false;
        }
    }
    private void StuckState()
    {
        currentTimeStuck += Time.deltaTime;

        if (currentTimeStuck >= timeStunned)
        {
            SwitchAnimation(CurrentAnimation.Release);
            if (canReturn)
            {
                if (Vector3.Distance(transform.position, initialPosition.position) >= 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, initialPosition.position, Time.deltaTime * speed / 1.5f);
                    transform.LookAt(thrustObjective);
                }
                else
                {
                    canReturn = false;
                    status = BossStatus.Invulnerable;
                    currentTimeStuck = 0.0f;
                    transform.rotation = Quaternion.identity;
                    currentRotation = Quaternion.identity;
                    box.enabled = false;
                    capsule.enabled = true;
                }
            }
        }
        else
        {
            Quaternion finalRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            SwitchAnimation(CurrentAnimation.Struggling);
            Debug.Log(Mathf.Abs(Mathf.Cos(Quaternion.Angle(currentRotation, finalRotation) * Mathf.Deg2Rad)));
            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, new Vector3(thrustObjective.x, thrustObjective.y - (Mathf.Abs(Mathf.Cos(Quaternion.Angle(currentRotation, finalRotation) * Mathf.Deg2Rad)) * 1.3f) + 2.15f, thrustObjective.z), timeCountStruggle * 0.65f),
                Quaternion.Lerp(currentRotation, finalRotation, timeCountStruggle * 0.65f));
            timeCountStruggle += Time.deltaTime;
        }
    }
    private void InvulnerableState()
    {
        currentTimeInvulnerable += Time.deltaTime;
        animator.SetFloat("speedReadying", 1.5f);
        if (numberThrust >= 3)
        {
            SwitchAnimation(CurrentAnimation.Idle);
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            currentTimeStrike = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        if (currentTimeInvulnerable <= 2.0f)
        {
            SwitchAnimation(CurrentAnimation.Tantrum);
            thrustObjective = playerTarget.transform.position;
            transform.LookAt(thrustObjective);
        }
        else
        {
            switch (invulnerablePhase)
            {
                case InvulnerablePhase.Returning:
                    {
                        SwitchAnimation(CurrentAnimation.Thrust);
                        motionVFX.SetActive(true);
                        if (Vector3.Distance(transform.position, initialPosition.position) >= 0.01f)
                        {
                            transform.LookAt(initialPosition);
                            transform.position = Vector3.MoveTowards(transform.position, initialPosition.position, Time.deltaTime * speed);
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Charging;
                            numberThrust++;
                        }
                    }
                    break;
                case InvulnerablePhase.Charging:
                    {
                        SwitchAnimation(CurrentAnimation.Readying);
                        motionVFX.SetActive(false);
                        if (currentTimeStrike <= 1.33f)
                        {
                            currentTimeStrike += Time.deltaTime;
                            preemptiveShadow.SetActive(true);
                            preemptiveShadow.transform.position = playerTarget.transform.position;
                            Vector3 positionCorrector = new(preemptiveShadow.transform.localPosition.x, height2, preemptiveShadow.transform.localPosition.z);
                            preemptiveShadow.transform.localPosition = positionCorrector;

                            thrustObjective = preemptiveShadow.transform.position;
                            transform.LookAt(thrustObjective);
                            transform.position = Vector3.MoveTowards(transform.position, initialPosition.position - (this.transform.forward * 2.0f), Time.deltaTime * 3.0f);
                        }
                        else
                        {
                            animationFunctions.PlaySound(SoundTypeBird.Thrust);
                            invulnerablePhase = InvulnerablePhase.Thrusting;
                            currentTimeStrike = 0.0f;
                        }
                    }
                    break;
                case InvulnerablePhase.Thrusting:
                    {
                        SwitchAnimation(CurrentAnimation.Thrust);
                        motionVFX.SetActive(true);
                        if (Vector3.Distance(thrustObjective, transform.position) >= distanceBeforeImpact)
                        {
                            transform.LookAt(thrustObjective);
                            transform.position = Vector3.MoveTowards(transform.position, thrustObjective, Time.deltaTime * speed);
                            preemptiveShadow.SetActive(false);
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Returning;
                            SpawnShockwave();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void SpawnShockwave()
    {
        GameObject instance = Instantiate(shockwavePrefab, transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        ShockwaveBehaviour behaviour = instance.GetComponentInChildren<ShockwaveBehaviour>();
        if (behaviour != null)
        {
            behaviour.lifeTimeTotal = 2.0f;
            behaviour.expansionRate = 1.75f;
            behaviour.finalHeightMultiplier = finalHeightMultiplier;
        }

        GameObject particles = Instantiate(particlesPrefab, transform.position + particlesPrefab.transform.position, particlesPrefab.transform.rotation);
        Destroy(particles, 2.0f);
    }

    private void MinionState()
    {
        currentTimeInvulnerable += Time.deltaTime;
        if (numberThrust >= 2)
        {
            SwitchAnimation(CurrentAnimation.Idle);
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        SwitchAnimation(CurrentAnimation.Tantrum);
        thrustObjective = playerTarget.transform.position;
        transform.LookAt(thrustObjective);

        if (currentTimeInvulnerable >= 1.5f)
        {
            if (currentTimeInvulnerable >= 1.5f + timeBetweenBirds)
            {
                currentTimeInvulnerable = 1.5f;
                SpawnMinion();
                animationFunctions.PlaySound(SoundTypeBird.EggPop);
                numberThrust++;
            }
        }
    }

    void SpawnMinion()
    {
        GameObject bird = Instantiate(minionPrefab, transform.position + new Vector3(1.0f, 0.0f), Quaternion.identity);
        bird.transform.LookAt(playerTarget.transform.position);
        bird.GetComponent<MinionBehaviour>().maxVelocity = 9.0f;
        bird = Instantiate(minionPrefab, transform.position + new Vector3(-1.0f, 0.0f), Quaternion.identity);
        bird.transform.LookAt(playerTarget.transform.position);
        bird.GetComponent<MinionBehaviour>().maxVelocity = 9.0f;

        GameObject birdVFX = Instantiate(minionVFX, bird.transform);
        Destroy(birdVFX, 0.6f);
    }

    public void SwitchAnimation(CurrentAnimation animation)
    {
        animator.SetInteger("currentAnimation", (int)animation);
    }
}
