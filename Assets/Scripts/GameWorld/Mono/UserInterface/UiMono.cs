using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiMono : MonoBehaviour
{
    protected UIDocument m_Doc;
    public VisualElement Root;

    [SerializeField] protected bool InitialVisibility;

    protected virtual void Awake()
    {
        this.m_Doc = this.GetComponent<UIDocument>();
        this.Root = this.m_Doc.rootVisualElement;
        this.Root.visible = this.InitialVisibility;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            this.m_Doc = this.GetComponent<UIDocument>();
            this.Root = this.m_Doc.rootVisualElement;
            this.Root.visible = this.InitialVisibility;
        }
    }
#endif
}
