using UnityEngine;

public abstract class abstractEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHp = 1;           // 최대 체력
    [SerializeField] protected float moveSpeed = 2f;    // 이동 속도

    [Header("Death")]
    [SerializeField] private float dieDuration = 1f;  // 죽는 애니 보여줄 시간

    [Header("Despawn Bounds")]                          // 디스폰 X, Y 최대, 최소
    [SerializeField] private float despawnMinX = -12f;  
    [SerializeField] private float despawnMaxX = 12f;
    [SerializeField] private float despawnMinY = -7f;
    [SerializeField] private float despawnMaxY = 7f;

    [Header("Drop")]
    [SerializeField] private GameObject potionPrefab;   // 드롭할 포션 프리팹
    [SerializeField, Range(0f, 1f)] private float potionDropChance = 0.1f; // 10% 드롭 확률

    protected int hp;                                   // 현재 체력
    protected bool isDead = false;                      // 죽는 중/ 사망 여부
    private Vector2 direction;                          // 이동 방향 (플레이어 쪽)

    protected Animator anim;                            // 애니메이터 참조
    protected Collider2D col;                           // 콜라이더 참조

    protected virtual void Awake()
    {
        hp = maxHp;                                     // 시작 체력을 최대 체력으로 설정 (Bat:1, Zombie:2)
        anim = GetComponent<Animator>();                // 같은 오브젝트의 Animator 가져오기
        col = GetComponent<Collider2D>();               // 같은 오브젝트의 Collider2D 가져오기
                                                        // Polygon Collider 2D도 Collider2D의 자식 타입이라 가져옴
    }
    protected void Start()
    {
        // 방향 정하기
        if (GameManager.Instance != null && GameManager.Instance.player != null)    // 게임매니저, 플레이어 있을 때
        {
            // 스폰 시점에 플레이어 쪽을 바라보는 방향을 1회 계산(이후 직진)
            direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead == true) return;                     // 죽는 중이면 이동 중단
        // 최초에 정한 direction으로 계속 직진 이동
        transform.position += (Vector3)(direction * moveSpeed * Time.fixedDeltaTime);      
    }
    protected virtual void LateUpdate()
    {
        if (isDead) return;                             // 죽는 모션 중엔 디스폰으로 삭제되지 않게 방지
       
        Vector3 p = transform.position;                 // 현재 위치 가져오기

        // 화면/ 맵 밖으로 나가면 재거 
        if (p.x < despawnMinX || p.x > despawnMaxX || p.y < despawnMinY || p.y > despawnMaxY)
            Destroy(gameObject);
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;                             // 이미 죽는 중이면 추가 피해 무시
        
        hp -= amount;                                   // 체력 감소

        if (hp > 0)                                     // 살아있으면 피격 애니 재생
        {
            anim?.SetTrigger("hit");                        
            return;
        }

        // 체력이 0 이하가 되면 사망 시작
        StartDeath();
    }

    private void StartDeath()
    {
        if (isDead) return;                             // 사망 중복 실행 방지
        isDead = true;                                  // 사망 상태로 전환

        if (col != null) col.enabled = false;           // 죽는 동안 플레이어 피해/충돌 차단

        anim?.ResetTrigger("hit");                      // hit 트리거가 남아있으면 제거
        anim?.SetTrigger("die");                        // 죽는 애니 재생

        TryDropPotion();                                // 포션 드롭 시도(확률)
        GameManager.Instance?.AddScore(1);              // 점수 +1 (피해로 죽었을 때만)

        StartCoroutine(CoDestroyAfter(dieDuration));    // 죽는 모션을 다 보여준 후 제거
    }

    private void TryDropPotion()
    {
        if (potionPrefab == null) return;               // 포션 프리팹이 없으면 드롭 불가
        if (Random.value >= potionDropChance) return;   // 확률 실패면 드롭 안 함

        Instantiate(potionPrefab, transform.position, Quaternion.identity); // 포션 생성
    }

    private System.Collections.IEnumerator CoDestroyAfter(float seconds)    // 죽는 애니 보여줄 시간만큼 대기
    {
        yield return new WaitForSeconds(seconds);
        Die();
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}