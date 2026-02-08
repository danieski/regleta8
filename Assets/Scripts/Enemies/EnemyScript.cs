using UnityEngine;

public class EnemyBasicScript : MonoBehaviour
{
    public GameObject targetPosition;
    public CharacterController controller;
    public float speed = 100f;
    void Start()
    {
        if (targetPosition == null)
        {
            targetPosition = GameManager.instance.player.gameObject;
        }
    }
    void Update()
    {
        Vector3 direction = (targetPosition.transform.position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);
    }
    
}
