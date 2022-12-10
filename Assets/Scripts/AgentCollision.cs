using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class AgentCollision : MonoBehaviour
{
    [SerializeField] private int damageOtherOnContact;
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private float knockbackForce = 2f;

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
            rb.MovePosition(transform.position + Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackTimer / knockbackDuration) * Time.fixedDeltaTime);
            if (knockbackTimer > knockbackDuration)
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
            damagable.ReceiveDamage(damageOtherOnContact);
            OnKnockbackStarted?.Invoke();
            knockbackVelocity = knockbackForce * (transform.position - collision.transform.position).normalized;
            knockbackTimer = 0f;
            isBeingKnockedBack = true;
        }
    }
}
