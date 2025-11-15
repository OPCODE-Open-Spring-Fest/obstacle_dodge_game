using UnityEngine;

public class PlayerIdleAnimate : MonoBehaviour
{
    public float pulseAmount = 0.1f;
    public float pulseSpeed = 2f;
    public float rotateSpeed = 20f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

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
            float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale + Vector3.one * scaleOffset;
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = originalScale;
        }
    }
}
