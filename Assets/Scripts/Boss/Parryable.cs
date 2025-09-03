using UnityEngine;

public class Parryable : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    private BossController boss;

    private void Awake()
    {
        boss = GetComponentInParent<BossController>();
    }

    public void Open(float duration)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(OpenRoutine(duration));
    }

    private System.Collections.IEnumerator OpenRoutine(float duration)
    {
        IsOpen = true;
        yield return new WaitForSeconds(duration);
        IsOpen = false;
    }

    public void OnParried(PlayerController player)
    {
        if (!IsOpen) return;
        IsOpen = false;
        boss?.OnParried(player);
    }
}
