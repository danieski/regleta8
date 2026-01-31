using UnityEngine;
using UnityEngine.Events;

public class TriggerEnter : MonoBehaviour
{
    [SerializeField] private UnityEvent onTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterScript>() == null) return;

        onTrigger.Invoke();
    }
}
