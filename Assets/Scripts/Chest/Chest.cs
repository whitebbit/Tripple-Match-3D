using System;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum State
    {
        None, Fall, Idle, Open, Reduce
    }
    State state;
    public State CurState
    {
        get => state;
        set
        {
            if (state == value) return;
            animator.SetInteger("State", (int)value);
            state = value;
        }
    }

    [SerializeField] Animator animator;

    public event Action EndFallEvent, OnOpenEvent;

    void EndFall()
    {
        CurState = State.Idle;
        EndFallEvent?.Invoke();
    }
    
    void Open()
    {
        CurState = State.Reduce;
        OnOpenEvent?.Invoke();
    }
}
