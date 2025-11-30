using UnityEngine;

public class MonsterSensorTrigger : MonoBehaviour
{
    // 몬스터 본체의 EnemyDadChase 스크립트를 참조하기 위한 변수
    private EnemyDadChase dadChaseScript; 

    [Header("Detection Filter")]
    // 💡 Inspector에서 플레이어의 캡슐 콜리더 레이어를 연결합니다!
    public LayerMask PlayerSensorLayer; 

    void Start()
    {
        // 부모 오브젝트에서 EnemyDadChase 스크립트를 찾습니다.
        dadChaseScript = GetComponentInParent<EnemyDadChase>();

        if (dadChaseScript == null)
        {
            Debug.LogError("MonsterSensorTrigger가 부모에서 EnemyDadChase 스크립트를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // LayerMask를 사용하여 상대방(other)이 플레이어의 감지 콜리더(PlayerSensorLayer)인지 필터링
        if (((1 << other.gameObject.layer) & PlayerSensorLayer) != 0)
        {
            Debug.Log($"💥 몬스터의 [작은 원 센서]가 플레이어 센서와 접촉했습니다! (게임 오버 로직 실행)");
            
            // 여기에 게임 오버 로직을 실행합니다. 
            // 예: dadChaseScript.GameOver(); 와 같이 부모 스크립트의 함수를 호출할 수 있습니다.
        }
    }
}