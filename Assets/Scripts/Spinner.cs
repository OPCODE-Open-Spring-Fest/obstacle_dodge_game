using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float yRotate = 1f;
    
    void Update()
    {
        transform.Rotate(0, yRotate * Time.deltaTime * 60f, 0);
    }
}
