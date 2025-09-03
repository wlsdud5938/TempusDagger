using System.Collections;
using UnityEngine;

public abstract class BossPattern : ScriptableObject
{
    [Header("Timings (sec)")]
    public float telegraphTime = 0.6f;
    public float activeTime = 0.4f;
    public float recoveryTime = 0.6f;

    [Header("Parry Window (in Active Phase)")]
    public bool parryable = true;
    [Range(0f, 1f)] public float parryWindowStart = 0.2f;
    [Range(0f, 1f)] public float parryWindowEnd = 0.6f;

    [Header("Damage / Misc")]
    public float damage = 18f;
    public string animationTrigger = "Slash";

    public virtual IEnumerator Play(BossController boss)
    {
        boss.Animator.SetTrigger(animationTrigger);
        boss.OnTelegraph(this);
        yield return new WaitForSeconds(telegraphTime);

        boss.Hitbox.Enable(damage);
        boss.OnAttackActive(this);

        if (parryable && boss.Parryable != null)
        {
            float openT = Mathf.Clamp01(parryWindowStart) * activeTime;
            float closeT = Mathf.Clamp01(parryWindowEnd) * activeTime;
            yield return new WaitForSeconds(openT);
            boss.Parryable.Open(closeT - openT);
            yield return new WaitForSeconds(activeTime - openT);
        }
        else
        {
            yield return new WaitForSeconds(activeTime);
        }

        boss.Hitbox.Disable();
        boss.OnRecover(this);
        yield return new WaitForSeconds(recoveryTime);
    }
}
