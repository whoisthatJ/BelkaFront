using UnityEngine;
public class ServiceXML : MonoBehaviour
{
    public static ServiceXML Instance
    {
        get;
        private set;
    }

    private const string PATH = "Xmls/";

    void Awake()
    {
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

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        
    }

    public void LocalizeNames()
    {
        
    }
}