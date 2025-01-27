using System.Collections.Generic;
using UnityEngine;

public class ParticleTargetMove : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem par;
    ParticleSystem.Particle[] arrPar;
    int arrCount = 0;
    private float speed = 0.4f;
    private float delayTime = 0.8f;
    private Vector3 targetpos;
    private List<int> flyinv = new List<int>();
    private bool canMove =false;
    void Update()
    {
        if (canMove && par.isPlaying && par)
        {
            arrCount = par.GetParticles(arrPar);
            for (var i = 0; i < arrCount; i++)
            {
                arrPar[i].position = Vector3.MoveTowards(arrPar[i].position, targetpos, Time.deltaTime / speed * 20);
                if(arrPar[i].position== targetpos)
                {
                    if (flyinv.IndexOf(i) == -1)
                    {
                        flyinv.Add(i);
                    }
                }
                else
                {
                    arrPar[i].position = Vector3.MoveTowards(arrPar[i].position, targetpos, Time.deltaTime / speed * 20);
                }
            }
            par.SetParticles(arrPar, arrCount);
            if (flyinv.Count >= arrCount)
            {
                canMove = false;
                par.Stop();
                this.gameObject.SetActive(false);
            }
        }
    }

    //新加了一个play，可以根据不同需求去设置并且播放。
    public void Play(Vector3 pos)
    {
        if (!canMove)
        {
            this.gameObject.SetActive(true);
            if (par == null)
            {
                par = GetComponentInChildren<ParticleSystem>();
                arrPar = new ParticleSystem.Particle[par.main.maxParticles];
            }
            Invoke("StatFlag", delayTime);
            targetpos = pos;
            flyinv.Clear();
        }
    }

    private void StatFlag()
    {
        canMove = true;
        if (par.isStopped)
        {
            par.Play(true);//供外边调用
        }
    }

}