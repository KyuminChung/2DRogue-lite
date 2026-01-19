using UnityEngine;
using static GameManager;

public class EnemySpawner : MonoBehaviour
{
    // 설정 값
    [Header("References")]
    public GameObject enemyPrefab;
    public Camera mainCamera;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.0f;      // 초기 생성 주기
    public float minSpawnInterval = 0.2f;   // 최소 생성 주기

    [Header("Difficulty")]
    public float difficultyInterval = 30f;  // 30초
    public float spawnDecrease = 0.1f;      // 감소량

    // 내부 계산
    private float spawnTimer = 0f;  // 마지막 적 생성 이후 경과 시간
    private float difficultyTimer = 0f; // 마지막 난이도 상승 이후 경과 시간

    void Update()
    {
        // 게임 Play 상태가 아니면 멈춤
        if (GameManager.Instance.currentState != GameState.Play)
            return;
        // 프레임마다 시간 누적
        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        // =================   적 생성  =================
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

        // ================= 난이도 상승 =================
        if (difficultyTimer >= difficultyInterval)
        {
            // (생성 이후 경과 시간 - 감소량), 최소 생성 주기 중 큰거  
            spawnInterval = Mathf.Max(
                minSpawnInterval,
                spawnInterval - spawnDecrease
            );

            difficultyTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPosition();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
    Vector2 GetRandomPosition()
    {
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // 위
                return new Vector2(Random.Range(-10f, 10f), 6f);
            case 1: // 아래
                return new Vector2(Random.Range(-10f, 10f), -6f);
            case 2: // 왼쪽
                return new Vector2(-11f, Random.Range(-5f, 5f));
            case 3: // 오른쪽
                return new Vector2(11f, Random.Range(-5f, 5f));
        }

        return Vector2.zero;
    }
}