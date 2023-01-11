namespace GBG.GameAbilitySystem.Buff
{
    // todo: uint Buff Duration
    // todo: Buff Exclusion?

    // todo: Stacking
    public enum BuffStackingMode
    {
    }

    public interface IBuff
    {
        BuffSpec BuffSpec { get; }

        int InstanceId { get; }
    }
}