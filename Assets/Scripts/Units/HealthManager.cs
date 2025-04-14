public class HealthManager
{
    private int maxHealth;
    private int currentHealth;
    private HealthBar healthBar;

    public HealthManager(int health, HealthBar bar)
    {
        maxHealth = health;
        currentHealth = health;
        healthBar = bar;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public bool IsDead() => currentHealth <= 0;
}
