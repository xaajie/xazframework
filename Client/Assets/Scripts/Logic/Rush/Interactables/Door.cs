using UnityEngine;
using DG.Tweening;
using Xaz;

public class Door : Interactable
{
    [SerializeField] private Transform doorTransform;
    private float openDuration = 0.4f;
    private float closeDuration = 0.5f;

    Vector3 openAngle = new Vector3(0f, 90f, 0f);
    private bool isOpeningDoor = false;

    protected override void OnPlayerEnter()
    {
        OpenDoor(owner.transform);
    }

    protected override void OnPlayerExit()
    {
        CloseDoor();
    }


    public void OpenDoor(Transform interactor)
    {
        if (!isOpeningDoor)
        {
            isOpeningDoor = true;
            //Vector3 direction = (interactor.position - transform.position).normalized;
            //只能向外开
            Vector3 direction = (RushManager.Instance.trashBin.transform.position- transform.position).normalized;
            float dotProduct = Vector3.Dot(direction, transform.forward);
            Vector3 targetAngle = openAngle * Mathf.Sign(dotProduct);
            doorTransform.DOLocalRotate(targetAngle, openDuration, RotateMode.LocalAxisAdd);
            //doorTransform.DOLocalRotate(new Vector3(0,-90,0), openDuration, RotateMode.LocalAxisAdd);
        }
    }

    public void CloseDoor()
    {
        doorTransform.DOLocalRotate(Vector3.zero, closeDuration).SetEase(Ease.OutBounce);
        isOpeningDoor = false;
    }
}

