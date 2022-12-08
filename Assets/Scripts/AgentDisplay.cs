using TMPro;
using UnityEngine;

public class AgentDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup displayPanel;
    [SerializeField] private TextMeshProUGUI agentNameTextField;
    [SerializeField] private TextMeshProUGUI agentHealthPointsTextField;

    private AgentSelectionManager agentSelection;
    private Agent selectedAgent;

    private void Awake()
    {
        agentSelection = FindObjectOfType<AgentSelectionManager>();
        HidePanel();
    }

    private void OnEnable()
    {
        agentSelection.OnAgentSelected += UpdateAgent;
        agentSelection.OnAgentDeselected += DiscardAgent;
    }

    private void UpdateAgent(Agent agent)
    {
        selectedAgent = agent;
        agentNameTextField.SetText(agent.Name);
        agentHealthPointsTextField.SetText(agent.HealthPoints.ToString());
        agent.OnHealthChanged += UpdateHealth;
        ShowPanel();
    }

    private void DiscardAgent()
    {
        HidePanel();
        selectedAgent.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth(int newHealthPoints)
    {
        agentHealthPointsTextField.SetText(newHealthPoints.ToString());
    }

    private void ShowPanel()
    {
        displayPanel.alpha = 1f;
        displayPanel.interactable = true;
    }

    private void HidePanel()
    {
        displayPanel.alpha = 0f;
        displayPanel.interactable = false;
    }

    private void OnDisable()
    {
        agentSelection.OnAgentSelected -= UpdateAgent;
        agentSelection.OnAgentDeselected -= DiscardAgent;
    }
}
