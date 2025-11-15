using UnityEngine;

public class ApplySimpleShape : MonoBehaviour
{
    private MeshFilter meshFilter;

    public Mesh cubeMesh;
    public Mesh sphereMesh;
    public Mesh capsuleMesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found on Player!");
            return;
        }

        switch (ShapeChoice.selectedShape)
        {
            case 0:
                meshFilter.mesh = cubeMesh;
                break;

            case 1:
                meshFilter.mesh = sphereMesh;
                break;

            case 2:
                meshFilter.mesh = capsuleMesh;
                break;
        }
    }
}
