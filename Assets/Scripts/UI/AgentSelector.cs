using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AgentSelector : MonoBehaviour
{
    [SerializeField] private AgentPresenter agentPresenter;
    [SerializeField] private LayerMask agentLayer;
    [SerializeField] private LayerMask selectedAgentLayer;

    private PlayerControls inputActions;
    private bool isPointerOverUI;

    public event Action<Agent> OnAgentSelected;
    public event Action OnAgentDeselected;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Select.performed += DetectAgent;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Select.performed -= DetectAgent;
        inputActions.Disable();
    }

    private void Update()
    {
        // IsPointerOverGameObject() procudes a warning when called in Input Actions callback. Read the docs below:
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/UISupport.html#handling-ambiguities-for-pointer-type-input
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    private void DetectAgent(InputAction.CallbackContext _)
    {
        if (!isPointerOverUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputActions.Player.Mouse.ReadValue<Vector2>());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, agentLayer, QueryTriggerInteraction.Collide)
                && hit.collider.TryGetComponent(out Agent agent))
            {
                agentPresenter.Select(agent);
            }
            else
            {
                agentPresenter.DiscardSelectionIfSelected();
            }
        }
    }
}
