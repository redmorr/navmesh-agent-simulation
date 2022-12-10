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
    [SerializeField] private int damageOnContact;

    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private int originalLayerID;

    private Vector3 knockbackForce;
    private float knockbackTime = 0f;

    private int fixedframes = 0;
    private int dynamicframes = 0;
    private bool bumped = false;

    public UnityAction<int> OnHealthChanged;
    public UnityAction<Agent> OnDeath;

    public delegate void OnDisableCallback(Agent Instance);
    public OnDisableCallback Disable;

    public string Name { get => gameObject.name; }
    public int HealthPoints { get; private set; }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalLayerID = gameObject.layer;
    }

    private void OnEnable()
    {
        HealthPoints = initialHealthPoints;
        gameObject.layer = originalLayerID;
    }

    private void Update()
    {
        if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= 0.2f)
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
            rb.MovePosition(transform.position + Vector3.Lerp(knockbackForce, Vector3.zero, knockbackTime) *Time.fixedDeltaTime);
            if (knockbackTime > 1f)
            {
                bumped = false;
                navMeshAgent.enabled = true;
                navMeshAgent.updatePosition = true;
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
            Disable?.Invoke(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(string.Format("Collision In: {0} {1} {2}", gameObject.name, fixedframes, dynamicframes));
        Vector3 velocity1 = navMeshAgent.velocity;
        navMeshAgent.updatePosition = false;
        navMeshAgent.enabled = false;
        knockbackForce = -velocity1;
        knockbackTime = 0f;

        bumped = true;
    }
}
