using System;

public class AbilityAddTime : StartGameAbility
{
    protected override void Use()
    {
        Level.Instance.AddTime(GameData.Default.addTimeAbilityValue);
        base.Use();
    }

    protected override bool IsInfinity => GameData.Saves.InfinityAddTimeTime > DateTime.Now;

    public static void AddInfinity(int time)
    {
        if (GameData.Saves.InfinityAddTimeTime < DateTime.Now) GameData.Saves.InfinityAddTimeTime = DateTime.Now.AddMinutes(time);
        else GameData.Saves.InfinityAddTimeTime = GameData.Saves.InfinityAddTimeTime.AddMinutes(time);
        GameData.SaveProgress();
    }
}