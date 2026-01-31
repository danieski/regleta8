using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int velocity = 1;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * velocity,ForceMode.Impulse);
    }
    void Update()
    {
        
    }
}
