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
        public string name;        // ������ �̸�
        public string description; // ������ ����
        public int price;         // ����(���� ����)
        public Sprite icon;       // ������
    }
    [Header("UI References")] public RectTransform content; //Viewport ����
    public GameObject itemCardPrefab; //ItemCard ������
    [Header("Layout Settings")]
    public float itemHeight = 140f;        // ī�� ����
    public float spacing = 15f;            // ī�� ���� ����
    public float topPadding = 0f;          // ��� �е�
    public float bottomPadding = 0f;       // �ϴ� �е�
    public float leftRightMargin = 16f;    // ī�� �¿� ����(������ ���ο��� �̹� ó���Ǿ� ������ ����)

    [Header("Sample Data")]
    public List<ItemData> sampleItems = new List<ItemData>();

    // �ʱ�ȭ ������
    private void Start()
    {
        // ���� �����Ͱ� ��� ������ �ӽ� ������ ����(�ǻ�� �� ���� ����)
        if (sampleItems.Count == 0)
        {
            sampleItems = CreateDummyItems(10);
        }

        Populate(sampleItems);
    }

    public void Populate(List<ItemData> items)
    {
        // ���� �ڽ� ����
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // Content�� ��Ŀ/�ǹ� ��� ���� Ȯ��(�ܰ� 3���� ���� �Ϸ� ����)
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);

        // ī�� ������ ��ġ
        for (int i = 0; i < items.Count; i++)
        {
            var data = items[i];

            // ������ ����
            var go = Instantiate(itemCardPrefab, content);
            var rt = go.GetComponent<RectTransform>();

            // ī�� ��ü ��Ŀ/�ǹ�(��� ���� + �¿� ��Ʈ��ġ)
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);

            // �¿� �����°� ����(�����տ��� �̹� 16/16/140�̶�� ���� ����)
            rt.offsetMin = new Vector2(leftRightMargin, rt.offsetMin.y); // Left
            rt.offsetMax = new Vector2(-leftRightMargin, rt.offsetMax.y); // Right
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);

            // Y ��ġ ���(��� ����)
            float y = -(topPadding + i * (itemHeight + spacing));
            rt.anchoredPosition = new Vector2(0f, y);

            // UI ���ε�
            BindItemCard(go, data);
        }

        // Content ���� ���
        float contentHeight = 0f;
        if (items.Count > 0)
        {
            contentHeight = topPadding + bottomPadding
                            + items.Count * itemHeight
                            + (items.Count - 1) * spacing;
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    // ī�� ���� ������Ʈ ���ε�
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
            // Text �Ǵ� TextMeshPro ó��
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
            string priceStr = FormatPrice(data.price); // ��1,000 ���Ρ� ���� ǥ��
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
                // ���⼭ ���� ���� ����
                btn.onClick.AddListener(() =>
                {
                    OnClickBuy(data);
                });
            }
        }
    }

    // ���� ǥ��: 123456 �� ��123,456 ���Ρ�
    private string FormatPrice(int price)
    {
        return string.Format("{0:N0} ����", price);
    }

    // ���� ��ư �ݹ�(����)
    private void OnClickBuy(ItemData data)
    {
        Debug.Log($"���� �õ�: {data.name} / {data.price} ����");
        // ���� ���� �帧(Ȯ�� �˾�/�κ��丮/��ȭ ���� ��)�� ���⿡ ����
    }

    // ����� ���� ������ ����
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
                icon = null // �������� �ӽ� null, �����Ϳ��� Sprite �Ҵ� ����
            });
        }
        return list;
    }
}
