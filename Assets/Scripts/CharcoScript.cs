using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CharcoScript : MonoBehaviour
{
    public float tick = 10.0f;
    public int damage = 1;
    public int TTL = 10;
    GameObject test;
    Collider collider;
    bool isActive;
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
        transform.DOScale(6,1.3f);
        Invoke("TTLDie", TTL);
    }
    private void TTLDie()
    {
        Destroy(gameObject); 
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<HealthComponent>() != null && !other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().TakeDamage(damage);
        }
    }
}
