using System;
using System.Collections.Generic;
using MyInputManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Singletone
    private static PlayerController _instance;
    public static PlayerController Instance { get => _instance; }
    public PlayerController() => _instance = this;
    #endregion

    public enum State
    {
        Idle
    }
    State state;
    public State CurState
    {
        get => state;
        set
        {
            if (value == state) return;
            switch (value)
            {
                case State.Idle:
                    {
                    }
                    break;
            }
            state = value;
        }
    }

    [NonSerialized] public Transform thisTransform;
    Camera cam;
    bool down;
    Item curItem;

    void OnEnable()
    {
        InputManager.DownEvent += Down;
        InputManager.UpEvent += Up;
    }

    void OnDisable()
    {
        InputManager.DownEvent -= Down;
        InputManager.UpEvent -= Up;
    }

    void Down() => down = true;
    void Up()
    {
        down = false;
        if (curItem)
        {
            curItem.Collect();
            curItem = null;
        }
    }

    public void Init()
    {
        thisTransform = transform;
        cam = Camera.main;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                {
                    if (down)
                    {
                        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                        Item item = null;
                        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.1f, 100, GameData.Default.itemsLayerMask);
                        if (hits.Length > 1)
                        {
                            ref List<Quest> quests = ref Level.Instance.quests;
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].rigidbody.TryGetComponent(out item))
                                {
                                    for (int j = 0; j < quests.Count; j++) if (item.ID == quests[j].ID && quests[j].count % 3 == 1) goto END;
                                }
                            }
                        }
                        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit, 100, GameData.Default.itemsLayerMask)) hit.rigidbody.TryGetComponent(out item);

                        END:
                        if (item)
                        {
                            if (item != curItem)
                            {
                                DeselectItem();
                                curItem = item;
                                curItem.Select();
                            }
                        }
                        else DeselectItem();
                    }
                }
                break;
        }
    }

    void DeselectItem()
    {
        if (!curItem) return;
        curItem.Deselect();
        curItem = null;
    }
}
