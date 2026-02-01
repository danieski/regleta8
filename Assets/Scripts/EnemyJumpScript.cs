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

    private float timer = 0f;

    void Start()
    {
        if (targetPosition == null)
        {
            targetPosition = GameManager.instance.player;
        }
        currentState = State.Moving;
    }
    void Update()
    {
        switch(currentState)
        {
            case State.Moving:
                if (isJumping) return;
                //MoveTowardsTarget();
                timer += Time.deltaTime;
                if (timer > 1f)
                {
                    timer = 0f;
                    currentState = State.Jumping;
                }
                break;
            case State.Jumping:
                //Jump();
                if (isJumping) return;
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
        yield return new WaitForSeconds(0.5f);
        Debug.Log("saltando!");
        LastPlayerPos = targetPosition.position;
        transform.DOJump(new Vector3(LastPlayerPos.x, transform.position.y, LastPlayerPos.z), 3f, 1, 1f);
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
        currentState = State.Moving;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LastPlayerPos, 3);
    }
}
