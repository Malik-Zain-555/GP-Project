using UnityEngine;
using UnityEngine.Tilemaps;

public class Collectable_Items : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ScoreManager.instance.Add(1);
            Destroy(gameObject); // destroy only THIS apple
        }
    }
}
