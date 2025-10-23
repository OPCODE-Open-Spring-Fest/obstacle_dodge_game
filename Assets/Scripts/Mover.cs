using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    private Vector3 movement;
    
    void Start()
    {
        PrintInstructions();
    }

    void Update()
    {
        MovePlayer();
    }

    void PrintInstructions() {
        Debug.Log("WELCOME TO THE GAME!");
        Debug.Log("W A S D!");
    }
    
    void MovePlayer() {
        // Cache input values to avoid multiple calls
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Only move if there's input
        if (horizontal != 0f || vertical != 0f)
        {
            movement.Set(horizontal * Time.deltaTime * moveSpeed, 0f, vertical * Time.deltaTime * moveSpeed);
            transform.Translate(movement);
        }
    }
}
