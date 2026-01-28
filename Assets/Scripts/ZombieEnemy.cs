using UnityEngine;

public class ZombieEnemy : abstractEnemy
{
    protected override void Awake()
    {
        maxHp = 2;                         // 좀비 체력 2
        moveSpeed = 1.5f;                  // 좀비 이동 속도 설정
        base.Awake();                      // 부모 Awake 실행
    }
    protected override void Die()          // 좀비 사망
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.ZombieDieClip,5f);
        base.Die();
    }
}
