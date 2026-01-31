using UnityEngine;

public class EnemyBasicScript : MonoBehaviour
{
    public GameObject targetPosition;
    public CharacterController controller;
    public float speed = 3f;
    void Start()
    {
        
    }
    void Update()
    {
        Vector3 direction = (targetPosition.transform.position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);
    }
    
}
