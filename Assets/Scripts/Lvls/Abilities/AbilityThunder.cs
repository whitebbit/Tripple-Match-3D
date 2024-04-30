using System;

public class AbilityThunder : StartGameAbility
{
    protected override void Use()
    {
        Level.Instance.UseThunder();
        base.Use();
    }

    protected override bool IsInfinity => GameData.Saves.InfinityThunderTime > DateTime.Now;

    public static void AddInfinity(int time)
    {
        if (GameData.Saves.InfinityThunderTime < DateTime.Now) GameData.Saves.InfinityThunderTime = DateTime.Now.AddMinutes(time);
        else GameData.Saves.InfinityThunderTime = GameData.Saves.InfinityThunderTime.AddMinutes(time);
        GameData.SaveProgress();
    }
}