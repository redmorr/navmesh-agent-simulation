using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Agent : MonoBehaviour, IDamagable
{
    [SerializeField] private string agentName;
    [SerializeField] private int initialHealthPoints;
    [SerializeField] private int damageOnContact;
    [SerializeField] private Material selectionMaterial;

    private NavMeshAgent navMeshAgent;
    private MeshRenderer mesh;
    private Material originalMaterial;

    public UnityAction<int> OnHealthChanged;
    public UnityAction<Agent> OnDeath;

    public delegate void OnDisableCallback(Agent Instance);
    public OnDisableCallback Disable;

    public string Name { get => agentName; private set => agentName = value; }
    public int HealthPoints { get; private set; }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mesh = GetComponent<MeshRenderer>();
        originalMaterial = mesh.material;
    }

    private void OnEnable()
    {
        HealthPoints = initialHealthPoints;
    }

    private void OnDisable()
    {
        mesh.material = originalMaterial;
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

    public void HighlightSelected()
    {
        mesh.material = selectionMaterial;
    }

    public void ResetHighlight()
    {
        mesh.material = originalMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Agent damagable))
        {
            damagable.ReceiveDamage(damageOnContact);
        }
    }
}
