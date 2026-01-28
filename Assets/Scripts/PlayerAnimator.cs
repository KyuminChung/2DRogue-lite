using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] Animator anim;                                             // 애니메이터
    [SerializeField] SpriteRenderer sr;                                         // 스프라이트 렌더러 (좌/우 반전)

    int lastDir = 1;                                                            // 기본 Down
    bool lastFlip = false;                                                      // 기본 왼쪽

    void Update()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Play)    // Play 상태가 아니면: 입력 처리 X, Idle만 고정
        {
            anim.SetFloat("speed", 0f);                                         // Idle 고정
            anim.SetFloat("dir", lastDir);                                      // 마지막 방향 유지
            
            return;
        }
        
        float x = Input.GetAxisRaw("Horizontal");                               // 수평 입력
        float y = Input.GetAxisRaw("Vertical");                                 // 수직 입력
        Vector2 v = new Vector2(x, y);                                          // 입력 벡터
        bool moving = v.sqrMagnitude > 0.001f;                                  // 움직이는지 판단

        // 1) flipX 갱신 (x가 있을 때만)
        if (x > 0) lastFlip = true;                                             // 오른쪽 입력이면 flipX = true
        else if (x < 0) lastFlip = false;
        sr.flipX = lastFlip;

        // 2) dir 갱신 (움직일 때만)
        if (moving)
            lastDir = CalcDir4(x, y, lastDir);

        // 3) animator 반영
        anim.SetFloat("speed", moving ? 1f : 0f);                               // 이동/정지
        anim.SetFloat("dir", lastDir);                                          // 방향(0~3)

        // 4) shooting / reload
        anim.SetBool("isShooting", Input.GetMouseButton(0));                    // 클릭 중이면 사격 상태

        if (Input.GetKeyDown(KeyCode.R))                                        // R 누르면
            anim.SetTrigger("reload");                                          // 재장전 트리거
    }

    // 0 Up, 1 Down, 2 UpLeft(=UpRight via flip), 3 DownLeft(=DownRight via flip)
    int CalcDir4(float x, float y, int fallback)
    {
        // x가 거의 없으면 상/하 판정
        if (Mathf.Abs(x) < 0.001f)
        {
            if (y > 0) return 0;                                                // 위
            if (y < 0) return 1;                                                // 아래
            return fallback;                                                    // 입력 없으면 이전 유지
        }

        // 대각선(좌상/좌하) + flipX로 우상/우하 표현
        if (y > 0) return 2;                                                    // 위 대각
        if (y < 0) return 3;                                                    // 아래 대각

        // 마지막이 위 계열이면 좌상, 아래 계열이면 좌하 유지
        if (fallback == 0 || fallback == 2) return 2;
        return 3;
    }
}