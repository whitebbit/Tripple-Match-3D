using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePopup : EndPopup
{
    public override void Show()
    {
        GameData.Default.PayHP(1);
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}