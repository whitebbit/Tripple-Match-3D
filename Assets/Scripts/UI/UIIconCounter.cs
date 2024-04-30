using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIconCounter : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI counter;

    public UIIconCounter Init(Sprite icon, int count = 0)
    {
        this.icon.sprite = icon;
        counter.gameObject.SetActive(count != 0);
        if (count != 0) counter.text = count.ToString();
        return this;
    }

    public void SetCount(int count) => counter.text = count.ToString();
}