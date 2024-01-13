using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour
where T : SingletonMono<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Debug.LogWarning($"There might be more than 1 {this.ToString()} in the scene.", this);
        }
    }
}
