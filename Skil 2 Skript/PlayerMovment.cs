using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovment : MonoBehaviour
{
    public float speed = 0.2f;
    public float sideways = 0.2f;
    public float jump = 10f;

    public TextMeshProUGUI countText;

    private Rigidbody rb;
    private bool isGrounded = true; // Initially grounded

    public int count;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody>();
        SetCountText();
        CountKeep.LoadCount(); // Load count value when the game starts
    }

    void FixedUpdate()
    {
        if (Input.GetKey("w")) // Forward
        {
            transform.position += transform.forward * speed;
        }
        if (Input.GetKey("s")) // Backward
        {
            transform.position += -transform.forward * speed;
        }
        if (Input.GetKey("d")) // Right
        {
            transform.position += transform.right * sideways;
        }
        if (Input.GetKey("a")) // Left
        {
            transform.position += -transform.right * sideways;
        }
        if (Input.GetKey("e")) // Rotate right
        {
            transform.Rotate(new Vector3(0, 5, 0));
        }
        if (Input.GetKey("q")) // Rotate left
        {
            transform.Rotate(new Vector3(0, -5, 0));
        }

        if (transform.position.y <= -1)
        {
            Respawn();
        }
    }

    void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // Jump only when grounded
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
            isGrounded = false; // Prevent jumping continuously
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground")) // Check for ground
        {
            isGrounded = true; // Player is grounded when colliding with the ground
        }

        if (collision.collider.CompareTag("peningur"))
        {
            collision.collider.gameObject.SetActive(false);
            count += (collision.collider.CompareTag("peningur")) ? 1 : 5;
            SetCountText();
        }
        else if (collision.collider.CompareTag("hindrun"))
        {
            collision.collider.gameObject.SetActive(false);
            count -= 3;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Stig: " + count.ToString();

        if (count <= 0)
        {
            this.enabled = false;
            countText.text = "Dauður " + count.ToString() + " stigum";
            StartCoroutine(RespawnAfterDelay());
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }

    void Respawn()
    {
        SceneManager.LoadScene(0);
    }

    public void Byrja()
    {
        SceneManager.LoadScene(1);
    }

    public void Endurræsa()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Save count value when exiting the game
    private void OnApplicationQuit()
    {
        CountKeep.SaveCount();
    }

}
