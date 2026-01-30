using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public Camera mainCamera;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            mainCamera = Camera.main;
        } else
        {
            Destroy(gameObject);
        }
    }
}
