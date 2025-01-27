using UnityEngine;
using DG.Tweening;

public class TrashBin : Interactable
{
    private float throwInterval=0.3f;
    private Vector3 throwOffset = new Vector3(0f, 0.1f, 0f);
    private float throwCooldown= 0.3f;

    void Update()
    {
        if (owner == null) return;

        throwCooldown -= Time.deltaTime;

        if (throwCooldown <= 0)
        {
            throwCooldown = throwInterval;

            var thrownObj = RushManager.Instance.mainplayer.Stack.RemoveFromStack(-1);
            if (thrownObj == null) return;

            AudioMgr.Instance.Play(AudioEnum.Trash);

            thrownObj.DOJump(transform.TransformPoint(throwOffset), 5f, 1, 0.5f)
                .OnComplete(() =>
                {
                    PoolManager.Instance.ReturnObject(thrownObj.gameObject);
                    AudioMgr.Instance.Play(AudioEnum.Bin);
                });
        }
    }

    float standrange = 0.7f;
    public Vector3 GetStandPoint()
    {
        var randomCircle = UnityEngine.Random.insideUnitCircle * standrange;
        return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
    public void ThrowToBin(HandStack stack)
    {
        var thrownObj = stack.RemoveFromStack(-1);
        
        thrownObj.DOJump(transform.TransformPoint(throwOffset), 5f, 1, 0.5f)
            .OnComplete(() =>
            {
                PoolManager.Instance.ReturnObject(thrownObj.gameObject);
            });
    }
}

