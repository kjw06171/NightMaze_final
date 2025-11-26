using UnityEngine;
using Pathfinding; 

public class EnemyDadChase : MonoBehaviour
{
    // A* Pathfinding 컴포넌트
    public Seeker seeker;
    public AIPath aiPath;

    [Header("Target & Light")]
    public Transform Player;
    // 플레이어의 Light Script (TorchLightToggle) 컴포넌트를 연결합니다.
    public TorchLightToggle LightControlScript; 

    [Header("Movement & State")]
    public float ChaseSpeed = 3f;
    public float FleeSpeed = 2f; 
    public float ChaseDistance = 5f; // 플레이어 추격 시작 거리
    public float FleeDistance = 10f; // 도망갈 때 플레이어로부터 멀어지려는 거리

    private bool isChasing = false;
    private bool isFleeing = false;


    void Start()
    {
        // 컴포넌트 자동 참조
        if (aiPath == null) aiPath = GetComponent<AIPath>();
        if (seeker == null) seeker = GetComponent<Seeker>();

        // 초기 상태: 추적 비활성화
        if (aiPath != null) aiPath.enabled = false;
    }

    void Update()
    {
        // 필수 컴포넌트 및 타겟 확인
        if (Player == null || LightControlScript == null || aiPath == null) return;

        // A. 상태 변수 및 거리 확인
        bool isLightActive = LightControlScript.IsLightOn; 
        // 💡 지속적으로 플레이어와의 거리를 계산합니다.
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        
        // --- 1. 상태 전환 및 제어 로직 ---
        
        // 1-1. 빛이 켜진 경우 (Flee)
        if (isLightActive)
        {
            if (!isFleeing)
            {
                StartFleeing();
            }
        }
        // 1-2. 빛이 꺼진 경우 (Stop Flee / Chase / Idle)
        else 
        {
            // (A) 도망 중이었다면 즉시 중지하여 isFleeing을 false로 리셋
            if (isFleeing)
            {
                StopFleeing(); 
            }

            // (B) 횃불이 꺼져있고, 추격 범위 안에 들어왔을 경우 추격 시작
            if (!isFleeing && distanceToPlayer < ChaseDistance)
            {
                if (!isChasing) // 이미 추격 중이 아니라면
                {
                    StartChasing();
                }
            }
            // (C) 추격 중이었는데, 플레이어가 범위 밖으로 나갔을 경우 추격 중지
            else if (isChasing && distanceToPlayer >= ChaseDistance)
            {
                StopChasing();
            }
        }
        
        // --- 2. 이동 처리 ---
        
        if (isFleeing)
        {
            // 몬스터 위치에서 플레이어 반대 방향으로 멀어지는 가상 목표 지점 계산
            Vector3 directionToPlayer = Player.position - transform.position;
            Vector3 fleeDirection = -directionToPlayer.normalized;
            Vector3 targetPosition = transform.position + fleeDirection * FleeDistance;

            // 💡 끼임 현상 방지: 유효한 경로 노드를 찾습니다.
            NNConstraint constraint = NNConstraint.None;
            NNInfo nearestNodeInfo = AstarPath.active.GetNearest(targetPosition, constraint);
            Vector3 nearestValidTarget = nearestNodeInfo.position;

            // AIPath의 목표 지점을 가장 가까운 유효 노드로 설정
            aiPath.destination = nearestValidTarget;
            
            if (!aiPath.enabled) aiPath.enabled = true;
            aiPath.maxSpeed = FleeSpeed;
        }

        else if (isChasing)
        {
            // 추격 중일 때는 플레이어를 목표로 추격 유지
            if (!aiPath.enabled) aiPath.enabled = true;
            aiPath.target = Player;
            aiPath.maxSpeed = ChaseSpeed;
        }
    }

    // ⚠️ OnTriggerEnter2D와 OnTriggerExit2D 함수는 제거하거나 주석 처리해야 합니다.
    // 이제 상태 관리가 Update()의 거리 기반으로 이루어집니다.
    /*
    private void OnTriggerEnter2D(Collider2D other) { }
    private void OnTriggerExit2D(Collider2D other) { }
    */
    
    // --------------------------------------------------------
    // 상태 변경 도우미 함수 (이전과 동일)
    // --------------------------------------------------------

    void StartChasing()
    {
        if (isFleeing) StopFleeing();
        
        isChasing = true;
        
        aiPath.target = Player; 
        aiPath.enabled = true;
        aiPath.maxSpeed = ChaseSpeed;
        Debug.Log("추격 시작!");
    }

    void StopChasing()
    {
        isChasing = false;
        if (!isFleeing)
        {
            aiPath.enabled = false;
            seeker.CancelCurrentPathRequest();
            Debug.Log("추격 중지!");
        }
    }
    
    void StartFleeing()
    {
        if (isChasing) StopChasing(); 
        
        isFleeing = true;
        aiPath.target = null;
        aiPath.enabled = true;
        aiPath.maxSpeed = FleeSpeed;
        
        Debug.Log("불 감지! 도망 시작!");
    }

    void StopFleeing()
    {
        isFleeing = false;
        aiPath.enabled = false;
        seeker.CancelCurrentPathRequest();
        Debug.Log("도망 중지!");
    }
}