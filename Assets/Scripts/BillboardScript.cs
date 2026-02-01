using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(GameManager.instance.mainCamera.transform);
        transform.Rotate(0, 180f, 0);
    }
}
