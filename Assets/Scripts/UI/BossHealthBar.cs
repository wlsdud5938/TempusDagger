using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Health bossHealth;
    public Slider slider;

    private void Start()
    {
        if (bossHealth)
        {
            slider.maxValue = bossHealth.MaxHealth;
            slider.value = bossHealth.MaxHealth;
            bossHealth.onHealthChanged.AddListener((c, m) => slider.value = c);
        }
    }
}
