using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0, 0);
    }
    //void LateUpdate()
    //{
    //    //transform.LookAt(GameManager.instance.mainCamera.transform);
    //    //transform.Rotate(0, 180f, 0);
    //    Vector3 rotation = GameManager.instance.mainCamera.transform.eulerAngles;
    //    transform.rotation = Quaternion.Euler(0, rotation.y, 0);
    //}
}
