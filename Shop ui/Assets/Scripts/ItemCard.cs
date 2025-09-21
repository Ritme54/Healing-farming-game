using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [Serializable] public class ItemData
    {
        public string name;        // 아이템 이름
        public string description; // 아이템 설명
        public int price;         // 가격(코인 단위)
        public Sprite icon;       // 아이콘
    }
    [Header("UI References")] public RectTransform content; //Viewport 하위
    public GameObject itemCardPrefab; //ItemCard 프리팹
    [Header("Layout Settings")]
    public float itemHeight = 140f;        // 카드 높이
    public float spacing = 15f;            // 카드 사이 간격
    public float topPadding = 0f;          // 상단 패딩
    public float bottomPadding = 0f;       // 하단 패딩
    public float leftRightMargin = 16f;    // 카드 좌우 마진(프리팹 내부에서 이미 처리되어 있으면 참고만)

    [Header("Sample Data")]
    public List<ItemData> sampleItems = new List<ItemData>();

    // 초기화 진입점
    private void Start()
    {
        // 데모 데이터가 비어 있으면 임시 데이터 생성(실사용 시 제거 가능)
        if (sampleItems.Count == 0)
        {
            sampleItems = CreateDummyItems(10);
        }

        Populate(sampleItems);
    }

    public void Populate(List<ItemData> items)
    {
        // 기존 자식 정리
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // Content의 앵커/피벗 상단 기준 확인(단계 3에서 설정 완료 가정)
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);

        // 카드 생성과 배치
        for (int i = 0; i < items.Count; i++)
        {
            var data = items[i];

            // 프리팹 생성
            var go = Instantiate(itemCardPrefab, content);
            var rt = go.GetComponent<RectTransform>();

            // 카드 자체 앵커/피벗(상단 기준 + 좌우 스트레치)
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);

            // 좌우 오프셋과 높이(프리팹에서 이미 16/16/140이라면 생략 가능)
            rt.offsetMin = new Vector2(leftRightMargin, rt.offsetMin.y); // Left
            rt.offsetMax = new Vector2(-leftRightMargin, rt.offsetMax.y); // Right
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);

            // Y 위치 계산(상단 기준)
            float y = -(topPadding + i * (itemHeight + spacing));
            rt.anchoredPosition = new Vector2(0f, y);

            // UI 바인딩
            BindItemCard(go, data);
        }

        // Content 높이 계산
        float contentHeight = 0f;
        if (items.Count > 0)
        {
            contentHeight = topPadding + bottomPadding
                            + items.Count * itemHeight
                            + (items.Count - 1) * spacing;
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    // 카드 내부 컴포넌트 바인딩
    private void BindItemCard(GameObject card, ItemData data)
    {
        // Icon
        var iconTrans = card.transform.Find("Icon");
        if (iconTrans != null)
        {
            var img = iconTrans.GetComponent<Image>();
            if (img != null) img.sprite = data.icon;
        }

        // NameText
        var nameT = card.transform.Find("InfoGroup/NameText");
        if (nameT != null)
        {
            // Text 또는 TextMeshPro 처리
            var tmp = nameT.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = data.name;
            else
            {
                var uText = nameT.GetComponent<Text>();
                if (uText != null) uText.text = data.name;
            }
        }

        // DescText
        var descT = card.transform.Find("InfoGroup/DescText");
        if (descT != null)
        {
            var tmp = descT.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = data.description;
            else
            {
                var uText = descT.GetComponent<Text>();
                if (uText != null) uText.text = data.description;
            }
        }

        // PriceText
        var priceT = card.transform.Find("ActionGroup/PriceText");
        if (priceT != null)
        {
            string priceStr = FormatPrice(data.price); // “1,000 코인” 같은 표기
            var tmp = priceT.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = priceStr;
            else
            {
                var uText = priceT.GetComponent<Text>();
                if (uText != null) uText.text = priceStr;
            }
        }

        // BuyButton
        var btnTrans = card.transform.Find("ActionGroup/BuyButton");
        if (btnTrans != null)
        {
            var btn = btnTrans.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                // 여기서 구매 로직 연결
                btn.onClick.AddListener(() =>
                {
                    OnClickBuy(data);
                });
            }
        }
    }

    // 가격 표기: 123456 → “123,456 코인”
    private string FormatPrice(int price)
    {
        return string.Format("{0:N0} 코인", price);
    }

    // 구매 버튼 콜백(샘플)
    private void OnClickBuy(ItemData data)
    {
        Debug.Log($"구매 시도: {data.name} / {data.price} 코인");
        // 실제 구매 흐름(확인 팝업/인벤토리/재화 차감 등)을 여기에 구현
    }

    // 데모용 더미 데이터 생성
    private List<ItemData> CreateDummyItems(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"포션 {i + 1}",
                description = "체력을 회복하는 기본 포션입니다.",
                price = 1000 + i * 250,
                icon = null // 아이콘은 임시 null, 에디터에서 Sprite 할당 권장
            });
        }
        return list;
    }
}
