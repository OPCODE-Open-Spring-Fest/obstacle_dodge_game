using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float yRotate = 1;
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, yRotate, 0);
    }
}
