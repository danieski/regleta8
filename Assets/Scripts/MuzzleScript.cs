using DG.Tweening;
using UnityEngine;

public class MuzzleScript : MonoBehaviour
{
    void OnEnable()
    {
        transform.DOScale(1, 2f).SetEase(Ease.OutBack);
    }
    void OnDisable()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
