using UnityEngine;

public class FrogAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 5f;
    public float moveSpeed = 2f;
    public float idleTime = 0.5f;

    [Header("Damage Settings")]
    public int damageToPlayer = 1;
    public float knockbackForceX = 5f;
    public float knockbackForceY = 3f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool movingRight = false;
    private bool isGrounded = true;
    private bool isIdle = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // If frog is grounded and not idling → jump again
        if (isGrounded && !isIdle)
        {
            Jump();
        }
    }

    void Jump()
    {
        isGrounded = false;

        // Play jump animation
        anim.Play("frog-jump");

        // Movement direction: left or right
        float dir = movingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * moveSpeed, jumpForce);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ---------------- PLAYER DAMAGE + KNOCKBACK ----------------
        if (collision.collider.CompareTag("Player"))
        {
            Player_HealthBar p = collision.collider.GetComponent<Player_HealthBar>();

            if (p != null)
            {
                p.TakeDamage(damageToPlayer);
                PlayerHitFlash(collision.collider.gameObject);

                // Apply knockback
                Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    float knockDir = movingRight ? knockbackForceX : -knockbackForceX;
                    playerRb.linearVelocity = new Vector2(knockDir, knockbackForceY);
                }
            }
        }

        // ---------------- GROUND DETECTION ----------------
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            StartCoroutine(IdleRoutine());
        }
    }

    private System.Collections.IEnumerator IdleRoutine()
    {
        isIdle = true;

        // Play idle animation once
        anim.Play("Frog-Idle");

        yield return new WaitForSeconds(idleTime);

        // Change direction and flip sprite
        movingRight = !movingRight;
        Flip();

        isIdle = false;
    }

    // ---------------- SPRITE FLIP ----------------
    void Flip()
    {
        Vector3 s = transform.localScale;

        // REVERSED because sprite faces the opposite direction normally
        s.x = movingRight ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);

        transform.localScale = s;
    }


    // ---------------- PLAYER DAMAGE FLASH ----------------
    void PlayerHitFlash(GameObject player)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.color = Color.red;
            Invoke("ResetPlayerColor", 0.15f);
        }
    }

    void ResetPlayerColor()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            sr.color = Color.white;
        }
    }
}
