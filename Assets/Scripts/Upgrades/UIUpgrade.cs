// using System.Collections.Generic;
// using DG.Tweening;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using YG;

// public class UIUpgrade : MonoBehaviour
// {
//     [SerializeField] UpgradeType type;
//     [SerializeField] Image maskImage;
//     Tween colorTween;

//     [Space]
//     [SerializeField] TextMeshProUGUI priceTxt;
//     [SerializeField] TextMeshProUGUI lvlTxt;
//     [SerializeField] RectTransform pricePanelTransform;
//     RectTransform thisTransform;

//     readonly static Dictionary<Language, string> LvlName = new Dictionary<Language, string> { { Language.RU, "УР." }, { Language.EN, "LV." }, { Language.TR, "SE." } };
//     readonly static Dictionary<Language, string> MaxLvlName = new Dictionary<Language, string> { { Language.RU, "МАКС" }, { Language.EN, "MAX" }, { Language.TR, "MAKS" } };

//     readonly static Color LockColor = new Color(0, 0, 0, 0.675f);
//     readonly static Color UpgradeColor = new Color(1, 0.85f, 0.1f, 0.8f);

//     public void Start()
//     {
//         thisTransform = GetComponent<RectTransform>();
//         if (Data.MaxLvl > CurLvl)
//         {
//             GameData.UpdateCoinsEvent += UpdateCash;
//             UpdateCash();
//         }
//         UpdatePrice();
//         UpdateLvl();
//     }

//     void UpdateCash()
//     {
//         colorTween.Kill(true);
//         maskImage.color = Data.MaxLvl <= CurLvl || Data.CurPrice <= GameData.HP ? Color.clear : LockColor;
//     }
//     void UpdatePrice() => priceTxt.text = Data.MaxLvl > CurLvl ? Data.CurPrice + "<sprite=0>" : MaxLvlName[GameData.Language];
//     void UpdateLvl() => lvlTxt.text = LvlName[GameData.Language] + (GameData.Saves.UpgradesLvl[(int)type] + 2);

//     public void Buy()
//     {
//         if (Data.MaxLvl <= CurLvl || !GameData.Default.TryPayCoins(Data.CurPrice)) return;

//         GameData.Saves.UpgradesLvl[(int)type]++;
//         GameData.SaveProgress();

//         UpdatePrice();
//         UpdateLvl();
//         UpdateCash();
//         if (CurLvl >= Data.MaxLvl) GameData.UpdateCoinsEvent -= UpdateCash;

//         maskImage.DOColor(UpgradeColor, 0.33f).From();
//         SoundHolder.Default.PlayFromSoundPack("Buy");

//         int count = Random.Range(2, 5);
//         for (int i = 0; i < count; i++)
//         {
//             GameObject arrow = Instantiate(GameData.Default.upgradeArrowUI, thisTransform);
//             Transform arrowTransform = arrow.transform;
//             arrowTransform.SetSiblingIndex(pricePanelTransform.GetSiblingIndex());
//             arrowTransform.localPosition = pricePanelTransform.localPosition + Vector3.right * Random.Range(-90f, 90f);
//             arrowTransform.localScale *= Random.Range(0.75f, 1.25f);

//             float time = Random.Range(0.7f, 1.1f);
//             arrowTransform.DOLocalMoveY(Random.Range(120f, 300f), time);
//             arrowTransform.DOScale(0, 0.33f).SetDelay(time - 0.33f);
//             arrow.GetComponent<Image>().DOFade(0, 0.33f).SetDelay(time - 0.33f).OnComplete(() => Destroy(arrow));
//         }
//     }

//     int CurLvl => GameData.Saves.UpgradesLvl[(int)type] + 1;
//     Upgrade Data => GameData.Default.GetUpgrade(type);
// }