using UnityEngine;
using UnityEngine.UIElements;

public class UiMono : MonoBehaviour
{
    [SerializeField] protected UIDocument m_Doc;
    public VisualElement Root;

    protected virtual void Awake()
    {
        this.Root = this.m_Doc.rootVisualElement;
    }
}
