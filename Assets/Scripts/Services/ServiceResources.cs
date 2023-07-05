using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceResources : MonoBehaviour
{

    public ServiceResourcesInfo data;
    private static Dictionary<string, Object> resourcesStatic;

    private void Awake()
    {
        if (resourcesStatic == null)
        {
            resourcesStatic = new Dictionary<string, Object>();
            for (int i = 0; i < data.resourcesList.Count; i++)
                resourcesStatic.Add(data.resourcesList[i].key, data.resourcesList[i].value);
        }
    }

    public static Object Load(string path)
    {
        if (resourcesStatic.ContainsKey(path))
        {
            return resourcesStatic[path];
        }
        else
            return null;
    }

    public static Object[] LoadAll(string path)
    {
        List<Object> objList = new List<Object>();

        foreach (KeyValuePair<string, Object> kp in resourcesStatic)
        {
            if (kp.Key.StartsWith(path))
                objList.Add(kp.Value);
        }

        return objList.ToArray();
    }

    public static T Load<T>(string path) where T : Object
    {
        if (resourcesStatic.ContainsKey(path))
            return (T)resourcesStatic[path];
        else
            return null;
    }

    public static T[] LoadAll<T>(string path) where T : Object
    {
        List<T> objList = new List<T>();

        foreach (KeyValuePair<string, Object> kp in resourcesStatic)
        {
            if (kp.Key.StartsWith(path))
                objList.Add((T)kp.Value);
        }

        return objList.ToArray();
    }
    
    public static void LoadImage(string url, Image image){
        if (!string.IsNullOrEmpty(url))
            Loader.Instance.LoadImage(url, image, () => {
                if (image != null){
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