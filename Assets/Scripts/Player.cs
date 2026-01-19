using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    // 이동 속도 5
    public float speed = 5f;

    [Header("HP")]
    // HP 3칸
    public int maxHp = 3;
    public int hp;
    public Image[] hearts;

    [Header("Ray Attack")]
    // 레이 길이 (총알 사거리)
    public float rayRange = 20f;
    // 공격 쿨타임
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    [Header("Line Renderer")]
    // 레이 시각화
    public LineRenderer lineRenderer;

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
        UpdateHearts();

        // LineRenderer 초기 세팅 
        if (lineRenderer != null)
        {
            // 처음에는 레이 숨김
            lineRenderer.enabled = false;
            // 월드 좌표 기준
            lineRenderer.useWorldSpace = true;
            // 선 두께 설정
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            // 흰색 
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            // 머티리얼 없으면 안 보이는 문제 방지
            if (lineRenderer.material == null)
            {
                lineRenderer.material = new Material(Shader.Find("Default-Line"));
            }
        }
    }

    void Update()
    {
        //Ready / GameOver 상태면 입력 무시
        if (GameManager.Instance.currentState != GameState.Play)
            return;
        // ================= 이동 입력 =================
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        // 대각선 속도 보정 (벡터 크기 1로 바꿔서 위 input.x, input.y와 속도 같게)
        input = input.normalized;

        // 이동 방향에 따른 화살표 회전
        if (input != Vector2.zero)
        {
            // Atan(input.y,input.x)는 (x,y) 방향 벡터가 x축 기준 몇 도 방향인지 계산
            // * Mathf.Rad2Deg; 는 라디안을 도(degree)로 반환
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            // Z축만 회전하는 이유: 2D, XY평면에서 움직임, 회전은 Z축 기준
            // -90f: Atan2 기준은 오른쪽(1,0) , 화살표 스프라이트는 (0,1) 보고 있음 -> 기준이 다름
            // 만약 스프라이트가 오른쪽을 본다면 angle + 0f
            // 아래를 본다면 angle + 90f
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        // ================= 공격 입력 =================
        // 마우스 클릭 + 공격 쿨타임 아닐때
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(RayAttack());
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }

    //코루틴 
    IEnumerator RayAttack()
    {
        // 공격 시작시 쿨타임 진입
        canAttack = false;
        // 마우스 → 월드 좌표
        // ScreenToWorldPoint는 항상 Vector3를 반환
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // 방향 벡터: 플레이어 위치에서 출발해서 마우스가 가리키는 위치로 향함
        Vector2 direction = (mouseWorld - transform.position).normalized;

        // RaycastAll: 레이에 닿은 모든 오브젝트 검사
        // RaycastAll(origin, direction, distance);   시작 위치, 방향, 거리
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            direction,
            rayRange
        );

        // 레이 끝점(LineRenderer용)
        // 현재 위치에서 direction 방향으로 rayRange 만큼 이동한 위치
        Vector3 endPoint = transform.position + (Vector3)(direction * rayRange);
        endPoint.z = 0f;

        // 맞은 Enemy 전부 제거
        // foreach (타입 변수 in 컬렉션) { //반복할 코드  }
        //for (int i = 0; i < hits.Length; i++){ RaycastHit2D hit = hits[i]} 와 동일한 결과;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                GameManager.Instance.AddScore(1);
            }
        }

        // LineRenderer 표시
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            // 시작
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
            // 끝
            lineRenderer.SetPosition(1, endPoint);
        }

        // 잠깐 보여주기
        yield return new WaitForSeconds(0.2f);

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        // 8. 쿨타임
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ================= 데미지 처리 =================
    // 체력 감소, UI 갱신
    public void TakeDamage(int damage)
    {
        hp -= damage;
        UpdateHearts();
        if (hp <= 0)
        {
            Die();
        }
    }
    // 하트 UI 갱신
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < hp;
        }
    }
    // 사망 처리
    void Die()
    {
        // 게임 오버 알림
        GameManager.Instance.GameOver();

        // 플레이어 제거
        gameObject.SetActive(false);
    }
    //Enemy와 충돌 시 체력 1 감소 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}