using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
    public CharacterController controller;
    public float velocityVariable = 5f;
    public BulletScript bulletPrefab;
    private BulletScript MyBullet;
    private Vector3 velocityCharacter;


    void Start()
    {
        
    }
    void Update()
    {
        controller.Move(velocityCharacter * Time.deltaTime * velocityVariable);
    }

    public void ForwardMovement(InputAction.CallbackContext value)
    {
        var VelocityVector2 = value.ReadValue<Vector2>();
        velocityCharacter.x = VelocityVector2.x;
        velocityCharacter.z = VelocityVector2.y;
    }
    public void Shoot(InputAction.CallbackContext value)
    {

        if(value.phase!=InputActionPhase.Performed) return;
        MyBullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
    }

}
