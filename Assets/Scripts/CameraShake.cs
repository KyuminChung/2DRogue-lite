using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalLocalPos;                                     // 흔들림이 끝나면 돌아갈 원래 로컬 위치
    private Coroutine shakeRoutine;                                       // 현재 실행 중인 코루틴 참조(중복 방지)

    void Awake()
    {
        originalLocalPos = transform.localPosition;                       // 시작 시 카메라 기본 위치 저장
    }

    public void Shake(float duration = 0.12f, float strength = 0.12f)   
    {
        if (shakeRoutine != null)                                         // 이미 흔들리고 있으면 
            StopCoroutine(shakeRoutine);                                  // 기존 흔들림 중지
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));  // 새 흔들림 시작
    }

    private IEnumerator ShakeRoutine(float duration, float strength)        
    {
        float t = 0f;                                                     // 경과 시간 누적 변수

        while (t < duration)                                              // 지정 시간 동안
        {
            t += Time.unscaledDeltaTime;                                  // 경과 시간 누적

            float x = Random.Range(-1f, 1f) * strength;                   // X축 랜덤 흔들림
            float y = Random.Range(-1f, 1f) * strength;                   // Y 

            transform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
            yield return null;                                            // 다음 프레임까지 대기
        }

        transform.localPosition = originalLocalPos;                       // 원래 위치
        shakeRoutine = null;                      
    }
}