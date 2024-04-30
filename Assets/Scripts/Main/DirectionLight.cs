using UnityEngine;

public class DirectionLight : MonoBehaviour
{
    #region Singletone
    private static DirectionLight _instance;
    public static DirectionLight Instance { get => _instance; }
    public DirectionLight() => _instance = this;
    #endregion

    Transform thisTransform;

    void Awake()
    {
        thisTransform = transform;
    }

    public void SetRotation(Quaternion angle) => thisTransform.rotation = angle;
}
