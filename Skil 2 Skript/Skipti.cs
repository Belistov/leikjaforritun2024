using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skipti : MonoBehaviour
{
    void Start()
    {
        Debug.Log("byrja");
    }
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        StartCoroutine(Bida());    
    }
    IEnumerator Bida()
    {
        yield return new WaitForSeconds(3);
        Endurræsa();
    }
    public void Endurræsa()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);//næsta sena
    }

}
