using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Selectable : MonoBehaviour
{
    [SerializeField] private Material selectionMaterial;

    private MeshRenderer mesh;
    private Material originalMaterial;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        originalMaterial = mesh.material;
    }

    public void HighlightSelected()
    {
        mesh.material = selectionMaterial;
    }

    public void ResetHighlight()
    {
        mesh.material = originalMaterial;
    }
}
