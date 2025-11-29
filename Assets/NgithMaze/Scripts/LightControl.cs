using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchLightToggle : MonoBehaviour
{
    private Light2D playerLight;

    [Header("빛 반경 설정")]
    public float startRadius = 6.6f;     // 켰을 때 시작 반경
    public float endRadius = 1.5f;       // 완전히 꺼졌을 때 최소 반경
    public float duration = 60f;        // 빛이 완전히 줄어드는 데 걸리는 시간(초)

    [Header("빛 세기 설정")]
    public float startIntensity = 1f;
    public float endIntensity = 0.5f;

    private float timer = 0f;
    private bool isLightOn = false;      // 현재 횃불이 켜져 있는가?
    private bool isLightDepleted = false; // 완전히 어두워진 상태인가?

    public bool IsLightOn => isLightOn; // public 프로퍼티로 접근 제공
    
    // 💡 UI Slider에 연결할 빛의 남은 비율 (1.0f ~ 0.0f)
    public float LightRatio
    {
        // 1.0f - (소모된 비율) = 남은 비율을 계산합니다.
        get { return 1.0f - Mathf.Clamp01(timer / duration); }
    }

    void Start()
    {
        // 횃불 오브젝트에 붙어있는 Light2D 컴포넌트를 참조
        playerLight = GetComponent<Light2D>();
        playerLight.enabled = false; // 처음엔 꺼진 상태
    }

    void Update()
    {
        // 🔘 2번 키로 횃불 On/Off (단, 완전히 소진되면 다시 켤 수 없음)
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isLightDepleted)
        {
            isLightOn = !isLightOn;
            playerLight.enabled = isLightOn;

            if (!isLightOn)
                Debug.Log("횃불 OFF (타이머 정지)");
            else
                Debug.Log("횃불 ON (타이머 재개)");
        }

        // 🔥 불이 켜져 있을 때만 시간 흐름
        if (isLightOn && !isLightDepleted)
        {
            timer += Time.deltaTime;
            // t는 소모된 비율 (0.0 -> 1.0)
            float t = Mathf.Clamp01(timer / duration); 

            // 반경과 세기를 시간에 따라 Lerp(선형 보간)하여 줄어들게 함
            playerLight.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, t);
            playerLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            // 빛이 완전히 줄었을 때 멈춤 (소모된 비율 t가 1.0f에 도달했을 때)
            if (t >= 1f)
            {
                playerLight.pointLightOuterRadius = endRadius;
                playerLight.intensity = endIntensity;
                isLightOn = false;
                isLightDepleted = true; // 더 이상 켤 수 없음
                playerLight.enabled = false;

                Debug.Log("횃불이 완전히 꺼졌습니다.");
            }
        }
    }

    // 🔋 아이템으로 빛 회복 (예: 횃불 줍기)
    public void RestoreLight(float timeAmount)
    {
        // 완전히 꺼진 상태에서도 아이템으로 다시 살릴 수 있음
        isLightDepleted = false;

        // timer를 감소시켜 빛 잔량을 복원합니다.
        // (timer 0f가 만땅, duration이 바닥임)
        timer = Mathf.Clamp(timer - timeAmount, 0f, duration);
        
        // 빛이 즉시 켜지도록 설정 (옵션)
        playerLight.enabled = true;
        isLightOn = true;

        // 회복된 만큼 반경과 세기를 즉시 업데이트
        float t = Mathf.Clamp01(timer / duration);
        playerLight.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, t);
        playerLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);

        Debug.Log("횃불이 다시 밝아졌습니다!");
    }
}