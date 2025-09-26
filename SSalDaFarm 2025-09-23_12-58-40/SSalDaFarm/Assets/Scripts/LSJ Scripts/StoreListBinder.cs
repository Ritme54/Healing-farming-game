using System.Collections.Generic;
using TMPro; // TextMeshPro 사용 시 유지
using UnityEngine;
using UnityEngine.UI;

public class StoreListBinder : MonoBehaviour
{
    [Header("References")] public Transform content; // Viewport 하위 Content
    public GameObject itemCardPrefab; // ItemCard 프리팹

    [Header("Sample Data")]
    public List<ItemData> items = new List<ItemData>();

    private void Start()
    {
        if (items.Count == 0)
        {
            items = CreateDummyItems(10);
        }
        BindAll(items);
    }

    public void BindAll(List<ItemData> dataList)
    {
        // 기존 카드 제거
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // 카드 생성 + 값 대입
        foreach (var data in dataList)
        {
            var go = Instantiate(itemCardPrefab, content);
            BindOne(go, data);
        }
        // 세로 정렬/높이는 Vertical Layout Group + Content Size Fitter가 자동 처리
    }

    private void BindOne(GameObject card, ItemData data)
    {
        // Icon
        var iconTr = card.transform.Find("Icon");
        if (iconTr != null)
        {
            var img = iconTr.GetComponent<Image>();
            if (img != null) img.sprite = data.icon;
        }

        // NameText
        SetText(card.transform, "InfoGroup/NameText", data.name);

        // DescText
        SetText(card.transform, "InfoGroup/DescText", data.description);

        // PriceText
        SetText(card.transform, "ActionGroup/PriceText", FormatPrice(data.price));

        // BuyButton
        var btnTr = card.transform.Find("ActionGroup/BuyButton");
        if (btnTr != null)
        {
            var btn = btnTr.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    OnClickBuy(data);
                });
            }
        }
    }

    private void SetText(Transform root, string path, string value)
    {
        var tr = root.Find(path);
        if (tr == null) return;

        var tmp = tr.GetComponent<TMP_Text>();
        if (tmp != null) { tmp.text = value; return; }

        var uText = tr.GetComponent<Text>();
        if (uText != null) uText.text = value;
    }

    private string FormatPrice(int price)
    {
        return string.Format("{0:N0} 코인", price);
    }

    private void OnClickBuy(ItemData data)
    {
        Debug.Log($"구매 시도: {data.name} / {data.price} 코인");
        // TODO: 확인 팝업/재화 차감 등 실제 구매 로직 연결
    }

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
                icon = null
            });
        }
        return list;
    }
}

[System.Serializable] public class ItemData { public string name; public string description; public int price; public Sprite icon; }