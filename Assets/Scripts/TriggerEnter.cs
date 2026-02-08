using UnityEngine;
using UnityEngine.Events;

public class TriggerEnter : MonoBehaviour
{
    [SerializeField] private string onlytag = "";
    [SerializeField] private UnityEvent onTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (onlytag != "" && !other.CompareTag(onlytag)) return;

        onTrigger.Invoke();
    }
}
