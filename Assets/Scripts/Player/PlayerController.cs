using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{
    [Header("Combat")]
    public float attackCooldown = 0.4f;
    public float attackDamage = 12f; // Example; actual hitbox owned by Boss

    [Header("Dodge")] 
    public float dodgeSpeed = 9f;
    public float dodgeDuration = 0.35f;
    public float iFrameStart = 0.05f;
    public float iFrameDuration = 0.2f;

    [Header("Parry")] 
    public float parryWindow = 0.25f; // seconds
    public float parryRadius = 3f;
    public LayerMask bossLayer;

    [Header("Refs")] 
    public Animator animator;
    public TimeController timeController;

    private PlayerMotor motor;
    private Health health;

    private Vector2 moveInput;
    private bool canAttack = true;
    private bool isDodging;
    private bool parryActive;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!isDodging)
            motor.Move(moveInput);
        motor.TickGravity();
    }

    // Input (PlayerInput: Send Messages)
    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnLook(InputValue v) { /* Use Cinemachine Input Provider for camera */ }

    public void OnAttack()
    {
        if (!canAttack || isDodging) return;
        StartCoroutine(AttackRoutine());
    }

    public void OnDodge()
    {
        if (isDodging) return;
        StartCoroutine(DodgeRoutine());
    }

    public void OnParry()
    {
        if (parryActive) return;
        StartCoroutine(ParryRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        animator?.SetTrigger("Attack");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        animator?.SetTrigger("Dodge");

        float t = 0f;
        Vector3 dir = transform.forward;
        health.SetInvulnerable(false);

        var cc = GetComponent<CharacterController>();

        while (t < dodgeDuration)
        {
            t += Time.deltaTime;
            if (t > iFrameStart && t < iFrameStart + iFrameDuration)
                health.SetInvulnerable(true);
            else
                health.SetInvulnerable(false);

            cc.Move(dir * (dodgeSpeed * Time.deltaTime));
            yield return null;
        }
        health.SetInvulnerable(false);
        isDodging = false;
    }

    private IEnumerator ParryRoutine()
    {
        parryActive = true;
        animator?.SetTrigger("Parry");

        float t = 0f;
        while (t < parryWindow)
        {
            t += Time.deltaTime;
            var hits = Physics.OverlapSphere(transform.position, parryRadius, bossLayer);
            foreach (var h in hits)
            {
                var parryable = h.GetComponentInParent<Parryable>();
                if (parryable != null && parryable.IsOpen)
                {
                    parryable.OnParried(this);
                    timeController?.SlowFor(0.8f, 0.15f);
                }
            }
            yield return null;
        }
        parryActive = false;
    }
}
