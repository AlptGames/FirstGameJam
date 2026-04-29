using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Логика застревания")]
    public float stuckThreshold = 5f; // Сколько секунд монстр может "пытаться" дойти
    private float stuckTimer;
    private Vector3 lastPosition;

    [Header("Звуковые эффекты")]
    public AudioSource sfxSource;      // Для разовых звуков (атака, крик)
    public AudioSource musicSource;    // Для фоновой музыки погони
    public AudioSource footstepsSource; // Для шагов

    public AudioClip attackSound;
    public AudioClip rageSound;
    public AudioClip chaseMusic;
    public AudioClip walkStepSound;
    public AudioClip runStepSound;

    [Header("Настройки")]
    public Transform player;
    public Transform[] waypoints;
    public LayerMask obstacleMask;
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;
    public float viewDistance = 15f;
    public float attackRange = 2.2f;
    public float rageDuration = 2.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isRaging = false;
    private bool playerDetected = false;
    private float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Настройка источника шагов
        footstepsSource.loop = true;
        musicSource.loop = true;
        musicSource.clip = chaseMusic;

        SetRandomDestination();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSee = CanSeePlayer(distanceToPlayer);

        if (isRaging) return;

        if (canSee)
        {
            if (!playerDetected) StartRage();

            if (distanceToPlayer <= attackRange) Attack();
            else Chase();
        }
        else
        {
            if (playerDetected && distanceToPlayer > 25f) LosePlayer();
            else if (playerDetected) Chase();
            else Patrol();
        }

        HandleFootsteps();
    }

    void StartRage()
    {
        playerDetected = true;
        isRaging = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Звук крика
        sfxSource.PlayOneShot(rageSound);
        // Включаем музыку погони
        if (!musicSource.isPlaying) musicSource.Play();

        anim.SetTrigger("rage");
        SetAnimParams(false, false, true);
        Invoke(nameof(EndRage), rageDuration);
    }

    void EndRage()
    {
        isRaging = false;
        agent.isStopped = false;
        agent.speed = runSpeed;
    }

    void Chase()
    {
        if (isRaging) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
        SetAnimParams(false, true, false); // Бег

        // --- ЛОГИКА ПРОВЕРКИ ЗАСТРЕВАНИЯ ---
        // Проверяем, как далеко монстр продвинулся с прошлого кадра
        if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f; // Сбрасываем, если он хоть немного двигается
        }

        lastPosition = transform.position;

        // Если застрял дольше чем на stuckThreshold секунд
        if (stuckTimer >= stuckThreshold)
        {
            Debug.Log("Монстр не может достать игрока и уходит.");
            LosePlayer(); // Вызываем метод потери игрока
            stuckTimer = 0f;
        }
    }

    void Patrol()
    {
        agent.speed = walkSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            SetAnimParams(false, false, true);
            waitTimer += Time.deltaTime;
            if (waitTimer >= 2f) SetRandomDestination();
        }
        else
        {
            agent.isStopped = false;
            SetAnimParams(true, false, false);
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        if (!sfxSource.isPlaying && Random.value > 0.95f) // Чтобы не спамить звуком каждый кадр
            sfxSource.PlayOneShot(attackSound);

        anim.SetTrigger("attack");
        SetAnimParams(false, false, true);
    }

    void LosePlayer()
    {
        playerDetected = false;
        musicSource.Stop(); // Выключаем музыку
        agent.speed = walkSpeed;
        SetRandomDestination();
    }

    void HandleFootsteps()
    {
        // Проверяем, движется ли агент
        if (agent.velocity.magnitude > 0.2f && !agent.isStopped)
        {
            if (!footstepsSource.isPlaying) footstepsSource.Play();

            // Меняем звук шагов в зависимости от скорости
            footstepsSource.clip = (agent.speed > walkSpeed + 1f) ? runStepSound : walkStepSound;
            // Меняем высоту звука (pitch) для бега
            footstepsSource.pitch = (agent.speed > walkSpeed + 1f) ? 1.3f : 1.0f;
        }
        else
        {
            footstepsSource.Stop();
        }
    }

    bool CanSeePlayer(float dist)
    {
        if (dist > viewDistance) return false;
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * 1.5f;
        Vector3 end = player.position + Vector3.up * 1.5f;

        if (Physics.Linecast(start, end, out hit, obstacleMask))
        {
            return hit.transform == player;
        }
        return true;
    }

    void SetRandomDestination()
    {
        if (waypoints.Length == 0) return;
        agent.isStopped = false;
        int index = Random.Range(0, waypoints.Length);
        agent.SetDestination(waypoints[index].position);
        waitTimer = 0f;
    }

    void SetAnimParams(bool walk, bool run, bool idle)
    {
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("idle", idle);
    }
}