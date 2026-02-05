using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathState : CharacterBaseState
{

    public override void EnterState(CharacterStateManager character)
    {
        SceneManager.LoadScene("Death");
        //Debug.Log("Entered Die State");

    }
    public override void UpdateState(CharacterStateManager character)
    {

    }
    public override void OnCollisionEnter(CharacterStateManager character)
    {

    }

}
