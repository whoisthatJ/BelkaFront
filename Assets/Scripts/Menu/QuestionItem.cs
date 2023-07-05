using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour{
    [SerializeField] private Image _preview;

    public void SetPreview(string url){
        ServiceResources.LoadImage(url, _preview);
    }
}
