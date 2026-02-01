using UnityEngine;

public class LineAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LineRenderer lineRenderer;
    private bool isAttacking = false;
    private float attackTick = 0f;
    private int numTicks = 5, ticksCount = 0;
    private float tickDuration = 0.3f;
    public void Attack()
    {
        Debug.Log("Line Attack initiated.");
        isAttacking = true;
    }

    private void Update()
    {
        if (!isAttacking) return;
        attackTick += Time.deltaTime;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * 100f);
        if (attackTick < tickDuration) return;
        attackTick = 0f;
        ticksCount++;
        if (ticksCount >= numTicks)
        {
            ticksCount = 0;
            isAttacking = false;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 100f);
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Player") continue;
            HealthComponent health = hit.collider.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100f);
    }
}
