using UnityEngine;

[CreateAssetMenu]
public class AgentDisplayStrings : ScriptableObject
{
    [Header("Labels")]
    public string AgentNameLabel;
    public string AgentHealthLabel;

    [Header("Values")]
    public string NoValue;
    public string AgentDead;
}
