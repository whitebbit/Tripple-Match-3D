using UnityEngine;
using YG;

public class UIShopButton : MonoBehaviour
{
    public enum PurchasedType
    {
        Set_00, Set_01, Set_02, Set_03, Set_04, Set_05, No_Ads, Coins_00, Coins_01, Coins_02, Coins_03, Coins_04, Coins_05
    }

    [SerializeField] PurchasedType type;
    [SerializeField] bool oneTimePayment;

    void Awake()
    {
        if (!oneTimePayment) return;
        if (GameData.Saves.OneTimePurchase[(int)type]) gameObject.SetActive(false);
        else YandexGame.PurchaseSuccessEvent += SuccessPurchased;
    }

    void SuccessPurchased(string id)
    {
        if (id == type.ToString()) Buy();
        else if (type == PurchasedType.No_Ads && (id == "No_Ads_00" || id == "No_Ads_01")) Buy();
    }

    void Buy()
    {
        GameData.Saves.OneTimePurchase[(int)type] = true;
        GameData.SaveProgress();

        gameObject.SetActive(false);
    }
}