using System;
using TMPro;
using UnityEngine;

public class AgentDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup displayPanel;
    [SerializeField] private TextMeshProUGUI agentNameTextField;
    [SerializeField] private TextMeshProUGUI agentHealthPointsTextField;

    private AgentSelectionManager agentSelection;
    [SerializeField] private Agent selectedAgent;

    private void Awake()
    {
        agentSelection = FindObjectOfType<AgentSelectionManager>();
    }

    private void OnEnable()
    {
        agentSelection.OnAgentSelected += UpdateAgent;
        agentSelection.OnAgentDeselected += DiscardSelectedAgent;
    }

    private void UpdateAgent(Agent agent)
    {
        selectedAgent = agent;
        agentNameTextField.SetText(agent.Name);
        agentHealthPointsTextField.SetText(agent.HealthPoints.ToString());
        agent.OnHealthChanged += UpdateHealth;
        agent.OnDeath += DisplayAgentDead;
    }

    private void DisplayAgentDead(Agent agent)
    {
        UnsubscribeFromAgent(agent);
        agentHealthPointsTextField.SetText("DEAD");
    }

    private void DiscardSelectedAgent()
    {
        ResetPanelData();
        if (selectedAgent)
            UnsubscribeFromAgent(selectedAgent);
    }

    private void ResetPanelData()
    {
        agentNameTextField.SetText("-");
        agentHealthPointsTextField.SetText("-");
    }

    private void UnsubscribeFromAgent(Agent agent)
    {
        selectedAgent.OnHealthChanged -= UpdateHealth;
        selectedAgent.OnDeath -= DisplayAgentDead;
    }

    private void UpdateHealth(int newHealthPoints)
    {
        agentHealthPointsTextField.SetText(newHealthPoints.ToString());
    }

    private void OnDisable()
    {
        agentSelection.OnAgentSelected -= UpdateAgent;
        agentSelection.OnAgentDeselected -= DiscardSelectedAgent;
        if (selectedAgent)
            selectedAgent.OnDeath -= UnsubscribeFromAgent;
    }
}
