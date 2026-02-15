using DG.Tweening;
using UnityEngine;

public class ChargedShoot : MonoBehaviour
{
    int speed = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("ttlClear", 3);
    }

    // Update is called once per frame
    void Update()
    {
     transform.localPosition = new Vector3(transform.position.x- Time.deltaTime * speed, transform.position.y, transform.position.z);

    }
    void ttlClear()
    {
        Destroy(gameObject);
    }

}
