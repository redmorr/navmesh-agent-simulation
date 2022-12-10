using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AgentSelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask agentLayer;
    [SerializeField] private LayerMask selectedAgentLayer;

    private PlayerControls inputActions;
    [SerializeField] private Agent currentlySelectedAgent;

    public UnityAction<Agent> OnAgentSelected;
    public UnityAction OnAgentDeselected;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Select.performed += SelectAgent;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Select.performed -= SelectAgent;
        inputActions.Disable();
    }

    private void SelectAgent(InputAction.CallbackContext _)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputActions.Player.Mouse.ReadValue<Vector2>());

        // TODO: Move IsPointerOverGameObject() to update because when its in input action callback leads to a warning. Explanation below:
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/UISupport.html#handling-ambiguities-for-pointer-type-input
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, agentLayer, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.TryGetComponent(out Agent agent))
                {
                    if (currentlySelectedAgent != null)
                        Deselect();
                    Select(agent);
                }
            }
            else
            {
                if (currentlySelectedAgent != null)
                    Deselect();
                else
                    OnAgentDeselected?.Invoke();
            }
        }
    }

    private void Select(Agent agent)
    {
        currentlySelectedAgent = agent;
        currentlySelectedAgent.gameObject.layer = LayerMaskToInt(selectedAgentLayer);
        OnAgentSelected?.Invoke(currentlySelectedAgent);
    }

    private void Deselect()
    {
        currentlySelectedAgent.gameObject.layer = LayerMaskToInt(agentLayer);
        OnAgentDeselected?.Invoke();
        currentlySelectedAgent = null;
    }

    private int LayerMaskToInt(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask.value, 2);
    }
}
