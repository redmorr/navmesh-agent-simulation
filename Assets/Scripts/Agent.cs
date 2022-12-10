using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class Agent : MonoBehaviour, IDamagable
{
    //[SerializeField] private string agentName;
    [SerializeField] private int initialHealthPoints;

    private MeshRenderer meshRenderer;
    private NavMeshAgent navMeshAgent;
    private AgentCollision agentCollision;
    private int originalLayerID;
    private Color originalColor;

    public delegate void OnDisableCallback(Agent Instance);
    public OnDisableCallback Disable;
    public UnityAction<int> OnHealthChanged;
    public UnityAction<Agent> OnDeath;

    public string Name { get => gameObject.name; }
    public int HealthPoints { get; private set; }

    private void Awake()
    {
        agentCollision = GetComponent<AgentCollision>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        navMeshAgent = GetComponent<NavMeshAgent>();

        originalLayerID = gameObject.layer;
        agentCollision.OnKnockbackStarted += DisableNavmeshAgent;
        agentCollision.OnKnockbackEnded += EnableNavmeshAgent;
    }

    private void DisableNavmeshAgent()
    {
        navMeshAgent.enabled = false;
    }

    private void EnableNavmeshAgent()
    {
        if (Arena.Instance.IsAgentOnNavMesh(transform.position))
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            OnDeath?.Invoke(this);
            Disable?.Invoke(this);
        }
    }

    private void OnEnable()
    {
        meshRenderer.material.color = originalColor;
        HealthPoints = initialHealthPoints;
        gameObject.layer = originalLayerID;
        navMeshAgent.enabled = true;
    }

    private void Update()
    {
        if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.2f)
        {
            if (Arena.Instance.GetRandomPosition(out Vector3 position))
            {
                navMeshAgent.SetDestination(position);
            }
            else
            {
                Disable?.Invoke(this);
            }
        }
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
}
