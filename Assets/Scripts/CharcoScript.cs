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
        StartCoroutine(TickRefresh());
        transform.DOScale(6,1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        

       // print(test.name);

    }
    IEnumerator TickRefresh()
    {
        yield return new WaitForSeconds(tick);
        collider.providesContacts = isActive;
        isActive= !isActive;
        StartCoroutine(TickRefresh());
        StartCoroutine(TTLDie());
        
        
    }
    IEnumerator TTLDie()
    {
        yield return new WaitForSeconds(TTL);
        Destroy(gameObject); 
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<HealthComponent>() != null && !other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("tnego a :" + other.name);    
    }
}
