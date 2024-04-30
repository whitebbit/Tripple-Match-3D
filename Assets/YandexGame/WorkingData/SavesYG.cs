using System;

namespace YG
{
    [Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public bool NoAds;
        public bool[] OneTimePurchase = new bool[7];
        public bool[] OneTimeMarks = new bool[Enum.GetNames(typeof(UIOneTimeMark.OneTimeMarkType)).Length];
        public int NoAdsPopupLvl = 1;

        public DateTime OldHPTime;
        public int HP = 5, Coins;
        public DateTime InfinityHPTime;

        public int CurrentLevel;
        public int CompleteLevelCount;
        public int LastLevelIndex;
        public int CurrentAttempt;

        public bool Music = true, SFX = true;

        public int[] UpgradesLvl;

        public int[] AbilityCount = new int[6] { 3, 3, 3, 3, 3, 3 };
        public DateTime InfinityThunderTime, InfinityAddTimeTime;

        public int Stars, LvlChestOpen, ChestCurReward;

        public bool JuicyTestPopupIsShow;
        public DateTime OldJuicyTestTime;
        public int JuicyTestCurValue, JuicyTestCurLvl, JuicyTestCurType = -1;
        public DateTime X2JuicyTestScoreTime;

        public int CrownRaceScore = 1;
        public bool CrownRacePopupIsShow;

        public BigEventType CurEvent;
        public DateTime OldEventTime;

        public int SkyAdventureLvl = -1, SkyAdventurePlayersCount;
        public DateTime SkyAdventureCooldown;
        public bool SkyAdventurePopupIsShow;

        public int CarRaceScore, CarRaceLvl = -1;
        public int[] CarRacePlayersScore = new int[4];
        public DateTime CarRaceCooldown;
        public bool CarRacePopupIsShow;
        public bool CarRaceIsStarted;

        public SavesYG()
        {
        }
    }
}