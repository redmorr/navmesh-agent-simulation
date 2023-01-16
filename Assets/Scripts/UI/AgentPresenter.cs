using TMPro;
using UnityEngine;

public class AgentPresenter : MonoBehaviour
{
    [SerializeField] private LayerMask agentLayer;
    [SerializeField] private LayerMask selectedAgentLayer;
    [SerializeField] private CanvasGroup displayPanel;
    [SerializeField] private AgentDisplayStrings displayStrings;

    [Header("Name")]
    [SerializeField] private TextMeshProUGUI agentNameLabel;
    [SerializeField] private TextMeshProUGUI agentNameValue;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI agentHealthLabel;
    [SerializeField] private TextMeshProUGUI agentHealthValue;

    private Agent selectedAgent;

    private void Awake()
    {
        agentNameLabel.SetText(displayStrings.AgentNameLabel);
        agentHealthLabel.SetText(displayStrings.AgentHealthLabel);
        agentNameValue.SetText(displayStrings.NoValue);
        agentHealthValue.SetText(displayStrings.NoValue);
    }

    public void Select(Agent agent)
    {
        DiscardSelectionIfSelected();

        selectedAgent = agent;

        selectedAgent.OnHealthChanged += UpdateHealth;
        selectedAgent.OnDeath += DisplayAgentDead;

        ChangeLayer(selectedAgent, selectedAgentLayer);

        agentNameValue.SetText(selectedAgent.Name);
        agentHealthValue.SetText(selectedAgent.HealthPoints.ToString());
    }

    public void DiscardSelectionIfSelected()
    {
        if (selectedAgent != null)
        {
            selectedAgent.OnHealthChanged -= UpdateHealth;
            selectedAgent.OnDeath -= DisplayAgentDead;

            ChangeLayer(selectedAgent, agentLayer);
            
            agentNameValue.SetText(displayStrings.NoValue);
            agentHealthValue.SetText(displayStrings.NoValue);

            selectedAgent = null;
        }
    }

    private void DisplayAgentDead(Agent agent)
    {
        agent.OnHealthChanged -= UpdateHealth;
        agent.OnDeath -= DisplayAgentDead;
        agentHealthValue.SetText(displayStrings.AgentDead);
    }

    private void UpdateHealth(int newValue)
    {
        agentHealthValue.SetText(newValue.ToString());
    }

    private void ChangeLayer(Agent selectedAgent, LayerMask agentLayer)
    {
        selectedAgent.gameObject.layer = LayerMaskToInt(agentLayer);
    }

    private int LayerMaskToInt(LayerMask layerMask) => (int)Mathf.Log(layerMask.value, 2);
}
