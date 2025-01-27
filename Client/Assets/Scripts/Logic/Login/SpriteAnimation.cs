using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] sprites; // �洢����֡����������֡
    public float framesPerSecond = 10.0f; // ÿ�벥�ŵ�֡��

    public Image image;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayAnimation()
    {
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                image.sprite = sprites[i];
                yield return new WaitForSeconds(1f / framesPerSecond);
            }
        }
    }
}

