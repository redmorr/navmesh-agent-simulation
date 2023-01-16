using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AgentCollision))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour, IDamagable
{
    [SerializeField] private int initialHealthPoints;
    [SerializeField][Range(0f, 1f)] private float despawnDuration;
    [SerializeField] private Color colorOnDeath;

    private AgentCollision agentCollision;
    private MeshRenderer meshRenderer;
    private NavMeshAgent navMeshAgent;
    private Color originalColor;
    private int originalLayerID;

    public delegate void OnDisableCallback(Agent Instance);

    public event Action<int> OnHealthChanged;

    public OnDisableCallback Disable;
    public event Action<Agent> OnDeath;

    public int HealthPoints { get; private set; }
    public string Name { get => gameObject.name; }

    private void Awake()
    {
        agentCollision = GetComponent<AgentCollision>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();

        originalLayerID = gameObject.layer;
        originalColor = meshRenderer.material.color;

        agentCollision.OnKnockbackStarted += DisableNavmeshAgent;
        agentCollision.OnKnockbackEnded += EnableNavmeshAgent;
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
                navMeshAgent.SetDestination(position);
            else
                Disable?.Invoke(this);
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
        meshRenderer.material.color = colorOnDeath;
        yield return new WaitForSeconds(despawnDuration);
        Disable?.Invoke(this);
    }

    private void DisableNavmeshAgent()
    {
        navMeshAgent.enabled = false;
    }

    private void EnableNavmeshAgent()
    {
        navMeshAgent.enabled = true;
    }
}
