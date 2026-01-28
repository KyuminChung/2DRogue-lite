using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;            // 회복량 1

    private void OnTriggerEnter2D(Collider2D other)         
    {
        var ph = other.GetComponentInParent<Player>();      
        if (ph == null) return;                             // 플레이어가 아니면 무시

        ph.Heal(healAmount);                                // 플레이어 체력 회복
        Destroy(gameObject);                                // 포션 Destroy
    }
}