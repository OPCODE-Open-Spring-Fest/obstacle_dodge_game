using UnityEngine;

public class Scorer : MonoBehaviour
{
    private int hits = 0;
    
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Hit")) return;
        
        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");
    }
}
