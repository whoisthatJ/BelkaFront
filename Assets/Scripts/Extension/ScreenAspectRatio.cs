using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAspectRatio : MonoBehaviour
{
    public static string CalcAspect()
    {
        float r = (float)Screen.width / (float)Screen.height;
        string _r = r.ToString("F2");
        string ratio = _r.Substring(0, 4);
        
        Debug.Log(ratio);
        return ratio;
    }
}
