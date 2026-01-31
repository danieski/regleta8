using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0); // corrige si se ve al revés
    }
}
