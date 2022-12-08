using UnityEngine;
using UnityEngine.InputSystem;

public class AgentSelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask selectableLayers;

    private PlayerControls inputActions;
    private Selectable currentlySelected;

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

    private void SelectAgent(InputAction.CallbackContext obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputActions.Player.Mouse.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayers, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.TryGetComponent(out Selectable selectable))
            {
                if (currentlySelected != null)
                    currentlySelected.ResetHighlight();
                selectable.HighlightSelected();
                currentlySelected = selectable;
            }
        }
        else
        {
            if (currentlySelected != null)
                currentlySelected.ResetHighlight();
        }
    }
}
