using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }           // 싱글톤

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;                     // BGM 재생용 소스
    [SerializeField] private AudioSource sfxSource;                     // SFX 재생용 소스

    [Header("Clips (Total 5)")]
    [SerializeField] private AudioClip bgmClip;                         // 배경음
    [SerializeField] private AudioClip shootClip;                       // 총소리
    [SerializeField] private AudioClip batDieClip;                      // 박쥐 사망
    [SerializeField] private AudioClip zombieDieClip;                   // 좀비 사망
    [SerializeField] private AudioClip gameOverClip;                    // 게임오버

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.1f;     // BGM 볼륨
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.1f;     // SFX 볼륨

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }          // 중복 생성 방지
        Instance = this;                                                // 싱글톤 등록
        DontDestroyOnLoad(gameObject);                                  // 씬 전환시 유지

        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>(); 
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;                                          // BGM은 반복 재생
        bgmSource.playOnAwake = false;                                  // 시작 시 자동재생 끔
        sfxSource.playOnAwake = false;
    }

    void Update()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;            // 인스펙터 볼륨을 실시간 반영 (0 ~ 1)
    }

    public void PlayBGM()
    {
        if (bgmClip == null) return;                                    // 클립 없으면 종료
        bgmSource.clip = bgmClip;                                       // 재생할 BGM 지정
        bgmSource.volume = bgmVolume;                                   // 볼륨 적용
        if (!bgmSource.isPlaying) bgmSource.Play();                     // 이미 재생 중 아니면 재생
    }

    public void StopBGM() => bgmSource.Stop();                          // BGM 정지

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;                                       // 클립 없으면 종료
        sfxSource.PlayOneShot(clip, sfxVolume * volume);                // SFX를 겹쳐서(원샷) 재생
    }

    // AudioClip 필드를 바깥에서 읽기만 하게
    public AudioClip ShootClip => shootClip;                            // 총쏘기 클립 반환
    public AudioClip BatDieClip => batDieClip;                          // 박쥐 사망 클립 반환
    public AudioClip ZombieDieClip => zombieDieClip;                    // 좀비 사망 클립 반환
    public AudioClip GameOverClip => gameOverClip;                      // 게임오버 클립 반환
}