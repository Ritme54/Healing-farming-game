using UnityEngine;
using UnityEngine.UI; // UI(사용자 인터페이스)
using TMPro; // TextMeshPro(텍스트메시프로)

public class ItemModal : MonoBehaviour
{
    [Header("Refs (참조)")]

    public GameObject overlay; // ModalOverlay(모달 오버레이, 전체 배경)
    public RectTransform window; // ModalWindow(모달 윈도우)
    public Image iconImage; // IconImage(아이콘 이미지)
    public TMP_Text nameText; // NameText(이름 텍스트)
    public TMP_Text priceText; // PriceText(가격 텍스트)
    public TMP_Text descText; // DescText(설명 텍스트)
    public Button closeButton; // CloseButton(닫기 버튼, X)
    public Button cancelButton; // CancelButton(취소/닫기 버튼)
    public Button confirmButton; // ConfirmButton(확인/구매 버튼)
                               // 확인(Confirm) 시 외부로 전달할 콜백과 현재 아이템 데이터
    private System.Action<ItemData> onConfirm;  // Confirm 콜백(확인 시 호출)
    private ItemData current;                    // 현재 표시 중인 아이템 데이터

    void Awake()
    {
        // 시작 시 닫힘 상태로
        HideImmediate();

        // 버튼 이벤트 연결
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (cancelButton != null) cancelButton.onClick.AddListener(Close);

        // Confirm(확인) 버튼은 별도 메서드 사용
        if (confirmButton != null) confirmButton.onClick.AddListener(ClickConfirm);
    }

    // 모달 표시: 데이터 바인딩 + 콜백 저장
    public void Show(ItemData data, System.Action<ItemData> onConfirmHandler = null)
    {
        if (data == null)
        {
            Debug.LogWarning("ItemModal.Show 호출 시 data가 null입니다.");
            return;
        }

        current = data;
        onConfirm = onConfirmHandler;

        // 데이터 바인딩
        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.name;
        if (priceText != null) priceText.text = $"{data.price:N0} 코인";
        if (descText != null) descText.text = "임시 설명입니다. 추후 실제 설명으로 교체하세요.";

        // 표시
        if (overlay != null) overlay.SetActive(true);

        // 간단한 팝인 연출(선택)
        if (window != null)
        {
            window.localScale = Vector3.one * 0.96f;
            // 트윈(애니메이션) 라이브러리를 사용하신다면 여기서 부드럽게 1.0으로 보간하시면 됩니다.
        }

        // 접근성 보완: 확인 버튼에 포커스 주기(선택)
        if (confirmButton != null) confirmButton.Select();

        Debug.Log("오버레이 활성화");
    }

    // 배경 클릭을 통해 닫으려면, ModalOverlay(오버레이)의 Button(OnClick)에 이 Close를 연결
    public void Close()
    {
        if (overlay != null) overlay.SetActive(false);

        // 상태 초기화
        current = null;
        onConfirm = null;
        Debug.Log("오버레이 비 활성화");
    }

    // 즉시 숨기기(초기화 시 사용)
    public void HideImmediate()
    {
        if (overlay != null) overlay.SetActive(false);
    }

    // 확인 버튼 클릭 시: 콜백 호출 후 닫기
    public void ClickConfirm()
    {
        if (current != null)
        {
            onConfirm?.Invoke(current);
        }
        Close();
    }
}