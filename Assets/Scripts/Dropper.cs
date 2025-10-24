using UnityEngine;

public class Dropper : MonoBehaviour
{
    [SerializeField] float timeToWait = 2f;
    private MeshRenderer myMeshRenderer; 
    private Rigidbody myRigidBody;
    private bool hasActivated = false;
    
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myRigidBody = GetComponent<Rigidbody>();
        myMeshRenderer.enabled = false;
        myRigidBody.useGravity = false;
    }

    void Update()
    {
        if (!hasActivated && Time.time > timeToWait) 
        {
            myMeshRenderer.enabled = true;
            myRigidBody.useGravity = true;
            hasActivated = true;
            
            // Disable this component since it's no longer needed
            enabled = false;
        }
    }
}
