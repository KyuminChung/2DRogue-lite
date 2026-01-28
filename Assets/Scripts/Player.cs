using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    private float speed = 5f;                                                // 이동 속도 5

    [Header("HP")]    
    private int MaxHp = 3;                                                   // HP 3칸    
    [SerializeField] private Image[] hearts;                                 // 하트 UI 배열

    [SerializeField] private CameraShake cameraShake;                        // 피격 카메라 흔들림

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private int HP;

    // 외부에서 읽기만 가능한 프로퍼티
    public int hp => HP;
    public int maxHp => MaxHp;
    public Vector2 MovementDirection => movementDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        HP = maxHp;
        UpdateHearts();                                                      // HP UI 반영
        // cameraShake가 비어있으면 메인 카메라에서 찾아 연결
        if (cameraShake == null && Camera.main != null)
            cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Update()
    {        
        if (GameManager.Instance.currentState != GameState.Play)             //Ready / GameOver 상태면 입력 무시
            return;
        // ================= 이동 입력 =================
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
        // 대각선 속도 보정 (벡터 크기 1로 바꿔서 위 input.x, input.y와 속도 같게)
        movementDirection = movementDirection.normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementDirection * speed;
    }

    
    public void TakeDamage(int damage)
    {
        HP -= damage;
        UpdateHearts();
        cameraShake?.Shake(0.12f, 0.12f);
        if (HP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        HP = Mathf.Min(HP + amount, maxHp);                                 // 최대 체력 초과하지 않게 회복
        UpdateHearts();                                                     // 하트 UI 갱신
    }

    void UpdateHearts()                                                     // 하트 UI 갱신
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)                             // 하트 개수만큼 반복
        {
            hearts[i].enabled = i < HP;                                     // hp 미만 인덱스만 켜기(나머지 끄기)
        }
    }
    
    void Die()
    {        
        GameManager.Instance.GameOver();                                    // 게임 오버 알림
        gameObject.SetActive(false);                                        // 플레이어 제거
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<abstractEnemy>(out var enemy))
        {
            TakeDamage(1);                                                  // Enemy와 충돌 시 체력 1 감소 
            Destroy(enemy.gameObject);                                      // 적 제거
        }
    }
}