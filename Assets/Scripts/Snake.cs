using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform bodyPartPrefab; // snake body
    public Collider2D foodPrefab;
    public Vector2Int direction = Vector2Int.right; // snake walling direction
    public float hold, hold2, speed = 28f; // bigger the faster
    public float speedAdder = 1.009f; // speed timer
    public int initialSize = 1; 
    public bool wallTP = true; // true than teleportation, false than die
    public bool dieIsRebody = false; // true is nobody in replay, false is replay with full body
    public bool dieCutBody = false; // have bugs
    public Camera cam;

    public List<Transform> bodyParts = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;

    private Quaternion camRotation; // unused code
    private void Start() { ResetState(); camRotation = transform.rotation; cam.orthographicSize = 19; }

    private void Update()
    {   if (direction.x != 0f){ // facing x direction can move up down
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { input = Vector2Int.up; } // snake head up
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) { input = Vector2Int.down; } // snake head down
        }else if (direction.y != 0f) { // facing y direction can move left right
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { input = Vector2Int.right; } // snake head right
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { input = Vector2Int.left; }} // snake head left
        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.End)) { ResetState(); } // kill itself
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) { reCam(); } // unpoison
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.L)) { Grow(); } // grow snake body
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Tilde)) { if (wallTP) { wallTP = false; } else { wallTP = true; } } // open or close wall teleportation
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) { speed += 66 * Time.deltaTime; } // add speed
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) { speed -= 66 * Time.deltaTime; } // slower speed, tiny bugs
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Home)) { if (Time.timeScale!=1) { Time.timeScale=1; } else { Time.timeScale=0; } } // pause
        if (Input.GetKeyDown(KeyCode.C)) { if (dieIsRebody) { dieIsRebody = false; } else { dieIsRebody = true; } } // open or close dieIsRebody, have bugs
        if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.M)) { Ungrow(); } // ungrow snake body
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.O)) { LeaveBehind(); } // leave or egg out body
        if (Input.GetKeyDown(KeyCode.Z)) { hold = speed; speed = 9; } // slow speed
        if (Input.GetKeyUp(KeyCode.Z)) { speed = hold; } // get back speed
        //if (Input.GetKeyDown(KeyCode.LeftShift)) { hold2 = speed; speed = 13; } // slow speed
        //if (Input.GetKeyUp(KeyCode.LeftShift)) { speed = hold2; } // get back speed
        if (Input.GetKey(KeyCode.RightBracket) && cam.orthographicSize<66) { cam.orthographicSize += 66 * Time.deltaTime; } // bigger cam
        if (Input.GetKey(KeyCode.LeftBracket) && cam.orthographicSize>5) { cam.orthographicSize -= 66 * Time.deltaTime; } // smaller cam
        if (Input.GetKeyDown(KeyCode.F)) { Instantiate(foodPrefab); } // food food // have bugs
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); }
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) { SceneManager.LoadScene(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) { SceneManager.LoadScene(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) { SceneManager.LoadScene(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) { SceneManager.LoadScene(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) { SceneManager.LoadScene(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) { SceneManager.LoadScene(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) { SceneManager.LoadScene(6); }
        //if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad2)) { SceneManager.LoadScene(0); }
    }

    private void FixedUpdate()
    {   if (Time.time < nextUpdate) { return; } // Wait til next update
        if (input != Vector2Int.zero) { direction = input; } // Set the new direction based on the input

        for (int i = bodyParts.Count - 1; i > 0; i--){ bodyParts[i].position = bodyParts[i - 1].position; } //each bodyPart follows reverse order set previous position or stack on top of each other.

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);
        nextUpdate = Time.time + (1f / (speed * speedAdder)); // Set the next update time based on the speed
    }
    public void LeaveBehind(){ bodyParts.Remove(bodyParts[bodyParts.Count-1]); }
    public void Grow()
    {   Transform bodyPart = Instantiate(bodyPartPrefab);
        bodyPart.position = bodyParts[bodyParts.Count - 1].position;
        bodyParts.Add(bodyPart);
    }
    public void Ungrow(){ Destroy(bodyParts[bodyParts.Count].gameObject); bodyParts.Remove(bodyParts[bodyParts.Count+1]);} // bugs
    public void ResetState()
    {   if (Random.Range(0, 2)==0) { direction = Vector2Int.right; } else { direction = Vector2Int.left; } // ran go direction
        //direction = Random.(Vector2Int.right, Vector2Int.left); // ran go direction

        transform.position = Vector3.zero;

        for (int i = 1; i < bodyParts.Count; i++) { Destroy(bodyParts[i].gameObject); } // start at 1 to not destroy head

        if (dieIsRebody) { bodyParts.Clear(); bodyParts.Add(transform);} // no more body but add back head

        for (int i = 0; i < initialSize - 1; i++) { Grow(); } // -1 (head is in list)

        reCam(); // no poison cam 
    }
    public bool Occupies(int x, int y)
    { foreach (Transform bodyPart in bodyParts) { if (bodyPart.position.x == x && bodyPart.position.y == y) { return true; } } return false; }// can use Mathf.RoundToInt()
    private void OnTriggerEnter2D(Collider2D other)
    {   if (other.gameObject.CompareTag("Food")) { Grow(); }
        else if (other.gameObject.CompareTag("Apple")) { Grow(); Grow(); } //bugs
        else if (other.gameObject.CompareTag("Poison")) { Grow(); poisonCam(Random.RandomRange(2,6)); }
        else if (other.gameObject.CompareTag("Obstacle")) { ResetState(); } // maybe I will updata new things
        else if (other.gameObject.CompareTag("Wall")) {if (wallTP) { Traverse(other.transform); } else { ResetState(); }}}
    private void Traverse(Transform wall)
    {   Vector3 position = transform.position;
        if (direction.x != 0f) { position.x = -wall.position.x + direction.x; } // can use Mathf.RoundToInt()
        else if (direction.y != 0f) { position.y = -wall.position.y + direction.y; } // can use Mathf.RoundToInt()
        transform.position = position;}
    private void poisonCam(float rotate) { transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, rotate); }
    private void reCam() { transform.eulerAngles = new Vector3(0, 0, 0); }
    }

