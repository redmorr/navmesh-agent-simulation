using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Arena : Singleton<Arena>
{
    private Vector3 GetRandomPoint()
    {
        Vector3 center = Vector3.zero;
        float width = 20f;
        float depth = 20f;

        NavMesh.SamplePosition(Vector3.zero, out NavMeshHit _, 1.0f, NavMesh.AllAreas);

        center.x += Random.Range(-0.5f, 0.5f) * width;
        center.z += Random.Range(-0.5f, 0.5f) * depth;
        return center;
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


