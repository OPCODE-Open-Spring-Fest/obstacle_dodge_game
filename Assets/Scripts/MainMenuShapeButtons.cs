using UnityEngine;

public class MainMenuShapeButtons : MonoBehaviour
{
    public void SetCube() { ShapeChoice.selectedShape = 0; }
    public void SetSphere() { ShapeChoice.selectedShape = 1; }
    public void SetCapsule() { ShapeChoice.selectedShape = 2; }
}
