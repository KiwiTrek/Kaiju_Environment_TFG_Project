using UnityEngine;
public class BossHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Renderer legRenderer = null;
    public GameObject legBrokenDecals = null;
    public AudioSource legAudioSource = null;
    public GameObject woodVFX = null;

    [Space]
    [Header("Audio Clips")]
    public AudioClip[] legSFX = null;
    public AudioClip legDestroyedSFX = null;

    [Space(10)]
    public DataCompilator compilator = null;
    public CharacterMov player = null;
    public Collider hitbox = null;

    [Space]
    public bool isDummy = false;
    public float speedDummy = 1.0f;
    public int maxLives = 12;
    public int currentHits = 0;
    float timerHit = 0.0f;
    readonly float maxHitTime = 0.15f;
    void Start()
    {
        timerHit = maxHitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHits >= maxLives)
        {
            if (isDummy)
            {
                Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90.0f, -230.0f, 0.0f), Time.deltaTime * speedDummy);
                this.transform.rotation = rotation;
            }
            this.hitbox.enabled = false;

            if (legBrokenDecals != null)
            {
                if (legBrokenDecals.activeSelf == false)
                {
                    legAudioSource.clip = legDestroyedSFX;
                    legAudioSource.pitch = Random.Range(0.9f, 1.1f);
                    legAudioSource.Play();

                    legBrokenDecals.SetActive(true);
                }
            }

            currentHits = maxLives;
        }
        else
        {
            if (timerHit < maxHitTime)
            {
                timerHit += Time.deltaTime;
                legRenderer.material.color = Color.red;
            }
            else
            {
                legRenderer.material.color = Color.white;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            Hit();
        }
    }

    void Hit()
    {
        if (compilator != null)
        {
            compilator.RegisterHitEnemy();
        }
        timerHit = 0.0f;

        legAudioSource.clip = legSFX[Random.Range(0, legSFX.Length)];
        legAudioSource.pitch = Random.Range(0.9f, 1.1f);
        legAudioSource.Play();

        GameObject wood = Instantiate(woodVFX, this.transform.position, this.transform.rotation);
        wood.transform.LookAt(player.transform.position);
        Vector3 eulerAngles = wood.transform.eulerAngles;
        eulerAngles.x = 0.0f;
        wood.transform.eulerAngles = eulerAngles;
        Destroy( wood , 1.0f);


        if (isDummy)
        {
            currentHits = player.numberClicks;
        }
        else
        {
            currentHits += player.numberClicks;
        }
    }
}
