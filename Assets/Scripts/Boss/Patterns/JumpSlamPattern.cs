using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Patterns/JumpSlam", fileName = "JumpSlamPattern")]
public class JumpSlamPattern : BossPattern
{
    public float jumpHeight = 3.5f;
    public float airTime = 0.8f;
    public float shockwaveDelay = 0.15f; // after landing

    public override IEnumerator Play(BossController boss)
    {
        boss.Animator.SetTrigger(animationTrigger); // "JumpSlam"
        boss.OnTelegraph(this);
        yield return new WaitForSeconds(telegraphTime);

        Transform t = boss.transform;
        Vector3 start = t.position;
        Vector3 apex = start + Vector3.up * jumpHeight;
        float half = airTime * 0.5f;
        float tmr = 0f;
        while (tmr < half)
        {
            tmr += Time.deltaTime;
            t.position = Vector3.Lerp(start, apex, tmr / half);
            yield return null;
        }
        tmr = 0f;
        while (tmr < half)
        {
            tmr += Time.deltaTime;
            t.position = Vector3.Lerp(apex, start, tmr / half);
            yield return null;
        }

        boss.Hitbox.Enable(damage);
        boss.OnAttackActive(this);

        if (parryable && boss.Parryable != null)
        {
            float openDur = Mathf.Clamp01(parryWindowEnd) * activeTime;
            boss.Parryable.Open(openDur);
        }

        yield return new WaitForSeconds(shockwaveDelay);
        yield return new WaitForSeconds(activeTime);

        boss.Hitbox.Disable();
        boss.OnRecover(this);
        yield return new WaitForSeconds(recoveryTime);
    }
}
