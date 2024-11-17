public interface IAbility
{
    void Activate();
    bool IsOnCooldown(); 
    float GetCooldown();
}
