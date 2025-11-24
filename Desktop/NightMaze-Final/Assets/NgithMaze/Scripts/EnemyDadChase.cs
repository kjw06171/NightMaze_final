using UnityEngine;

public class EnemyDadChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;   // 추적 속도
    [SerializeField] private float chaseDistance = 5f;  // 추적 범위
    
    // 플레이어의 LightControl 스크립트를 인스펙터에서 할당
    [Header("Component References")]
    [SerializeField] private TorchLightToggle lightControlScript; 
    
    private Transform player;                              // 플레이어 Transform
    private bool isChasing = false;                        // 추적 중 여부
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;                                // Rigidbody2D 참조
    // private Collider2D chaseCollider;  // ❌ 이 변수는 더 이상 사용하지 않아도 됩니다.

    void Start()
    {
        // 몬스터 자신에게서 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); 
        // chaseCollider는 OnTrigger/OnCollision 함수가 자동으로 처리하므로 Start에서 할당하지 않아도 됩니다.
    }
    
    void Update()
    {
        // ... (Update 함수는 변경 없음)
        if (player != null && lightControlScript != null) 
        {
            if (isChasing && !lightControlScript.IsLightOn)
            {
                ChasePlayer();
            }
            else if (lightControlScript.IsLightOn)
            {
                FleePlayer();
            }
        }
        else if (player == null)
        {
            rb.velocity = Vector2.zero;
            // Debug.Log("Player가 null입니다! (범위 밖이거나 아직 발견되지 않음)");
        }
        else if (lightControlScript == null)
        {
            Debug.LogError("LightControlScript가 인스펙터에 할당되지 않았습니다! 반드시 할당해주세요.");
        }
    }

    void ChasePlayer()
    {
        // ... (ChasePlayer 함수는 변경 없음)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= chaseDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * chaseSpeed;
            // ... (스프라이트 반전 로직)
            if (player.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void FleePlayer()
    {
        // ... (FleePlayer 함수는 변경 없음)
        Vector2 direction = (transform.position - player.position).normalized;
        rb.velocity = direction * chaseSpeed;

        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    // 🎯 [1] Circle Collider (Is Trigger) : 플레이어가 추적 범위에 들어오면 추적 시작
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            isChasing = true;
            Debug.Log("플레이어 발견, 추적 시작!");
        }
    }
    
    // 🎯 [2] Circle Collider (Is Trigger) : 플레이어가 추적 범위 밖으로 나가면 추적 종료
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            player = null;
            Debug.Log("플레이어가 멀어져 추적 종료.");
        }
    }
    
    // 💥 [3] Capsule Collider (NOT Trigger) : 플레이어와 물리적으로 닿았을 때 (원하는 로그 출력)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 이 로그는 캡슐 콜라이더가 플레이어와 '딱' 붙었을 때(물리적 충돌) 출력됩니다.
            Debug.Log($"💥 몬스터가 플레이어({collision.gameObject.name})와 물리적으로 충돌했습니다!");
        }
    }
}