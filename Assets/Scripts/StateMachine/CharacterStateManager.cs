using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterStateManager : MonoBehaviour
{
    CharacterBaseState currentState;
    public IdleState idleState = new IdleState();
    public ReloadState reloadState = new ReloadState();
    public DeathState deathState = new DeathState();


    public CharacterController controller;
    public float velocityVariable = 5f;
    public BulletScript bulletPrefab;
    public float reloadTime = 1;
    private BulletScript MyBullet;
    private Vector3 velocityCharacter;

    public Animator animator;
    [SerializeField] private LineAttack lineAttack;

    private Room.Direction facingDirection = Room.Direction.TOP;


    void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }
    public void ForwardMovement(InputAction.CallbackContext value)
    {
        //Movement
        var VelocityVector2 = value.ReadValue<Vector2>();
        velocityCharacter.x = VelocityVector2.x;
        velocityCharacter.z = VelocityVector2.y;

        if (velocityCharacter.z != 0 && velocityCharacter.x != 0)
        {
            animator.SetBool("isRight", true);
        }
        else
        {
            animator.SetBool("isRight", false);
        }


    }
    void Update()
    {
        controller.Move(velocityCharacter * Time.deltaTime * velocityVariable);
        //currentState.UpdateState(this);
    }

    public void SwitchState(CharacterBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void ShootDirection(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        if (input == Vector2.zero) return;


        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            // Izquierda / Derecha
            facingDirection = input.x > 0f ?Room.Direction.RIGHT : Room.Direction.LEFT;
        }
        else
        {
            // Arriba / Abajo
            facingDirection = input.y > 0f ? Room.Direction.TOP : Room.Direction.BOTTOM;
        }

    }
    public void Shoot(InputAction.CallbackContext value)
    {

        if (value.phase != InputActionPhase.Performed) return;
        //Debug.Log(currentState);
        if (idleState != currentState) return;
        SwitchState(reloadState);

        switch (facingDirection)
        {
            case Room.Direction.TOP:
                //animator.SetTrigger("ShootUp");
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0));
                break;
            case Room.Direction.BOTTOM:
                //animator.SetTrigger("ShootDown");
                break;
            case Room.Direction.LEFT:
                //animator.SetTrigger("ShootLeft");
                break;
            case Room.Direction.RIGHT:
                //animator.SetTrigger("ShootRight");
                lineAttack.Attack();
                break;
        }
        StartCoroutine(ReloadCoroutine());
    }
    public void OnDeath()
    {
        SceneManager.LoadScene("Death");
    }

    IEnumerator ReloadCoroutine()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        SwitchState(idleState);
        Debug.Log("Reloaded.");

    }
}
