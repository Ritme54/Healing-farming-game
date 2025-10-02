using Assets.Scripts;
using System.Collections.Generic;
using TMPro; // TextMeshPro ��� �� ����
using UnityEngine;
using UnityEngine.UI;

public class StoreListBinder : MonoBehaviour
{
    [Header("References")] public Transform content; // Viewport ���� Content
    public GameObject itemCardPrefab; // ItemCard ������

    [Header("Sample Data")]
    public List<ItemData> items = new List<ItemData>();

    public ItemListAsset storeItemList;

    private void Start()
    {
        if (items.Count == 0 && storeItemList != null) items = ListToRuntime(storeItemList);
        if (items.Count == 0) items = CreateDummyItems(10);
        BindAll(items);
    }

    public void BindAll(List<ItemData> dataList)
    {
        // ���� ī�� ����
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // ī�� ���� + �� ����
        foreach (var data in dataList)
        {
            var go = Instantiate(itemCardPrefab, content);
            BindOne(go, data);
        }
        // ���� ����/���̴� Vertical Layout Group + Content Size Fitter�� �ڵ� ó��
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
        return string.Format("{0:N0} Coins", price);
    }

    private void OnClickBuy(ItemData data)
    {
        Debug.Log($"���� �õ�: {data.name} / {data.price} Coins");
        // TODO: Ȯ�� �˾�/��ȭ ���� �� ���� ���� ���� ����
    }

    private List<ItemData> CreateDummyItems(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"���� {i + 1}",
                description = "ü���� ȸ���ϴ� �⺻ �����Դϴ�.",
                price = 1000 + i * 250,
                icon = null
            });
        }
        return list;
    }

    private List<ItemData> ListToRuntime(ItemListAsset listAsset)
    {
        var list = new List<ItemData>();
        if (listAsset == null || listAsset.entries == null) return list;
        foreach (var e in listAsset.entries)
        {
            if (e == null || e.item == null) continue;
            var r = e.item.ToRuntime();
            if (e.overridePrice >= 0) r.price = e.overridePrice; list.Add(r);
        }
        return list;
    }

}

