using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public enum states
    {
        IDLE,
        RELOAD,
        DEATH
    }
    public states myState;

    //Variables
    public float velocityVariable = 5f;
    public float reloadTime = 1;

    //Prefabs
    public GameObject fakeBigBullet;
    private BulletScript MyBullet;
    public BulletScript bulletPrefab;
    public CharcoScript charcoPrefab;
    public ChargedShoot bigBulletPrefab;

    //Misc
    public CharacterController controller;
    private Vector3 velocityCharacter;
    public Animator animator;
    [SerializeField] private LineAttack lineAttack, lineAttack2;
    private Room.Direction facingDirection = Room.Direction.TOP;


    void Start()
    {
        myState = states.IDLE;
        //currentState.EnterState(this);
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
        //Shoot Left
        if (value.control.name == "leftArrow" && myState != states.RELOAD)
        {
            if (value.phase == InputActionPhase.Performed)
            {
                fakeBigBullet.SetActive(true);
            }
            if (value.phase == InputActionPhase.Canceled)
            {
                Instantiate(bigBulletPrefab, transform.position + new Vector3(-3, 0, 0), Quaternion.Euler(0, 90, 0));
                fakeBigBullet.SetActive(false);
                StartCoroutine(ReloadCoroutine());
            }
            return;
        }

        if (value.phase != InputActionPhase.Performed) return;
        if (myState != states.IDLE) return;
        StartCoroutine(ReloadCoroutine());

        switch (facingDirection)
        {
            case Room.Direction.TOP:
                Instantiate(charcoPrefab,transform.position-new Vector3(0,3,0), Quaternion.Euler(90, 0, 0));

                break;
            case Room.Direction.BOTTOM:
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.Euler(0, 180, 0));
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.Euler(0, 150, 0));
                Instantiate(bulletPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.Euler(0, 210, 0));
                break;
            case Room.Direction.LEFT:
                break;
            case Room.Direction.RIGHT:
                lineAttack.Attack();
                break;
        }
        StartCoroutine(ReloadCoroutine());
    }
    IEnumerator ReloadCoroutine()
    {
        myState = states.RELOAD;
        yield return new WaitForSeconds(reloadTime);
        myState = states.IDLE;

    }
    public void OnPlayerDie()
    {
        SceneManager.LoadScene("Death");
    }

}
