using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Animator Animator;
    public NavMeshAgent Agent;
    public CharacterController Character;
    public Hitbox Hitbox;
    public Parryable Parryable;
    public Transform Target; // Player
    public TimeController TimeController;

    [Header("Phases")]
    public BossPhase phase1;
    public BossPhase phase2;
    [Range(0f, 1f)] public float phase2Threshold = 0.5f; // 50%

    [Header("Stun/Parry")]
    public float parryStunDuration = 1.2f;

    private Health health;
    private Coroutine aiLoop;
    private bool inPhase2;
    private bool stunned;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Character = GetComponent<CharacterController>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        health.onDie.AddListener(OnDie);
        health.onHealthChanged.AddListener(OnHealthChanged);
        aiLoop = StartCoroutine(AILoop());
    }

    private void OnHealthChanged(float current, float max)
    {
        if (!inPhase2 && current / max <= phase2Threshold)
        {
            inPhase2 = true;
            Animator?.SetTrigger("Phase2");
            TimeController?.SlowFor(0.6f, 0.2f);
        }
    }

    private void OnDie()
    {
        if (aiLoop != null) StopCoroutine(aiLoop);
        Animator?.SetTrigger("Die");
        Hitbox.Disable();
        Agent.isStopped = true;
        enabled = false;
    }

    private IEnumerator AILoop()
    {
        yield return new WaitForSeconds(0.4f);
        while (true)
        {
            if (stunned)
            {
                yield return null;
                continue;
            }

            var phase = inPhase2 ? phase2 : phase1;
            var pattern = phase != null ? phase.PickRandom() : null;
            if (pattern == null)
            {
                yield return null;
                continue;
            }
            yield return StartCoroutine(pattern.Play(this));
        }
    }

    // Hooks from patterns
    public void OnTelegraph(BossPattern pattern)
    {
        FaceTarget();
        Agent.isStopped = true;
    }

    public void OnAttackActive(BossPattern pattern)
    {
        // camera shake / sfx trigger point
    }

    public void OnRecover(BossPattern pattern)
    {
        Agent.isStopped = false;
    }

    public void OnParried(PlayerController player)
    {
        if (stunned) return;
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        stunned = true;
        Animator?.SetTrigger("Stunned");
        Hitbox.Disable();
        TimeController?.SlowFor(0.7f, 0.15f);
        // Open punish window
        health.SetInvulnerable(false);
        yield return new WaitForSeconds(parryStunDuration);
        stunned = false;
        Animator?.SetTrigger("Recover");
    }

    private void FaceTarget()
    {
        if (!Target) return;
        Vector3 d = Target.position - transform.position; d.y = 0f;
        if (d.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(d.normalized);
        }
    }
}
