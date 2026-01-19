using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적 이동속도 3
    public float speed = 3f;

    private Rigidbody2D rb;

    // 플레이어의 Transform 컴포넌트를 참조하는 변수
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Player 태그를 가진 오브젝트 찾기
        GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
        if (targetObj != null)
        {
            target = targetObj.transform;
        }
    }
    // FixedUpdate는 Update와 달리 고정된 시간 간격
    void FixedUpdate()
    {
        if (target == null) return;

        // 1. 방향 벡터 계산
        // transform.position은 Enemy자신의 위치, 변수 선언 없이 바로 사용 가능(MonoBehaviour가 transform을 제공)
        // 의미 target(player)위치 - Enemy위치 : Enemy에서 Player까지 가기 위해 이동해야 할 방향 
        Vector2 direction = (target.position - transform.position).normalized;

        // 2. 플레이어 쪽으로 이동
        rb.linearVelocity = direction * speed;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);
        }
    }
}
