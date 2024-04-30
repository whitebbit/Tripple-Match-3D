using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UISettingsButton : MonoBehaviour
{
    [SerializeField] Image musicImage, sfxImage;
    [SerializeField] Sprite musicSpriteOn, musicSpriteOff;
    [SerializeField] Sprite sfxSpriteOn, sfxSpriteOff;
    [SerializeField] TextMeshProUGUI gameVersion;

    void Start()
    {
        musicImage.sprite = GameData.Saves.Music ? musicSpriteOn : musicSpriteOff;
        sfxImage.sprite = GameData.Saves.SFX ? sfxSpriteOn : sfxSpriteOff;
        gameVersion.text = Application.version;
    }

    public void ToggleMusic()
    {
        GameData.Saves.Music = !GameData.Saves.Music;
        GameData.SaveProgress();

        SoundHolder.Default.SetSFX(GameData.Saves.Music);
        musicImage.sprite = GameData.Saves.Music ? musicSpriteOn : musicSpriteOff;
    }

    public void ToggleSFX()
    {
        GameData.Saves.SFX = !GameData.Saves.SFX;
        GameData.SaveProgress();

        SoundHolder.Default.SetSFX(GameData.Saves.SFX);
        sfxImage.sprite = GameData.Saves.SFX ? sfxSpriteOn : sfxSpriteOff;
    }

    public void SetLanguage(string code)
    {
        GameData.Language = GameData.LanguageType[code];
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == code);
        if (locale) LocalizationSettings.SelectedLocale = locale;
    }
}
