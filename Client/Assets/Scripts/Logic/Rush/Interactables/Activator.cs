using UnityEngine;

public class Actor : Interactable
{
    [SerializeField] private GameObject linkedObject;

    protected override void OnPlayerEnter()
    {
        linkedObject.SetActive(true);
    }

    protected override void OnPlayerExit()
    {
        linkedObject.SetActive(false);
    }
}

