using UnityEngine;

public class CountKeep : MonoBehaviour
{
    private static CountKeep _instance;

    public static CountKeep Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject countKeepObject = new GameObject("CountKeep");
                _instance = countKeepObject.AddComponent<CountKeep>();
                DontDestroyOnLoad(countKeepObject);
            }
            return _instance;
        }
    }

    private static int count = 5; // Default count value

    public static int Count
    {
        get { return count; }
        set
        {
            count = value;
            SaveCount(); // Save count value whenever it's modified
        }
    }

    public static void SaveCount()
    {
        PlayerPrefs.SetInt("Count", count);
        PlayerPrefs.Save(); // Save changes to PlayerPrefs immediately
    }

    public static void LoadCount()
    {
        count = PlayerPrefs.GetInt("Count", 5); // Default to 5 if not found
    }
}
