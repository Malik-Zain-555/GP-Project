using UnityEngine;

public class EagleAI : MonoBehaviour
{
    [Header("Movement")]
    public float flySpeed = 3f;
    public float detectionRange = 8f;
    public float patrolRange = 3f;
    public float retreatDistance = 2.5f;

    [Header("Attack")]
    public float attackCooldown = 0.8f;

    [Header("Damage")]
    public int damageToPlayer = 1;
    public float knockbackX = 5f;
    public float knockbackY = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isHurt;
    private bool isAttacking;
    private bool isCoolingDown;

    private float startX;
    private int patrolDir = 1;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        startX = transform.position.x;
    }

    void Update()
    {
        if (isHurt || isAttacking || isCoolingDown) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            ChasePlayer();
        else
            Patrol();
    }

    // ---------------- MOVEMENT ----------------

    void Patrol()
    {
        anim.Play("Eagle-Fly");

        float left = startX - patrolRange;
        float right = startX + patrolRange;

        if (transform.position.x >= right) patrolDir = -1;
        if (transform.position.x <= left) patrolDir = 1;

        rb.linearVelocity = new Vector2(patrolDir * flySpeed, 0);
        Flip(patrolDir);
    }

    void ChasePlayer()
    {
        anim.Play("Eagle-Fly");

        float dir = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * flySpeed, 0);
        Flip((int)dir);
    }

    void Flip(int dir)
    {
        Vector3 s = transform.localScale;
        s.x = dir > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ---------------- COLLISION ----------------

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
        if (isAttacking || isCoolingDown) return;

        isAttacking = true;
        isCoolingDown = true;

        anim.Play("Eagle-Dive-Attack");

        Player_HealthBar p = col.collider.GetComponent<Player_HealthBar>();
        Rigidbody2D playerRb = col.collider.GetComponent<Rigidbody2D>();

        if (p != null)
            p.TakeDamage(damageToPlayer);

        if (playerRb != null)
        {
            float dir = col.transform.position.x > transform.position.x ? 1 : -1;
            playerRb.linearVelocity = new Vector2(dir * knockbackX, knockbackY);
            StartCoroutine(PlayerHitFlash(col.collider.GetComponent<SpriteRenderer>()));
        }

        // retreat eagle
        float retreatDir = transform.position.x > col.transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(retreatDir * flySpeed * 2f, 0);

        Invoke(nameof(EndAttack), 0.25f);
        Invoke(nameof(EndCooldown), attackCooldown);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void EndCooldown()
    {
        isCoolingDown = false;
    }

    // ---------------- DAMAGE EFFECT ----------------

    System.Collections.IEnumerator PlayerHitFlash(SpriteRenderer sr)
    {
        if (sr == null) yield break;

        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        sr.color = original;
    }

    // ---------------- HURT ----------------

    public void TakeDamage()
    {
        isHurt = true;
        anim.Play("Eagle-Hurt");
        rb.linearVelocity = Vector2.zero;

        Invoke(nameof(Recover), 0.4f);
    }

    void Recover()
    {
        isHurt = false;
        anim.Play("Eagle-Fly");
    }

    // ---------------- GIZMOS ----------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - patrolRange, transform.position.y, 0),
            new Vector3(transform.position.x + patrolRange, transform.position.y, 0)
        );
    }
}
