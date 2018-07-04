namespace Weapon
{
    public interface IDamageTaker
    {
        float TakeDamage(float damage, string damagerReceiverName);
    }
}