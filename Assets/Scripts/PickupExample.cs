using UnityEngine;

[CreateAssetMenu(fileName = "PickupExample", menuName = "Scriptable Objects/PickupExample")]
public class PickupExample : ScriptableObject
{
    public int healAmount = 1;
    public void OnPickup()
    {
        Debug.Log("PickupExample picked up!");

    }
}
