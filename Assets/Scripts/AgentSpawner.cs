using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private List<Agent> activeAgents;

    [SerializeField][Range(0, 100)] private int agentLimit;
    [SerializeField][Range(0.1f, 10f)] private float agentSpawnDelayInSeconds;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(agentSpawnDelayInSeconds);

            if (activeAgents.Count < agentLimit && NavMesh.SamplePosition(GetRandomPoint(), out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                GameObject agentGameObject = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity);
                Agent agent = agentGameObject.GetComponent<Agent>();
                NavMeshAgent navMeshAgent = agentGameObject.GetComponent<NavMeshAgent>();
                navMeshAgent.Warp(hit.position);
                agent.OnDeath += RemoveFromActiveAgents;
                activeAgents.Add(agent);
            }
        }
    }

    private void RemoveFromActiveAgents(Agent agent)
    {
        agent.OnDeath -= RemoveFromActiveAgents;
        activeAgents.Remove(agent);
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
}
