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
    [SerializeField] private int healthPoints;
    [SerializeField] private int damageOnContact;
    [SerializeField] private Material selectionMaterial;

    private NavMeshAgent navMeshAgent;
    private MeshRenderer mesh;
    private Material originalMaterial;

    public UnityAction<int> OnHealthChanged;

    public string Name { get => agentName; private set => agentName = value; }
    public int HealthPoints { get => healthPoints; private set => healthPoints = value; }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mesh = GetComponent<MeshRenderer>();
        originalMaterial = mesh.material;
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
        float width = 10f;
        float depth = 10f;

        center.x += Random.Range(-0.5f, 0.5f) * width;
        center.z += Random.Range(-0.5f, 0.5f) * depth;
        return center;
    }

    public void ApplyDamage(int amount)
    {
        healthPoints -= amount;
        OnHealthChanged?.Invoke(healthPoints);
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
            damagable.ApplyDamage(damageOnContact);
        }
    }
}
