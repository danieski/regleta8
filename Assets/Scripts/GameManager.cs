using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public Camera mainCamera;
    public Transform player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player").transform;
        } else
        {
            Destroy(gameObject);
        }
    }
}
