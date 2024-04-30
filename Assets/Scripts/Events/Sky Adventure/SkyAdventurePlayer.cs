using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkyAdventurePlayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Image image;
    [SerializeField] TMP_Text letter;
    [NonSerialized] public Transform thisTransform;

    public void Init(Color color, char letter)
    {
        thisTransform = transform;
        if (spriteRenderer) spriteRenderer.color = color; else image.color = color;
        this.letter.text = letter.ToString();
    }
}
