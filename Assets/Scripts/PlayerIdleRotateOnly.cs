using UnityEngine;

public class PlayerIdleRotateOnly : MonoBehaviour
{
    public float rotateSpeed = 20f;

    void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D) ||
                        Input.GetKey(KeyCode.UpArrow) ||
                        Input.GetKey(KeyCode.DownArrow) ||
                        Input.GetKey(KeyCode.LeftArrow) ||
                        Input.GetKey(KeyCode.RightArrow);
        if (!isMoving)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }
}
