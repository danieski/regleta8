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
    public int reloadTime = 1;
    private BulletScript MyBullet;
    private Vector3 velocityCharacter;
    public bool isInvencible;
    public Animator animator;


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
        Vector2 input = value.ReadValue<Vector2>();
        if (input == Vector2.zero) return;

        Vector3 dir;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            // Izquierda / Derecha
            dir = new Vector3(Mathf.Sign(input.x), 0f, 0f);
        }
        else
        {
            // Arriba / Abajo
            dir = new Vector3(0f, 0f, Mathf.Sign(input.y));
        }

        transform.rotation = Quaternion.LookRotation(dir);
    }
    public void Shoot(InputAction.CallbackContext value)
    {

        //if (value.phase != InputActionPhase.Performed) return;
        MyBullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
        SwitchState(reloadState);
        StartCoroutine(ReloadCoroutine());
    }
    public void OnDeath()
    {
        SceneManager.LoadScene("Death");
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        SwitchState(idleState);

    }
    IEnumerator InvencivilityCorutine()
    {
        yield return new WaitForSeconds(1);
        isInvencible = false;
        Debug.Log("Ya no soy invencible");
    }
}
