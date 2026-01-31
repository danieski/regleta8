using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Kill", lifetime);
    }

    void Kill()
    {
        Destroy(gameObject);
    }
}
