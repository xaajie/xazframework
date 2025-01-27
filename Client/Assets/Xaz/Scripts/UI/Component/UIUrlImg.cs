using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Xaz;

public class UIUrlImg : Image,IControl
{
    public string imageUrl = ""; // 你的图片URL

    private Texture2D texture;



    public void SetUrlImg(string url)
    {
        if(imageUrl == url)
        {
            return;
        }
        imageUrl = url;
        StartCoroutine(DownloadImage(imageUrl));
    }

    private IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                this.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    protected override void OnDestroy()
    {
        // 确保在销毁时释放内存
        imageUrl = string.Empty;
        if (texture != null)
        {
            Destroy(texture);
        }
        base.OnDestroy();
    }
}