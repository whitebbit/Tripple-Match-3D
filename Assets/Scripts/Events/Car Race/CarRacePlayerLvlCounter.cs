using TMPro;
using UnityEngine;

public class CarRacePlayerLvlCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lvl;
    
    public void SetLvl(int lvl) => this.lvl.text = lvl.ToString();
}
