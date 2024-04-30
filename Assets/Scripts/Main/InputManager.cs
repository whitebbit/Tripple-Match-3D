using System;
using UnityEngine;

namespace MyInputManager
{
    public class InputManager : MonoBehaviour
    {
        #region Singletone
        private static InputManager _instance;
        public static InputManager Instance { get => _instance; }
        public InputManager() => _instance = this;
        #endregion

        public static event Action DownEvent;
        public static event Action UpEvent;
        int lockControll;

        public void Down() { if (lockControll == 0) DownEvent?.Invoke(); }
        public void Up() { if (lockControll == 0) UpEvent?.Invoke(); }

        void Lock() => lockControll++;
        void Unlock() => lockControll--;
        public static void LockControll() => Instance.Lock();
        public static void UnlockControll() => Instance.Unlock();
    }
}