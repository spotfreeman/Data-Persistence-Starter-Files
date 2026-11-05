using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public TextMeshProUGUI BestScoreText; // <- Arrastra aquí el objeto "BestScoreText" del Canvas
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;

    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        // Mostrar el nombre del jugador actual junto al puntaje inicial (0)
        UpdateScoreUI();
        // Mostrar el mayor puntaje guardado al iniciar la partida
        UpdateBestScoreUI();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        UpdateScoreUI();

        // Si mientras juega supera el mayor puntaje, actualizamos el récord en pantalla
        if (DataManager.Instance != null && m_Points > DataManager.Instance.m_BestPlayerScore)
        {
            DataManager.Instance.m_BestPlayerScore = m_Points;
            DataManager.Instance.m_BestPlayerName = DataManager.Instance.m_PlayerName;
            UpdateBestScoreUI();
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        // Guardamos en disco (JSON) al terminar la partida
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveDataGame();
        }
    }

    void UpdateScoreUI()
    {
        string currentName = (DataManager.Instance != null) ? DataManager.Instance.m_PlayerName : "Player";
        ScoreText.text = $"Score ({currentName}) : {m_Points}";
    }

    void UpdateBestScoreUI()
    {
        if (DataManager.Instance != null && BestScoreText != null)
        {
            BestScoreText.text = $"Best Score : {DataManager.Instance.m_BestPlayerName} : {DataManager.Instance.m_BestPlayerScore}";
        }
    }
}