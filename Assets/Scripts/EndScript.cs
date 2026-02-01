using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EndCorutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator EndCorutine()
    {
		yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Start");
	}
}
