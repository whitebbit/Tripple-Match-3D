using UnityEngine;

public class AbilityTutorial : MonoBehaviour
{
    [SerializeField] AbilityType type;
    Ability curAbility;
    Transform abilitiesParent;

    void Awake()
    {
        curAbility = UIManager.Instance.inGameWindow.abilities[(int)type];
        Debug.Log(curAbility.transform.position);
        if (type == AbilityType.Return) Level.Instance.curItems.Find(x => !x.isQuest).Collect();
    }

    void OnEnable()
    {
        curAbility.UseEvent += UseAbility;
        Transform abilityTransform = curAbility.transform;
        abilitiesParent = abilityTransform.parent;
        abilityTransform.SetParent(transform);
    }

    void OnDisable()
    {
        curAbility.UseEvent -= UseAbility;
        curAbility.transform.SetParent(abilitiesParent);
    }

    void UseAbility()
    {
        GameData.Saves.AbilityCount[(int)type]++;
        GameData.SaveProgress();

        transform.parent.GetComponent<UITutorialPlane>().Hide();
        curAbility.UseEvent -= UseAbility;
        curAbility.transform.SetParent(abilitiesParent);
    }
}
