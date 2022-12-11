using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private Agent agentPrefab;
    [SerializeField][Range(1, 10000)] private int agentPoolDefaultCapacity;
    [SerializeField][Range(1, 10000)] private int agentPoolMaxCapacity;
    [SerializeField][Range(0, 10000)] private int agentLimit;
    [SerializeField][MinMaxRange(0f, 10f)] private Vector2 randomSpawnRange;

    private Coroutine spawnRoutine;
    private ObjectPool<Agent> agentPool;
    private int spawnCount = 0;

    private void Awake()
    {
        agentPool = new ObjectPool<Agent>(CreateAgent, OnGetAgent, OnReleaseAgent, OnDestroyObjectAgent, false, agentPoolDefaultCapacity, agentPoolMaxCapacity);
    }

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private Agent CreateAgent()
    {
        Agent instance = Instantiate(agentPrefab, transform);
        instance.name = spawnCount.ToString();
        spawnCount++;
        instance.Disable += ReturnObjectToPool;
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetAgent(Agent instance)
    {
        NavMeshAgent navMeshAgent = instance.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        if (Arena.Instance.GetRandomPosition(out Vector3 position))
        {
            instance.transform.position = position + Vector3.up;
        }
        else
        {
            instance.transform.position = Vector3.up;
        }
        instance.gameObject.SetActive(true);
    }

    private void OnReleaseAgent(Agent instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroyObjectAgent(Agent instance)
    {
        Destroy(instance.gameObject);
    }

    private void ReturnObjectToPool(Agent instance)
    {
        agentPool.Release(instance);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(randomSpawnRange.x, randomSpawnRange.y));

            if (agentPool.CountActive < agentLimit)
                agentPool.Get();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f, 150f, 30f), string.Format("Total Pool Size: {0}", agentPool.CountAll));
        GUI.Label(new Rect(10f, 40f, 150f, 30f), string.Format("Currently Active: {0}", agentPool.CountActive));
    }
}
