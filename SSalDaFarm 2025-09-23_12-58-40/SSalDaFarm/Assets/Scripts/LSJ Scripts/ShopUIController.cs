using UnityEngine;
using UnityEngine.EventSystems;

public interface IShopOpener
{
    void OpenShop(); // 상점 UI를 연다
    void CloseShop(); // 상점 UI를 닫는다
    bool IsOpen { get; } // 현재 상점 UI 열림 여부 }
}

public class ShopUIController : MonoBehaviour, IShopOpener
{
    [Header("UI References")] public GameObject shopPanel; // 상점 UI 루트 패널(처음에는 비활성화 권장)
    public GameObject defaultFocus; // 상점이 열릴 때 최초로 포커스 줄 UI(예: Buy 버튼)
    public CanvasGroup dim; // 배경 딤(선택: 클릭 차단/알파 제어용)

    [Header("Behavior")]
    public bool lockTimeOnOpen = false; // 열릴 때 게임 일시정지(Time.timeScale=0) 할지 여부

    public bool IsOpen { get; private set; } // 현재 열림 상태
    private float _prevTimeScale = 1f;       // 열기 전 타임스케일 저장

    private void Start()
    {
        // 시작 시 상점은 닫힌 상태로 초기화
        SetOpen(false);
    }

    public void OpenShop()
    {
        // 이미 열려 있으면 무시
        if (IsOpen) return;
        SetOpen(true);
    }

    public void CloseShop()
    {
        // 이미 닫혀 있으면 무시
        if (!IsOpen) return;
        SetOpen(false);
    }

    private void Update()
    {
        // 열려 있을 때 ESC로 닫기 지원
        if (!IsOpen) return;
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
    }

    private void SetOpen(bool open)
    {
        // 내부적으로 열림/닫힘 상태를 전환하고, 관련 UI/시간/포커스를 처리
        IsOpen = open;

        // 패널 표시/숨김
        if (shopPanel != null)
            shopPanel.SetActive(open);

        // 배경 딤 처리(있을 때만)
        if (dim != null)
        {
            dim.alpha = open ? 0.5f : 0f;     // 알파
            dim.blocksRaycasts = open;        // 딤 영역 클릭 차단
            dim.interactable = open;          // 상호작용 여부
        }

        // 타임스케일 잠금(선택)
        if (open)
        {
            if (lockTimeOnOpen)
            {
                _prevTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            // UI 포커스 설정
            if (EventSystem.current != null && defaultFocus != null)
                EventSystem.current.SetSelectedGameObject(defaultFocus);
        }
        else
        {
            if (lockTimeOnOpen)
                Time.timeScale = _prevTimeScale;

            // 포커스 해제
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
