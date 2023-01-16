using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class AgentCollision : MonoBehaviour
{
    [SerializeField] private CollisionData collisionData;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;
    private bool isBeingKnockedBack;
    private Rigidbody rb;

    public event Action OnKnockbackStarted;
    public event Action OnKnockbackEnded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isBeingKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (isBeingKnockedBack)
        {
            knockbackTimer += Time.fixedDeltaTime;
            float knokbackTimeFactor = knockbackTimer / collisionData.KnockbackDuration;
            rb.MovePosition(transform.position + Vector3.Lerp(knockbackVelocity, Vector3.zero, knokbackTimeFactor) * Time.fixedDeltaTime);

            if (knockbackTimer > collisionData.KnockbackDuration)
            {
                isBeingKnockedBack = false;
                OnKnockbackEnded?.Invoke();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IDamagable damagable))
        {
            damagable.ReceiveDamage(collisionData.DamageOtherOnContact);
            OnKnockbackStarted?.Invoke();
            Vector3 directionFromOtherCollider = (transform.position - collision.transform.position).normalized;
            knockbackVelocity = collisionData.KnockbackForce * directionFromOtherCollider;
            knockbackTimer = 0f;
            isBeingKnockedBack = true;
        }
    }
}
