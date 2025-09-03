using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Patterns/Slash", fileName = "SlashPattern")]
public class SlashPattern : BossPattern
{
    [Header("Movement")]
    public float lungeDistance = 2.0f;
    public float lungeSpeed = 8f;

    public override System.Collections.IEnumerator Play(BossController boss)
    {
        boss.Animator.SetTrigger(animationTrigger);
        boss.OnTelegraph(this);
        yield return new WaitForSeconds(telegraphTime);

        float t = 0f;
        Vector3 dir = (boss.Target.position - boss.transform.position);
        dir.y = 0f; dir.Normalize();
        boss.transform.rotation = Quaternion.LookRotation(dir);

        boss.Hitbox.Enable(damage);
        boss.OnAttackActive(this);

        if (parryable && boss.Parryable != null)
        {
            float openT = Mathf.Clamp01(parryWindowStart) * activeTime;
            float closeT = Mathf.Clamp01(parryWindowEnd) * activeTime;
            float endT = activeTime;
            while (t < endT)
            {
                t += Time.deltaTime;
                float move = lungeSpeed * Time.deltaTime;
                boss.Character.Move(dir * move);
                if (t >= openT && t <= closeT)
                {
                    boss.Parryable.Open(closeT - openT);
                    closeT = -1f; // prevent reopen
                }
                yield return null;
            }
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
