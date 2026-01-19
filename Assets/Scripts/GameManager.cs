using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton Pattern
    public static GameManager Instance;

    // 게임 상태 
    public enum GameState
    {
        Ready,
        Play,
        GameOver
    }

    // 현재 게임 상태
    public GameState currentState = GameState.Ready;

    // 씬을 다시 불러왔을 때 Restart인지 구분
    public static bool isRestart = false;

    // 참조
    public GameObject player;
    public GameObject startButton;

    // 점수
    [Header("Score")]
    public int score = 0;
    public int bestScore = 0;

    // 항상 보이는 UI
    [Header("UI (Always On)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    // 게임 오버시 UI
    [Header("UI (GameOver)")]
    public GameObject gameOverPanel;

    // 씬 로드 직후 먼저 가장 먼저 실행되는 Awake()
    void Awake()
    {
        // Singleton 초기화
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // 최고 점수 로드
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    // Awake() 다음 실행
    void Start()
    {   
        // 씬이 Restart인지 확인
        if (isRestart)
        {
            // Play 상태로 바꿈
            isRestart = false;
            SetState(GameState.Play);
        }
        else
        {
            // 최초 실행 시
            SetState(GameState.Ready);
        }
    }

    // 상태 변경 함수
    public void SetState(GameState newState)
    {
        // 현재 상태 갱신
        currentState = newState;

        switch (currentState)
        {
            case GameState.Ready:
                EnterReadyState();
                break;

            case GameState.Play:
                EnterPlayState();
                break;

            case GameState.GameOver:
                EnterGameOverState();
                break;
        }
    }
    // Ready 상태
    void EnterReadyState()
    {
        // 게임 정지 (Update 정지)
        Time.timeScale = 0f;
        // 현재 점수만 초기화, 최고 점수는 유지
        score = 0;
        UpdateScoreUI();
        // Start 버튼 표시
        if (startButton != null)
            startButton.SetActive(true);
    }
    // Play 상태
    void EnterPlayState()
    {
        // 게임 재개
        Time.timeScale = 1f;
        score = 0;
        UpdateScoreUI();
        // Start 버튼 숨김
        if (startButton != null)
            startButton.SetActive(false);
    }
    // GameOver 상태
    void EnterGameOverState()
    {
        // 게임 정지
        Time.timeScale = 0f;
        // 최고 점수 저장
        SaveBestScore();
        // GameOver UI 표시
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // 점수 증가 함수
    public void AddScore(int value)
    {
        // Play 상태에만 점수 증가
        if (currentState != GameState.Play) return;

        score += value;
        // 최고 점수 실시간 갱신
        if (score > bestScore)
            bestScore = score;
        // UI 반영
        UpdateScoreUI();
    }
    // 최고 점수 저장
    void SaveBestScore()
    {
        // PlayerPrefs에 최고 점수 저장
        // PlayerPrefs는 게임을 껏다 켜도 유지되는 간단한 값(int, float, string)을 저장하는 Unity 로컬 저장소
        PlayerPrefs.SetInt("BestScore", bestScore);
    }
    // 점수 UI 업데이트 함수
    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {score}";

        if (bestScoreText != null)
            bestScoreText.text = $"Best : {bestScore}";
    }

    // 외부 호출 함수들 
    // Start 버튼 클릭시 
    public void StartGame()
    {
        SetState(GameState.Play);
    }
    // Player 사망 시
    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;
        SetState(GameState.GameOver);
    }
    // Main 버튼 클릭시 씬을 다시 불러옴 (Play 아니고 Ready)
    public void GoToMain()
    {
        // GameOver시 Time.timeScale = 0 이므로 씬 불러오기 전에 1로  
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Restart 버튼 클릭 시 씬 재로드 ( Ready 아니고 Play)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isRestart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}