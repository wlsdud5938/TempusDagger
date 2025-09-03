using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Phase", fileName = "BossPhase")]
public class BossPhase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public BossPattern pattern;
        [Range(0f, 1f)] public float weight = 1f;
    }

    public List<Entry> patterns = new();

    public BossPattern PickRandom()
    {
        if (patterns == null || patterns.Count == 0) return null;
        float total = 0f;
        foreach (var e in patterns) total += Mathf.Max(0f, e.weight);
        float r = UnityEngine.Random.Range(0f, total);
        foreach (var e in patterns)
        {
            float w = Mathf.Max(0f, e.weight);
            if (r < w) return e.pattern;
            r -= w;
        }
        return patterns[0].pattern;
    }
}
