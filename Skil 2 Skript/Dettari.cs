using UnityEngine;

public class Dettari : MonoBehaviour
{
    public float amplitude = 1.0f; // The maximum distance the object will move vertically
    public float speed = 1.0f; // The speed at which the object will move

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Save the initial position of the object
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

        // Update the object's position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
