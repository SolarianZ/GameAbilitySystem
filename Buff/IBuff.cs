namespace GBG.GameAbilitySystem.Buff
{
    public interface IBuff
    {
        BuffData Data { get; }

        int InstanceId { get; }
    }
}