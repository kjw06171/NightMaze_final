using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 3f;
    private Transform targetPlayer; // 스폰 매니저에서 할당할 플레이어 Transform

    // 스폰 매니저가 호출할 메서드
    public void SetTarget(Transform player)
    {
        targetPlayer = player;
    }

    void Update()
    {
        if (targetPlayer != null)
        {
            // 플레이어를 향해 이동
            Vector2 direction = (targetPlayer.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, chaseSpeed * Time.deltaTime);

            // 옵션: 스프라이트 뒤집기 (플레이어 방향)
            if (targetPlayer.position.x < transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    // 유령이 벽을 통과한다면 물리 충돌 로직은 필요 없을 수 있습니다.
    // 필요에 따라 OnTriggerEnter2D 등을 사용하여 플레이어와 닿았을 때의 효과를 구현합니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("유령이 플레이어와 접촉했습니다!");
            // 플레이어에게 피해를 주거나 유령을 파괴하는 로직
            Destroy(gameObject);
        }
    }
}