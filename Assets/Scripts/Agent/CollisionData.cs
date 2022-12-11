using UnityEngine;

[CreateAssetMenu]
public class CollisionData : ScriptableObject
{
    [Range(0, 3)] public int DamageOtherOnContact;
    [Range(0.1f, 2f)] public float KnockbackDuration;
    [Range(0.1f, 30f)] public float KnockbackForce;
}
