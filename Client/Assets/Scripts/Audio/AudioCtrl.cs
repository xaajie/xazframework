using UnityEngine;
public class AudioCtrl : MonoBehaviour
{

    [SerializeField]
    internal protected int audioId;

    [SerializeField]
    internal protected bool playOnAwake;

     void Awake()
    {
        if (playOnAwake)
        {
            Play();
        }
    }
    public void Play()
    {
        if (audioId > 0)
        {
            AudioMgr.Instance.Play(audioId);
        }
    }
    //定时器函数

}
