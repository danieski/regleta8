using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public Vector3 LastPlayerPos;
    public Transform targetPosition;
    public CharacterController controller;
    public float speed = 3f;
    bool isJumping = false;
    enum State { Moving, Jumping, Falling};
    State currentState;
    public float distance;
    void Start()
    {

        currentState = State.Moving;
    }
    void Update()
    {
        switch(currentState)
        {
            case State.Moving:
                MoveTowardsTarget();
                break;
            case State.Jumping:
                Jump();
                StartCoroutine(JumpCoroutine());
                break;
            case State.Falling:
                FallDown();
                break;

        }
    }

    void Jump()
    {
        controller.Move(Vector3.up * speed * Time.deltaTime);
        
    }
    void FallDown()
    {
        //controller.Move(Vector3.down * speed * Time.deltaTime);
        if (controller.isGrounded)
        {
            currentState = State.Moving;
        }
    }
    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition.transform.position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerOnRange"))
        {
            currentState = State.Jumping;
        }
    }
    IEnumerator JumpCoroutine()
    {
        Debug.Log("Preparándome para saltar");
        isJumping = true;
        LastPlayerPos = targetPosition.position;
        yield return new WaitForSeconds(2f);
        Debug.Log("saltando!");
        isJumping = false;
        transform.DOMove(LastPlayerPos + (Vector3.up * 3), 0.5f).SetEase(Ease.InBounce);
        currentState = State.Falling;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LastPlayerPos, 3);
    }
}
