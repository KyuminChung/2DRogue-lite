using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool pool;                               // 총알을 꺼낼 풀
    [SerializeField] private Transform firePoint;                           // 총알이 생성될 위치
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 12f;                       // 총알 속도
    [SerializeField] private float fireRate = 0.15f;                        // 연사 간격(초)

    [Header("FX")]
    [SerializeField] private ParticleSystem muzzleFxPrefab;                 // 발사 이펙트 프리팹
    [SerializeField] private Transform muzzlePoint;                         // 발사 이펙트 위치
    private float t;                                                        // 발사 쿨타임 타이머

    void Awake()
    {
        anim ??= GetComponent<Animator>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        t += Time.deltaTime;                                                // 시간 누적(쿨타임 계산)       
        if (!Input.GetMouseButton(0)) return;                               // 마우스 클릭 중이 아니면 발사 안 함
        if (t < fireRate) return;                                           // 쿨타임이 안 찼으면 발사 안 함

        t = 0f;                                                             // 발사할 때 타이머 리셋
        Shoot();
    }

    void Shoot()
    {
        Vector2 dir = GetAimDir();                                          // 마우스 기준 발사 방향 계산
        Bullet b = pool.Get(firePoint.position);                            // 풀에서 총알 하나 꺼내서 위치 세팅
        b.Init(pool, dir, bulletSpeed);                                     // 총알에 풀/방향/속도 초기화
        if (muzzleFxPrefab != null && muzzlePoint != null)                  // 발사 이펙트 생성
            Instantiate(muzzleFxPrefab, muzzlePoint.position, muzzlePoint.rotation);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.ShootClip);    // 발사 사운드 재생
    }

    Vector2 GetAimDir()  // 애니 구조: dir(0:Up,1:Down,2:UpLeft,3:DownLeft) + flipX로 우상/우하                           
    {
        Vector3 mouse = Input.mousePosition;                                // 마우스 스크린 좌표
        Vector3 world = Camera.main.ScreenToWorldPoint(mouse);              // 월드 좌표로 변환
        world.z = 0f;                                                       // 2D이므로 Z는 0으로 고정

        Vector2 dir = (world - firePoint.position);                         // 총구→마우스 방향 벡터
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;                // 너무 가까우면 예외로 기본 방향
        return dir.normalized;                                              // 방향만 필요하니 정규화해서 반환
    }
}