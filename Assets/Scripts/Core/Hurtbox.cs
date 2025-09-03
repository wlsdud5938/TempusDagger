using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}
