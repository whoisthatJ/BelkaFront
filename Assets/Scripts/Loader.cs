using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Loader : MonoBehaviour{
    public static Loader Instance;

    private void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private IEnumerator DownloadImage(string url, Image image, Action callback = null) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
        } else {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            if (image != null)
                image.sprite = spriteToUse;
        }
        callback?.Invoke();
    }

    public void LoadImage(string url, Image image, Action callback = null) {
        StartCoroutine(DownloadImage(url, image, callback));
    }
}
