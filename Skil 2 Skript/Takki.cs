using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Takki : MonoBehaviour
{
    public TextMeshProUGUI texti;

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            PlayerMovment playerMovement = FindObjectOfType<PlayerMovment>();
            if (playerMovement != null)
            {
                texti.text = "Lokastig " + playerMovement.count.ToString();
            }
            else
            {
                Debug.LogError("PlayerMovment script not found in the scene!");
            }
        }
    }

    public void Byrja()
    {
        SceneManager.LoadScene(1);
    }

    public void Endir()
    {
        SceneManager.LoadScene(0);
    }
}
