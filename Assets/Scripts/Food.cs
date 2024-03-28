using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public static Food instance;
    
    public Collider2D foodArea; // get food spawn area
    private Snake snake; // get small snake

    private void Awake() { snake = FindObjectOfType<Snake>(); } // find
    private void Start() { RandomizePosition(); } // random food spawn

    public void RandomizePosition()
    {   Bounds bounds = foodArea.bounds; // find food spawn bounds

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        float x = Random.Range(bounds.min.x, bounds.max.x); // can use Mathf.RoundToInt() to make it blockful
        float y = Random.Range(bounds.min.y, bounds.max.y);

        // Prevent the food from spawning on the snake
        while (snake.Occupies((int) x, (int)y)) { 
            x++; if (x > bounds.max.x) { x = bounds.min.x; y++; if (y > bounds.max.y) { y = bounds.min.y; } }// can use Mathf.RoundToInt()
        }
        transform.position = new Vector2(x, y);
    }

    
    private void OnTriggerEnter2D(Collider2D other){ RandomizePosition(); }
}
