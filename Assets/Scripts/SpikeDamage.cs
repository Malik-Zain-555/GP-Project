using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 6;
    public float damageCooldown = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player_HealthBar hp = other.GetComponent<Player_HealthBar>();
            if (hp != null)
            {
                hp.TryDamage(damage, damageCooldown);
            }
        }
    }
}
