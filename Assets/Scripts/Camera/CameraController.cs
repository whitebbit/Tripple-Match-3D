using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraController : MonoBehaviour
{
    #region Singletone
    private static CameraController _instance;
    public static CameraController Instance { get => _instance; }
    public CameraController() => _instance = this;
    #endregion

    [SerializeField] CameraData data;

    public static Vector3 MainOffset;
    public static Vector3 MainAngle;

    public class FollowTarget
    {
        public Transform transform;
        public Vector3 offset;
        public Quaternion angle;
        public float speed;
        public int priority;
        public int hash;

        public FollowTarget(Transform transform, Vector3 offset, Quaternion angle, float speed, int priority, int hash)
        {
            this.transform = transform;
            this.offset = offset;
            this.angle = angle;
            this.speed = speed;
            this.priority = priority;
            this.hash = hash;
        }
    }
    [NonSerialized] public List<FollowTarget> followTargets;
    float curFollowSpeed;

    Camera cam;
    [NonSerialized] public Transform thisTransform, camTransform;
    Vector3 startCamLocalPos;
    float startFOV, addFOV;
    bool lockUpdate;
    Tween FOVTween;

    Coroutine returnCamCoroutine;

    public void Init(CameraData data = null)
    {
        if (data != null) this.data = data;

        thisTransform = transform;
        cam = Camera.main;
        camTransform = cam.transform;
        startCamLocalPos = camTransform.localPosition;

        followTargets = new();

        startFOV = cam.fieldOfView;
        SetDefaultPos();
    }

    public void ReturnToDefault()
    {
        thisTransform.position = MainOffset;
        thisTransform.rotation = Quaternion.Euler(MainAngle);
    }

    void SetDefaultPos()
    {
        MainOffset = thisTransform.position;
        MainAngle = thisTransform.rotation.eulerAngles;
    }

    public void Lock() => lockUpdate = true;
    public void Unlock() => lockUpdate = false;

    public void SetFOV(float addFOV)
    {
        FOVTween.Kill();
        FOVTween = DOTween.To(() => this.addFOV, x => this.addFOV = x, addFOV, 3).SetSpeedBased();
    }

    public int SetFollowTarget(Vector3 targetPos, bool transition = true, int priority = 0, float speed = 0, float time = 0, Vector3 offset = default, Vector3 angle = default) => SetFollowTarget(null, transition, priority, speed, time, targetPos + offset, angle);
    public int SetFollowTarget(Transform target, bool transition = true, int priority = 0, float speed = 0, float time = 0, Vector3 offset = default, Vector3 angle = default)
    {
        int index = followTargets.FindLastIndex(x => x.priority >= priority);
        if (index == -1) index = 0;
        else index++;

        if (offset == default) offset = MainOffset;
        if (angle == default) angle = MainAngle;

        int hash = target ? target.GetHashCode() : offset.GetHashCode();
        FollowTarget followTarget = new FollowTarget(target, offset, Quaternion.Euler(angle), speed, priority, hash);
        followTargets.Insert(index, followTarget);
        if (time > 0) DOTween.Sequence().SetDelay(time).OnComplete(() => RemoveTargetByHash(hash));

        if (transition)
        {
            if (returnCamCoroutine != null) StopCoroutine(returnCamCoroutine);
            returnCamCoroutine = StartCoroutine(SmoothCamera());
        }

        return hash;
    }

    public void RemoveTargetByPriority(int priority, bool smooth = true)
    {
        followTargets.RemoveAll(x => x.priority == priority);
        if (smooth) TrySmoothCamera();
    }

    public void RemoveTargetByHash(int hash, bool smooth = true)
    {
        followTargets.RemoveAll(x => x.hash == hash);
        if (smooth) TrySmoothCamera();
    }

    void TrySmoothCamera()
    {
        if (followTargets.Count == 0) return;

        if (returnCamCoroutine != null) StopCoroutine(returnCamCoroutine);
        returnCamCoroutine = StartCoroutine(SmoothCamera());
    }

    IEnumerator SmoothCamera()
    {
        lockUpdate = true;

        Vector3 startPos = thisTransform.position;
        Quaternion startAngle = thisTransform.rotation;
        float time = Vector3.Distance(startPos, GetTargetPos()) * data.smoothSpeed + Quaternion.Angle(startAngle, followTargets[0].angle) * data.smoothAngle;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            thisTransform.position = Vector3.Slerp(startPos, GetTargetPos(), t / time);
            thisTransform.rotation = Quaternion.Slerp(startAngle, followTargets[0].angle, t / time);
            yield return null;
        }

        lockUpdate = false;
    }

    void Update()
    {
        if (lockUpdate) return;
        float deltaTime = Time.deltaTime;

        if (followTargets.Count > 0)
        {
            Vector3 targetPos = GetTargetPos();
            if (followTargets[0].speed != 0) curFollowSpeed = Mathf.Lerp(curFollowSpeed, followTargets[0].speed, deltaTime * 6);

            if (followTargets[0].speed != 0)
                thisTransform.position = Vector3.Lerp(thisTransform.position, targetPos, deltaTime * curFollowSpeed);
            else thisTransform.position = targetPos;
            thisTransform.rotation = followTargets[0].angle;
        }
    }

    void LateUpdate()
    {
        cam.fieldOfView = startFOV + addFOV;
    }

    void CollisionCam()
    {
        Vector3 camTargetPos = startCamLocalPos;
        if (Physics.SphereCast(thisTransform.position, 0.5f, camTransform.position - thisTransform.position, out RaycastHit hit, 100, data.camCollisionLayerMask))
        {
            float dist = Vector3.Scale(thisTransform.position - hit.point, thisTransform.TransformDirection(startCamLocalPos).normalized).magnitude;
            camTargetPos = new Vector3(startCamLocalPos.x, startCamLocalPos.y, Mathf.Max(-dist, startCamLocalPos.z));
        }
        camTransform.localPosition = Vector3.Slerp(camTransform.localPosition, camTargetPos, Time.deltaTime * 9);
    }

    Vector3 GetTargetPos() => (followTargets[0].transform ? followTargets[0].transform.position : Vector3.zero) + followTargets[0].offset;
}