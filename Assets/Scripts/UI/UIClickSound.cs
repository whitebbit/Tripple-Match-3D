using UnityEngine;
using UnityEngine.UI;

public class UIClickSound : MonoBehaviour
{
    [SerializeField] bool close;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    void Click() => SoundHolder.Default.PlayFromSoundPack(close ? "Click Close" : "Click");
}