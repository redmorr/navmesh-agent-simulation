using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Agent : MonoBehaviour, IDamagable
{
    //[SerializeField] private string agentName;
    [SerializeField] private int initialHealthPoints;
    [SerializeField] private int damageOtherOnContact;

    private MeshRenderer meshRenderer;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private int originalLayerID;

    private Color originalColor;

    private Vector3 knockbackForce;
    private float knockbackTime = 0f;

    private bool bumped = false;

    public UnityAction<int> OnHealthChanged;
    public UnityAction<Agent> OnDeath;

    public delegate void OnDisableCallback(Agent Instance);
    public OnDisableCallback Disable;

    public string Name { get => gameObject.name; }
    public int HealthPoints { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalLayerID = gameObject.layer;
    }

    private void OnEnable()
    {
        meshRenderer.material.color = originalColor;
        HealthPoints = initialHealthPoints;
        gameObject.layer = originalLayerID;
        navMeshAgent.enabled = true;
        bumped = false;
    }

    private void Update()
    {
        if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.2f)
        {
            if (NavMesh.SamplePosition(GetRandomPoint(), out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }
    private void FixedUpdate()
    {
        if (bumped)
        {
            knockbackTime += Time.fixedDeltaTime;
            rb.MovePosition(transform.position + Vector3.Lerp(knockbackForce, Vector3.zero, knockbackTime) * Time.fixedDeltaTime);
            if (knockbackTime > 1f)
            {
                bumped = false;
                navMeshAgent.enabled = true;
            }
        }
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 center = Vector3.zero;
        float width = 20f;
        float depth = 20f;

        center.x += Random.Range(-0.5f, 0.5f) * width;
        center.z += Random.Range(-0.5f, 0.5f) * depth;
        return center;
    }

    public void ReceiveDamage(int amount)
    {
        HealthPoints -= amount;
        OnHealthChanged?.Invoke(HealthPoints);
        if (HealthPoints <= 0)
        {
            OnDeath?.Invoke(this);
            StartCoroutine(Despawn());
        }
    }

    private IEnumerator Despawn()
    {
        meshRenderer.material.color = new Color(1f, 0f, 0f, 1f);
        yield return new WaitForSeconds(0.6f);

        Disable?.Invoke(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IDamagable damagable))
        {
            damagable.ReceiveDamage(damageOtherOnContact);

            Vector3 velocity1 = navMeshAgent.velocity;
            navMeshAgent.enabled = false;
            knockbackForce = 2f * (transform.position - collision.transform.position);
            knockbackTime = 0f;

            bumped = true;

        }
    }
}
