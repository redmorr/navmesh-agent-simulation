using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class AgentCollision : MonoBehaviour
{
    [SerializeField] private CollisionData collisionData;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;
    private bool isBeingKnockedBack;
    private Rigidbody rb;

    public UnityAction OnKnockbackStarted;
    public UnityAction OnKnockbackEnded;

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
            rb.MovePosition(transform.position + Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackTimer / collisionData.KnockbackDuration) * Time.fixedDeltaTime);

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
