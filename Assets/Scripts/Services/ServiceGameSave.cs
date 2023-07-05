using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class ServiceGameSave : MonoBehaviour
{
    public static ServiceGameSave Instance
    {
        get;
        private set;
    }

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
        Init();
    }

    private BinaryFormatter binaryFormatter = new BinaryFormatter();
    private const string DIR = "/data/";

    void Init()
    {
        if (!Directory.Exists(Application.persistentDataPath + DIR))
        {
            Directory.CreateDirectory(Application.persistentDataPath + DIR);
        }
    }

    public void Save(System.Object saveObject, string fileName)
    {
        FileStream file = File.Create(Application.persistentDataPath + DIR + fileName + ".bin");
        binaryFormatter.Serialize(file, saveObject);
        file.Close();
    }

    public System.Object Load(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + DIR + fileName + ".bin"))
        {
            FileStream file = File.Open(Application.persistentDataPath + DIR + fileName + ".bin", FileMode.Open);
            System.Object saveObject = binaryFormatter.Deserialize(file);
            file.Close();

            return saveObject;
        }
        return null;
    }

	public void DeleteSave(string fileName)
    {
        if (Directory.Exists(Application.persistentDataPath + DIR + fileName + ".bin"))
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + DIR + fileName + ".bin");

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }
#if UNITY_EDITOR
//     [SHOW_IN_HIER]

    [MenuItem("Custom Editor/RESET DATA (WARNING)!!!")]
#endif
    [ContextMenu("RESET DATA (WARNING)")]
    public static void ResetData()
    {
        if (Directory.Exists(Application.persistentDataPath + DIR))
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + DIR);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        PlayerPrefs.DeleteAll();
    }
}