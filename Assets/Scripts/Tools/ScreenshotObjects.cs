#if UNITY_EDITOR
using System.Collections;
using AlmostEngine.Screenshot;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenshotObjects : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    void OnEnable()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
            yield return SimpleScreenshotCapture.CaptureToFileCoroutine($"C:/Users/user/Pictures/Screenshots/{i}.png", 128, 128, colorFormat: ScreenshotTaker.ColorFormat.RGBA, recomputeAlphaMask: true);
            objects[i].SetActive(false);
        }
    }
}
#endif