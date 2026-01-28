using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject zombiePrefab;                   // 좀비
    [SerializeField] private GameObject batPrefab;                      // 박쥐

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 2f;                  // 스폰 간격

    [Header("Spawn Area (world coords)")]                               // 화면 안 (X, Y)
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    [Header("Outside offsets")]                                         // 화면 밖 (X, Y)
    [SerializeField] private float topY = 6f;
    [SerializeField] private float bottomY = -6f;
    [SerializeField] private float leftX = -11f;
    [SerializeField] private float rightX = 11f;

    [Range(0f, 1f)]                                                     // 스폰 확률
    [SerializeField] private float zombieRatio = 0.8f; // 8:2

    private float t;                                                    // 마지막 스폰 이후 경과 시간

    void Update()
    {
        t += Time.deltaTime;                                            // 시간 누적
        if (t < spawnInterval) return;                                  // 리스폰 시간 안지났으면 종료
        t = 0f;                                                         // 타이머 리셋
        SpawnOne();                                                     // 적 스폰
    }

    private void SpawnOne()                                             // 스폰 
    {
        if (zombiePrefab == null || batPrefab == null) return;     
        
        Vector2 pos = GetRandomOutsidePosition();                       // 화면 밖 랜덤 위치 계산(스폰 위치)
        // 좀비/박쥐 확률로 선택
        GameObject prefab = (Random.value < zombieRatio) ? zombiePrefab : batPrefab; 
        Instantiate(prefab, pos, Quaternion.identity);                  // 선택된 적을 스폰 위치에 생성
    }

    private Vector2 GetRandomOutsidePosition()                          // 랜덤 위치 계산 
    {
        int side = Random.Range(0, 4);                                  // 0:위 1:아래 2:왼쪽 3:오른쪽

        switch (side)
        {
            case 0: // 위
                return new Vector2(Random.Range(minX, maxX), topY);
            case 1: // 아래
                return new Vector2(Random.Range(minX, maxX), bottomY);
            case 2: // 왼쪽
                return new Vector2(leftX, Random.Range(minY, maxY));
            default: // 오른쪽
                return new Vector2(rightX, Random.Range(minY, maxY));
        }
    }
}