using UnityEngine;
using UnityEngine.AI;

public class Arena : Singleton<Arena>
{
    private float boundsX;
    private float boundsZ;
    private Vector3 center;

    protected override void Awake()
    {
        base.Awake();
        center = transform.position;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;

        boundsX = transform.localScale.x * bounds.size.x;
        boundsZ = transform.localScale.z * bounds.size.z;
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = center;

        randomPoint.x += Random.Range(-0.5f, 0.5f) * boundsX;
        randomPoint.z += Random.Range(-0.5f, 0.5f) * boundsZ;
        return randomPoint;
    }

    public bool GetRandomPosition(out Vector3 position)
    {
        bool success = NavMesh.SamplePosition(GetRandomPoint(), out NavMeshHit hit, 2.0f, NavMesh.AllAreas);

        if (success)
            position = hit.position;
        else
            position = Vector3.zero;

        return success;
    }

    public bool IsAgentOnNavMesh(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit _, 1.0f, NavMesh.AllAreas);
    }
}
