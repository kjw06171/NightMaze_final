using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchLightToggle : MonoBehaviour
{
    private Light2D playerLight;

    [Header("빛 반경 설정")]
    public float startRadius = 6.6f;     // 켰을 때 시작 반경
    public float endRadius = 1.5f;     // 완전히 꺼졌을 때 최소 반경
    public float duration = 60f;      // 빛이 완전히 줄어드는 데 걸리는 시간(초)

    [Header("빛 세기 설정")]
    public float startIntensity = 1f;
    public float endIntensity = 0.5f;

    private float timer = 0f;
    private bool isLightOn = false;     // 현재 횃불이 켜져 있는가?
    private bool isLightDepleted = false; // 완전히 어두워진 상태인가?

    public bool IsLightOn => isLightOn; // public 프로퍼티로 접근 제공

    void Start()
    {
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
            float t = Mathf.Clamp01(timer / duration);

            playerLight.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, t);
            playerLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            // 빛이 완전히 줄었을 때 멈춤
            if (t >= 1f)
            {
                playerLight.pointLightOuterRadius = endRadius;
                playerLight.intensity = endIntensity;
                isLightOn = false;
                isLightDepleted = true; // 더 이상 켜지지 않음
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

        timer = Mathf.Clamp(timer - timeAmount, 0f, duration);
        playerLight.enabled = true;
        isLightOn = true;

        Debug.Log("횃불이 다시 밝아졌습니다!");
    }


}
