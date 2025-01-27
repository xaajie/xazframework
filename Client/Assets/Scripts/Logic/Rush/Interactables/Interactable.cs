using UnityEngine;


[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    protected GameObject owner { get; private set; }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(XazConfig.TagDefine.PLAYER))
        {
            owner = other.gameObject;
            if (owner != null) OnPlayerEnter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(XazConfig.TagDefine.PLAYER))
        {
            OnPlayerExit();
            owner = null;
        }
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
}

