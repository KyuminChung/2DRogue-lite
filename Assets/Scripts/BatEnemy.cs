using UnityEngine;

public class BatEnemy : abstractEnemy
{
    protected override void Awake()
    {
        maxHp = 1;                         // 박쥐 체력 1
        moveSpeed = 4.5f;                  // 박쥐 이동 속도 설정
        base.Awake();                      // 부모 Awake 실행
    }
    protected override void Die()          // 박쥐 사망
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.BatDieClip,5f);
        base.Die();
    }
}