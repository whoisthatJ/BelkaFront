using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class FriendListItem : MonoBehaviour
{
    public Image avatar;
    public string ID { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public int RankValue { get; set; }
    public string RankType { get; set; }

    public abstract void Init(UserData data);
    public abstract string GetName();
    public bool IsStatus{ get; set; }

    public virtual void SetStatus(bool isStatus){
        IsStatus = isStatus;
    }

    public void LoadImage(string url, Image image){
        if (!string.IsNullOrEmpty(url))
        Loader.Instance.LoadImage(url, image, () => {
            if (image != null && image.sprite != null){
                float aspectRatio = (float) image.sprite.texture.width / (float) image.sprite.texture.height;
                image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
                //Sprite size bug fixed
                var sprite = image.sprite;
                image.sprite = null;
                image.sprite = sprite;
            }
        });
    }
}
