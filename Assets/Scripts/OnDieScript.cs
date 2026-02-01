using UnityEngine;
using UnityEngine.SceneManagement;

public class OnDieScript : MonoBehaviour
{
	public void changeSceneOnDie()
	{
		SceneManager.LoadScene("Death");
	}
	
}
