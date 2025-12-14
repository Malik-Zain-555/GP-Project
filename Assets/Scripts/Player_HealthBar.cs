using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_HealthBar : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider;
    public GameObject deathPanel;

    SpriteRenderer sr;
    bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        sr = GetComponent<SpriteRenderer>();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    // Called repeatedly by spikes
    public void TryDamage(int damage, float cooldown)
    {
        if (!isInvincible)
        {
            TakeDamage(damage);
            StartCoroutine(Invincibility(cooldown));
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Time.timeScale = 0;
        deathPanel.SetActive(true);
    }

    IEnumerator Invincibility(float time)
    {
        isInvincible = true;

        // Blink red
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;

        // Wait cooldown before damage again
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
}
