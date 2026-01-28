using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Bullet bulletPrefab;           // 총알 프리팹
    [SerializeField] private int prewarmCount = 40;         // 미리 만들어둘 개수

    private readonly Queue<Bullet> pool = new();            // 비활성 총알 담아둘 큐

    void Awake()
    {
        Prewarm();                                          // 총알 미리 생성해 풀 채움
    }

    private void Prewarm()
    {
        for (int i = 0; i < prewarmCount; i++)              // 지정한 개수만큼 반복
        {
            Bullet b = CreateBullet();                      // 총알 하나 생성
            pool.Enqueue(b);                                // 큐에 넣어 보관
        }
    }

    private Bullet CreateBullet()
    {
        Bullet b = Instantiate(bulletPrefab, transform);    
        b.gameObject.SetActive(false);                      // 생성 직후 비활성화
        return b;
    }
    public Bullet Get(Vector3 position)
    {
        Bullet b = pool.Count > 0                           // 풀에 남은 총알이 있으면
            ? pool.Dequeue()                                // 하나 꺼내서 사용
            : CreateBullet();                               // 없으면 새로 생성

        b.transform.position = position;                    // 위치
        b.transform.rotation = Quaternion.identity;         // 회전 초기화
        b.gameObject.SetActive(true);                       // 활성화
        return b;
    }

    public void Return(Bullet b)
    {
        if (b == null) return;
        b.gameObject.SetActive(false);                      // 비활성화
        pool.Enqueue(b);                                    // 큐에 넣어 대기
    }
}