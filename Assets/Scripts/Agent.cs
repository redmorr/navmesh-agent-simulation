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
    private int originalLayerID;

    public UnityAction<int> OnHealthChanged;
    public UnityAction<Agent> OnDeath;

    public delegate void OnDisableCallback(Agent Instance);
    public OnDisableCallback Disable;

    public string Name { get => gameObject.name; }
    public int HealthPoints { get; private set; }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        originalLayerID = gameObject.layer;
    }

    private void OnEnable()
    {
        HealthPoints = initialHealthPoints;
        gameObject.layer = originalLayerID;
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance <= 0.2f)
        {
            if (NavMesh.SamplePosition(GetRandomPoint(), out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Agent damagable))
        {
            damagable.ReceiveDamage(damageOnContact);
        }
    }
}
