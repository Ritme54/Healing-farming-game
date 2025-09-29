using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Assets.Scripts;


public class ShopItemCardView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemImage; // 썸네일
    [SerializeField] private TMP_Text nameText; // 이름
    [SerializeField] private Image coinIcon; // 코인 아이콘
    [SerializeField] private TMP_Text priceText; // 가격

    [Header("Optional")]
    [SerializeField] private Button clickButton;      // 카드 클릭용(없으면 루트에 이벤트 트리거로 대체 가능)

    private ItemData _item;
    private Action<ItemData> _onClick;

    // 외부에서 데이터와 클릭 콜백을 주입
    public void SetData(ItemData item, Action<ItemData> onClick = null)
    {
        _item = item;
        _onClick = onClick;

        // 이미지
        if (itemImage != null)
            itemImage.sprite = item?.icon;

        // 이름
        if (nameText != null)
            nameText.text = item != null ? item.name : string.Empty;

        // 가격
        if (priceText != null)
            priceText.text = item != null ? item.price.ToString() : string.Empty;

        // 코인 아이콘은 디자인상 고정 스프라이트일 수 있음(없다면 그대로 두기)
        // coinIcon은 null이어도 무방

        // 버튼 연결
        WireButton();
    }

    private void Awake()
    {
        // 혹시 인스펙터에서 빠졌다면 루트에서 Button 탐색 시도(선택)
        if (clickButton == null)
            clickButton = GetComponent<Button>();

        // 에디터에서 미리 배치된 경우 대비해 초기화
        WireButton();
    }

    private void OnDestroy()
    {
        if (clickButton != null)
            clickButton.onClick.RemoveListener(OnClick);
    }

    private void WireButton()
    {
        if (clickButton == null) return;

        clickButton.onClick.RemoveListener(OnClick);
        clickButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_item == null) return;
        _onClick?.Invoke(_item);
    }
}
