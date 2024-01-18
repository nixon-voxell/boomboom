using UnityEngine;
using UnityEngine.UIElements;

public class UISingletonMono<T> : SingletonMono<T>
where T : SingletonMono<T>
{
    [SerializeField] protected UIDocument m_Doc;
    protected VisualElement m_Root;

    protected override void Awake()
    {
        base.Awake();
        this.m_Root = this.m_Doc.rootVisualElement;
    }
}
