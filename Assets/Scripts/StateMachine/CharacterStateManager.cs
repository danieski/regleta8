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
    public bool isInvencible;
    public Animator animator;
    [SerializeField] private LineAttack lineAttack, lineAttack2;

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
    public void OnDamage()
    {
        //Invencibilidad
        Debug.Log("OnDamage");
        isInvencible = true;
        StartCoroutine(InvencivilityCorutine());

    }

    public void SwitchState(CharacterBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void ShootDirection(InputAction.CallbackContext value)
    {
        if(value.phase != InputActionPhase.Performed) return;
        switch (value.control.name)
        {
            case "upArrow":
                facingDirection = Room.Direction.TOP;
                break;
            case "downArrow":
                facingDirection = Room.Direction.BOTTOM;
                break;
            case "leftArrow":
                facingDirection = Room.Direction.LEFT;
                break;
            case "rightArrow":
                facingDirection = Room.Direction.RIGHT;
                break;

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
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0));
                break;
            case Room.Direction.BOTTOM:
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.Euler(0, 180, 0));
                break;
            case Room.Direction.LEFT:
                lineAttack2.Attack();
                break;
            case Room.Direction.RIGHT:
                lineAttack.Attack();
                break;
        }
        StartCoroutine(ReloadCoroutine());
    }
    IEnumerator ReloadCoroutine()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        SwitchState(idleState);
        Debug.Log("Reloaded.");

    }
    IEnumerator InvencivilityCorutine()
    {
        yield return new WaitForSeconds(1);
        isInvencible = false;
        Debug.Log("Ya no soy invencible");
    }
}
