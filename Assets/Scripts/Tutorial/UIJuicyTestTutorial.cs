using UnityEngine;

public class UIJuicyTestTutorial : UITutorialPlane
{
    [SerializeField] RectTransform hand;
    Item item;
    RectTransform rectTransform;

    protected override void Show()
    {
        base.Show();
        item = Level.Instance.curItems.Find(x => x as JuicyTestItem);
        item.OnCollectEvent += (x) => gameObject.SetActive(false);
        item.thisTransform.position = Vector3.up * 4;

        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (item)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(item.thisTransform.position);
            screenPoint.x *= rectTransform.rect.width / (float)Camera.main.pixelWidth;
            screenPoint.y *= rectTransform.rect.height / (float)Camera.main.pixelHeight;
            hand.anchoredPosition = screenPoint - rectTransform.sizeDelta / 2f;
        }
    }

    public static Vector2 GetRelativePosOfWorldPoint(Vector3 worldPoint, Camera camera)
    {
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
        return new Vector2(screenPoint.x / camera.pixelWidth, screenPoint.y / camera.pixelHeight);
    }

}
