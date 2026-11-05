using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- 1. Reemplazamos UnityEngine.UI por TMPro
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("UI del Menú (TextMeshPro)")]
    public TMP_InputField playerNameInput;     // <-- 2. Cambiado a TMP_InputField
    public TextMeshProUGUI bestScoreMenuText;  // <-- 3. Cambiado a TextMeshProUGUI

    [Header("Datos entre escenas (Jugador Actual)")]
    public string m_PlayerName;

    [Header("Datos entre sesiones (Récord Guardado)")]
    public string m_BestPlayerName;
    public int m_BestPlayerScore;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar el récord guardado al abrir el juego
        LoadDataGame();
    }

    private void Start()
    {
        // Si asignaste un texto en el menú, muestra el récord cargado
        if (bestScoreMenuText != null)
        {
            bestScoreMenuText.text = $"Best Score : {m_BestPlayerName} : {m_BestPlayerScore}";
        }
    }

    public void StartGame()
    {
        // Guardamos el nombre del InputField para llevarlo a la siguiente escena
        if (playerNameInput != null)
        {
            m_PlayerName = playerNameInput.text;
        }

        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        SaveDataGame();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // --- SISTEMA DE GUARDADO ENTRE SESIONES (JSON) ---

    [System.Serializable]
    class SaveData
    {
        public string bestPlayerName;
        public int bestPlayerScore;
    }

    public void SaveDataGame()
    {
        SaveData data = new SaveData();
        data.bestPlayerName = m_BestPlayerName;
        data.bestPlayerScore = m_BestPlayerScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadDataGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            m_BestPlayerName = data.bestPlayerName;
            m_BestPlayerScore = data.bestPlayerScore;
        }
    }
}