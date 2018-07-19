namespace Weapon
{
    public interface IDamageTaker
    {
        void TakeDamage(float damage, string damagerReceiverName);
        void Reinitialize();
    }
}