using UnityEngine;

public class BossAnimatorEvents : MonoBehaviour
{
    public Hitbox hitbox;

    public void Anim_EnableHitbox() => hitbox?.Enable(hitbox.damage);
    public void Anim_DisableHitbox() => hitbox?.Disable();
    public void Anim_HitStop(float ms)
    {
        var tc = FindObjectOfType<TimeController>();
        tc?.HitStop(ms / 1000f);
    }
}
