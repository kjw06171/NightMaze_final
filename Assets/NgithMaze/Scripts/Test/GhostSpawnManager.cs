using UnityEngine;
using System.Collections; // Coroutine을 위해 필요

public class GhostSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ghostPrefab; // 스폰할 유령 프리팹을 인스펙터에 할당해주세요.
    public float minSpawnInterval = 5f; // 최소 스폰 시간 간격 (초)
    public float maxSpawnInterval = 15f; // 최대 스폰 시간 간격 (초)
    public int minSpawnCount = 1; // 한 번에 스폰될 최소 유령 개수
    public int maxSpawnCount = 3; // 한 번에 스폰될 최대 유령 개수
    public float spawnPadding = 1f; // 카메라 뷰포트 밖으로 얼마나 더 나갈지 (월드 단위)

    [Header("References")]
    public Transform playerTransform; // 플레이어의 Transform을 인스펙터에 할당하거나 Find 등으로 찾으세요.
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // 메인 카메라를 찾아 할당

        if (playerTransform == null)
        {
            // 플레이어를 태그로 찾거나, 게임 시작 시 플레이어 오브젝트를 직접 할당하는 등의 방법 사용
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); 
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player Transform이 할당되지 않았습니다! 플레이어 오브젝트를 찾을 수 없습니다.");
                enabled = false; // 스크립트 비활성화
                return;
            }
        }

        if (ghostPrefab == null)
        {
            Debug.LogError("Ghost Prefab이 할당되지 않았습니다! 스폰할 유령 프리팹을 인스펙터에 할당해주세요.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnGhostsCoroutine());
    }

    IEnumerator SpawnGhostsCoroutine()
    {
        while (true)
        {
            float spawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnDelay);

            int currentSpawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
            for (int i = 0; i < currentSpawnCount; i++)
            {
                SpawnSingleGhost();
            }
        }
    }

    void SpawnSingleGhost()
    {
        if (playerTransform == null || mainCamera == null) return;

        // 카메라 밖 랜덤 위치 계산
        Vector2 spawnPosition = GetRandomSpawnPositionOutsideCamera();

        GameObject newGhost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        GhostAI ghostAI = newGhost.GetComponent<GhostAI>(); // 유령 프리팹에 GhostAI 스크립트가 있다고 가정
        if (ghostAI != null)
        {
            ghostAI.SetTarget(playerTransform); // 유령 AI에 플레이어 타겟 설정
        }
    }

    // 카메라 밖 랜덤 위치를 계산하는 함수
    Vector2 GetRandomSpawnPositionOutsideCamera()
    {
        // 뷰포트 좌표 (0,0)은 화면 왼쪽 아래, (1,1)은 화면 오른쪽 위
        // 카메라 바깥쪽을 나타내기 위해 -spawnPadding 또는 1 + spawnPadding 사용

        float xViewport = Random.Range(0f, 1f); // X축은 화면 내에서 랜덤하게 선택
        float yViewport = Random.Range(0f, 1f); // Y축은 화면 내에서 랜덤하게 선택

        // 스폰할 가장자리를 랜덤하게 선택 (상, 하, 좌, 우)
        int edge = Random.Range(0, 4); // 0:Left, 1:Right, 2:Bottom, 3:Top

        switch (edge)
        {
            case 0: // Left
                xViewport = -spawnPadding / mainCamera.orthographicSize; // 뷰포트 비율로 패딩 적용
                break;
            case 1: // Right
                xViewport = 1f + spawnPadding / mainCamera.orthographicSize;
                break;
            case 2: // Bottom
                yViewport = -spawnPadding / mainCamera.orthographicSize;
                break;
            case 3: // Top
                yViewport = 1f + spawnPadding / mainCamera.orthographicSize;
                break;
        }
        
        // 뷰포트 좌표를 월드 좌표로 변환
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(new Vector3(xViewport, yViewport, mainCamera.nearClipPlane));
        worldPos.z = 0; // 2D 게임이므로 z축은 0으로 고정
        return worldPos;
    }
}