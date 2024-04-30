#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class AnimalFix : MonoBehaviour
{
    void OnEnable()
    {
        Transform transform = base.transform.GetChild(0);
        MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
        transform.localPosition -= transform.forward * ((meshRenderer.bounds.size.y / base.transform.localScale.y) / 4);
    }
}
#endif