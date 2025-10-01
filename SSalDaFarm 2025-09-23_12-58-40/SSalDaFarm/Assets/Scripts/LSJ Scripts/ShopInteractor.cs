using UnityEngine;
using static IShopOpener;

public class ShopInteractor : MonoBehaviour
{
    [Header("Shop Controller Lookup")]
    public ShopUIController shop; // 씬에 하나라면 비워두고 자동 탐색 가능

    [Header("Open Triggers")]
    public bool allowClickToOpen = true;   // 마우스 클릭으로 열기
    public bool allowKeyToOpen = true;     // 키 입력(E/Enter)로 열기
    public KeyCode key1 = KeyCode.E;       // 상호작용 키 1
    public KeyCode key2 = KeyCode.Return;  // 상호작용 키 2(Enter)

    [Header("Standalone Test Range (No Player Needed)")]
    public bool useFakeRange = false;      // 플레이어 없이 범위 제한을 흉내낼지
    public float fakeRange = 2.0f;         // 허용 거리(카메라 중심 또는 기준점과의 거리)
    public Transform rangeCenter;          // 기준점(비우면 이 오브젝트)

    private Camera _cam;

    private void Awake()
    {
        // 씬에서 ShopUIController를 자동으로 찾아 참조(명시 연결도 가능)
        if (shop == null) shop = FindObjectOfType<ShopUIController>();

        _cam = Camera.main;
        if (rangeCenter == null) rangeCenter = transform;
    }

    private void Update()
    {
        // 키 입력으로 열기/닫기 토글
        if (shop == null) return;

        if (allowKeyToOpen && (Input.GetKeyDown(key1) || Input.GetKeyDown(key2)))
        {
            // 가짜 범위 사용 시 범위 안에서만 허용
            if (!useFakeRange || InFakeRange())
                ToggleOpen();
        }
    }

    private void OnMouseDown()
    {
        // 오브젝트에 Collider2D/Collider가 있고 카메라가 해당 레이어를 보게 세팅되어 있어야 호출됨
        if (!allowClickToOpen || shop == null) return;

        if (!useFakeRange || InFakeRange())
            ToggleOpen();
    }

    private bool InFakeRange()
    {
        // 플레이어가 없는 환경에서 “범위 체크”를 흉내내는 함수
        // 기준: 카메라 중심과 rangeCenter 사이의 거리로 판정(임시)
        if (_cam == null) return true;
        var camPos = _cam.transform.position;
        camPos.z = rangeCenter.position.z; // 2D 평면 비교를 위해 z를 맞춤
        return Vector2.Distance(camPos, rangeCenter.position) <= fakeRange;
    }

    private void ToggleOpen()
    {
        // 열린 상태면 닫고, 닫혀 있으면 연다
        if (shop.IsOpen) shop.CloseShop();
        else shop.OpenShop();
    }
}