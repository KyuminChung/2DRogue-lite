using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;                                     // Singleton Pattern
  
    public enum GameState                                                   // 게임 상태
    {
        Ready,
        Play,
        GameOver
    }

    public GameState currentState = GameState.Ready;                        // 현재 게임 상태(초기값 Ready)

    public static bool isRestart = false;                                   // 씬을 다시 불러왔을 때 Restart인지 구분

    // 참조
    public GameObject player;                                               // 플레이어
    public GameObject startButton;                                          // Start 버튼

    // 점수
    [Header("Score")]
    public int score = 0;                                                   // 현재 점수
    public int bestScore = 0;                                               // 최고 점수

    // 항상 보이는 UI
    [Header("UI (Always On)")]
    public TextMeshProUGUI scoreText;                                       // 현재 점수 텍스트
    public TextMeshProUGUI bestScoreText;                                   // 최고 점수 텍스트

    // StringBuilder (GC 최소화용)
    private StringBuilder _scoreSb = new StringBuilder(32);                 
    private StringBuilder _bestScoreSb = new StringBuilder(32);

    // 게임 오버시 UI
    [Header("UI (GameOver)")]
    public GameObject gameOverPanel;                                        // 게임오버 패널 오브젝트

    void Awake()
    {      
        if (Instance == null) Instance = this;                              // 싱글톤 초기화
        else { Destroy(gameObject); return; }

        bestScore = PlayerPrefs.GetInt("BestScore", 0);                     // 최고 점수 로드
        AudioManager.Instance?.StopBGM();                                   // 씬 로드 직후 BGM 끄기
    }

    void Start()
    {   
        if (isRestart)                                                      // 씬이 Restart인지 확인
        {
            isRestart = false;                                              
            SetState(GameState.Play);                                       // Play 상태로 바꿈

        }
        else
        {
            SetState(GameState.Ready);                                      // 최초 실행 시 Ready
        }
    }

    public void SetState(GameState newState)                                // 상태 변경 함수
    {
        currentState = newState;                                            // 현재 상태 갱신

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
 
    void EnterReadyState()                                                  
    {        
        Time.timeScale = 0f;                                                // 게임 정지 (Update 정지)
        
        score = 0;                                                          // 현재 점수만 초기화, 최고 점수는 유지
        UpdateScoreUI();                                                    // 점수 UI 갱신
        
        if (startButton != null)                                            // Start 버튼 표시 (Ready 만)
            startButton.SetActive(true);
    }
    
    void EnterPlayState()
    {
        Time.timeScale = 1f;                                                // 게임 재개

        AudioManager.Instance?.PlayBGM();                                   // Play 상태에서만 BGM 재생
            
        score = 0;                                                          // 점수 초기화
        UpdateScoreUI();                                                    // 점수 UI 갱신
        
        if (startButton != null)                                            // Start 버튼 숨김
            startButton.SetActive(false);
    }
    
    void EnterGameOverState()
    {
        Time.timeScale = 0f;                                                // 게임 정지
    
        SaveBestScore();                                                    // 최고 점수 저장
     
        if (gameOverPanel != null)                                          // GameOver UI 표시
            gameOverPanel.SetActive(true);
    }

    public void AddScore(int value)                                         // 점수 증가 함수
    {
        if (currentState != GameState.Play) return;                         // Play 상태에만 점수 증가

        score += value;                                                     // 점수 증가
        
        if (score > bestScore)                                              // 최고 점수 실시간 갱신
            bestScore = score;       

        UpdateScoreUI();                                                    // UI 반영
    }
    
    void SaveBestScore()                                                    // PlayerPrefs에 최고 점수 저장
    {     
        // PlayerPrefs는 게임을 껏다 켜도 유지되는 간단한 값(int, float, string)을 저장하는 Unity 로컬 저장소
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }
  
    void UpdateScoreUI()                                                    // 점수 UI 업데이트 함수
    {
        if (scoreText != null)
        {
            _scoreSb.Clear();                                               // 이전 내용 제거
            _scoreSb.Append("Score : ");                                    // 라벨 추가
            _scoreSb.Append(score);                                         // 점수 추가
            scoreText.text = _scoreSb.ToString();                           // 텍스트 적용
        }

        if (bestScoreText != null)
        {
            _bestScoreSb.Clear();
            _bestScoreSb.Append("Best : ");
            _bestScoreSb.Append(bestScore);
            bestScoreText.text = _bestScoreSb.ToString();
        }
    }

    // 외부 호출 함수들  
    public void StartGame()
    {
        SetState(GameState.Play);                                           // Start 버튼 클릭 시 Play로 전환
    }
    // Player 사망 시
    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;
        SetState(GameState.GameOver);                                       // GameOver 상태로 전환
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.GameOverClip); // 게임오버 SFX 재생
        AudioManager.Instance?.StopBGM();                                   // Play 중이던 BGM 정지
    }
   
    public void GoToMain()
    {   // Main 버튼 클릭시 씬을 다시 불러옴 (Play 아니고 Ready)    
        Time.timeScale = 1f;    // GameOver시 Time.timeScale = 0 이므로 씬 불러오기 전에 1로 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);         // 현재 씬 다시 로드
    }
    
    public void RestartGame()
    {   // Restart 버튼 클릭 시 씬 재로드 ( Ready 아니고 Play)
        Time.timeScale = 1f;                                                // 씬 로드 전에 timeScale 복구
        isRestart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);         // 현재 씬 다시 로드
    }
}