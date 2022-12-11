using TMPro;
using UnityEngine;

public class AgentDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup displayPanel;
    [SerializeField] private AgentDisplayStrings displayStrings;
    
    [Header("Name")]
    [SerializeField] private TextMeshProUGUI agentNameLabelText;
    [SerializeField] private TextMeshProUGUI agentNameValueText;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI agentHealthLabelText;
    [SerializeField] private TextMeshProUGUI agentHealthValueText;

    private AgentSelectionManager selectionManager;
    private Agent selectedAgent;

    private void Awake()
    {
        selectionManager = FindObjectOfType<AgentSelectionManager>();
        InitTextFields();
    }

    private void InitTextFields()
    {
        agentNameLabelText.SetText(displayStrings.AgentNameLabel);
        agentNameValueText.SetText(displayStrings.NoValue);

        agentHealthLabelText.SetText(displayStrings.AgentHealthLabel);
        agentHealthValueText.SetText(displayStrings.NoValue);
    }

    private void OnEnable()
    {
        selectionManager.OnAgentSelected += UpdateAgent;
        selectionManager.OnAgentDeselected += DiscardSelectedAgent;
    }

    private void UpdateAgent(Agent agent)
    {
        selectedAgent = agent;
        agentNameValueText.SetText(agent.Name);
        agentHealthValueText.SetText(agent.HealthPoints.ToString());
        agent.OnHealthChanged += UpdateHealth;
        agent.OnDeath += DisplayAgentDead;
    }

    private void DisplayAgentDead(Agent agent)
    {
        UnsubscribeFromAgent(agent);
        agentHealthValueText.SetText(displayStrings.AgentDead);
    }

    private void DiscardSelectedAgent()
    {
        ResetPanelData();
        if (selectedAgent)
            UnsubscribeFromAgent(selectedAgent);
    }

    private void ResetPanelData()
    {
        agentNameValueText.SetText(displayStrings.NoValue);
        agentHealthValueText.SetText(displayStrings.NoValue);
    }

    private void UnsubscribeFromAgent(Agent agent)
    {
        selectedAgent.OnHealthChanged -= UpdateHealth;
        selectedAgent.OnDeath -= DisplayAgentDead;
    }

    private void UpdateHealth(int newHealthPoints)
    {
        agentHealthValueText.SetText(newHealthPoints.ToString());
    }

    private void OnDisable()
    {
        selectionManager.OnAgentSelected -= UpdateAgent;
        selectionManager.OnAgentDeselected -= DiscardSelectedAgent;
        if (selectedAgent)
            selectedAgent.OnDeath -= UnsubscribeFromAgent;
    }
}
