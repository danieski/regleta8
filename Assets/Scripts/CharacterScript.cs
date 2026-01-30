using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
    public CharacterController controller;
    public float velocityVariable = 5f;
    private Vector3 velocityCharacter;

    void Start()
    {
        
    }

    // Update is called once per frame
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

}
