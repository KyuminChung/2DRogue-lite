using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;                               

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 2f;                           // 총알 자동 사라질 시간

    [Header("FX")]
    [SerializeField] private ParticleSystem hitFxPrefab;                    // 적 맞을때 나오는 파티클

    private float timer;                                                    // 활성화 뒤 경과 시간
    private BulletPool pool;
    private bool initialized;                                               // Init 호출 여부
    private bool returned;                                                  // 중복 ReturnToPool 방지 플래그

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        timer = 0f;                                                         // 수명 타이머 초기화
        initialized = false;                                                // Init 전엔 동작X
        returned = false;                                                   // 중복 반환 플래그 초기화
        if (rb != null) rb.linearVelocity = Vector2.zero;                   // 재사용 시 속도 초기화
    }

    public void Init(BulletPool pool, Vector2 dir, float speed)             
    {
        this.pool = pool;                                                   
        initialized = true;                                                 // Update 동작 허용
        rb.linearVelocity = dir.normalized * speed;                          
        // 회전: 스프라이트 기본이 "왼쪽(-X)"을 보고 있음, 총알 쏜 방향으로 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
    }

    void Update()
    {
        if (!initialized || returned) return;                               // Init 전 / 반환 후엔 아무것도 안함

        timer += Time.deltaTime;                                            // 경과 시간 누적
        if (timer >= lifeTime) ReturnToPool();                              // 수명 다하면 풀로 반환
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (returned) return;                                               // 이미 반환 처리 중이면 무시

        // 적과 충돌시 데미지 + 피격 FX
        if (other.TryGetComponent<abstractEnemy>(out var enemy))
        {
            enemy.TakeDamage(1);                                            // 적 체력 1 감소
            SpawnHitFx();                                                   // 피격 FX 생성
        }
        ReturnToPool();
    }

    private void SpawnHitFx()
    {
        if (hitFxPrefab == null) return;                                    // 프리팹 없으면 FX 생략
        Instantiate(hitFxPrefab, transform.position, Quaternion.identity);  // 피격 FX 생성
    }

    private void ReturnToPool()
    {
        if (returned) return;                                               // 중복 호출방지
        returned = true;                                                    // 한번만 반환

        if (rb != null) rb.linearVelocity = Vector2.zero;                   // 반환 전 속도 초기화

        if (pool == null)                                                   // 예외처리
        { 
            gameObject.SetActive(false); 
            return; 
        }
        
        pool.Return(this);                                                   // 풀에 넣기
    }
}