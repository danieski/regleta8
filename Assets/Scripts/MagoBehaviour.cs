using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MagoBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackFrequency = 2f, attackDuration = 3f;
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject attackPrefab;

    private bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartAttack());
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking) return;
        Vector3 direction = (mostFarPointFromTarget().position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);

    }

    public IEnumerator StartAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackFrequency);
            if (!isAttacking)
            {
                isAttacking = true;
                Attack();
                yield return new WaitForSeconds(attackDuration);
                isAttacking = false;
            }

        }
    }

    private Transform mostFarPointFromTarget()
    {
        Transform farthestPoint = pathPoints[0];
        float maxDistance = Vector3.Distance(GameManager.instance.player.position, pathPoints[0].position);
        foreach (Transform point in pathPoints)
        {
            float distance = Vector3.Distance(GameManager.instance.player.position, point.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPoint = point;
            }
        }
        return farthestPoint;
    }

    private void Attack()
    {
        animator.SetTrigger("attack");

        Instantiate(attackPrefab, new Vector3(GameManager.instance.player.position.x, 1f, GameManager.instance.player.position.z), Quaternion.identity);
    }
}
