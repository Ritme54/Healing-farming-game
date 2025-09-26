using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class ShopBagController : MonoBehaviour
{
    [Header("Data (������)")][Tooltip("������ ǥ���� ������ ���")] public List<ItemData> shopItems = new List<ItemData>();


    [Tooltip("���濡 ǥ���� ������ ���")]
    public List<ItemData> bagItems = new List<ItemData>();

    [Header("UI Refs (����)")]
    [Tooltip("��� ShopGrid(����ĭ) Transform")]
    public Transform shopGrid;

    [Tooltip("�ϴ� BagGrid(����ĭ) Transform")]
    public Transform bagGrid;

    [Tooltip("ItemCard(������ ī��) ������")]
    public GameObject cardPrefab;

    [Tooltip("������ �� ���")]
    public ItemModal itemModal;

    [Header("Player (�÷��̾�)")]
    [Tooltip("���� ����")]
    public int playerCoins = 10000;

    // ���� ���: �̹� �ܰ�� 4��2 ����(8ĭ)
    private const int VisibleCount = 8;

    private void Start()
    {
        Debug.Log($"[SBC] Before Init -> shop:{shopItems?.Count}, bag:{bagItems?.Count}");

        // �ʼ� ���� ����(���� ���ܿ�)
        if (shopGrid == null) Debug.LogWarning("ShopBagController: shopGrid ������ ������ϴ�.");
        if (bagGrid == null) Debug.LogWarning("ShopBagController: bagGrid ������ ������ϴ�.");
        if (cardPrefab == null) Debug.LogWarning("ShopBagController: cardPrefab ������ ������ϴ�.");
        if (itemModal == null) Debug.LogWarning("ShopBagController: itemModal ������ ������ϴ�.");

        // ����� ���� ������ (�ʿ� �� ���)
         if (shopItems.Count == 0) shopItems = CreateDummy(20);
           Debug.Log($"[SBC] shopItems dummy -> {shopItems.Count}");

         if (bagItems.Count  == 0) bagItems  = CreateDummy(8);
        Debug.Log($"[SBC] bagItems dummy -> {bagItems.Count}");



        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshShopGrid();
        RefreshBagGrid();
    }

    private void RefreshShopGrid()
    {
        if (!shopGrid || !cardPrefab) return;
        ClearChildren(shopGrid);

        int count = Mathf.Min(VisibleCount, shopItems.Count);
        for (int i = 0; i < count; i++)
        {
            var data = shopItems[i];
            if (data == null) continue; // null ���

            CreateCard(shopGrid, data, ItemSource.Shop);
        }
    }

    private void RefreshBagGrid()
    {
        if (!bagGrid || !cardPrefab) return;
        ClearChildren(bagGrid);

        int count = Mathf.Min(VisibleCount, bagItems.Count);
        for (int i = 0; i < count; i++)
        {
            var data = bagItems[i];
            if (data == null) continue; // null ���

            CreateCard(bagGrid, data, ItemSource.Bag);
        }
    }

    // ���� ī�� ����/���ε� ����
    private void CreateCard(Transform parent, ItemData data, ItemSource source)
    {
        var go = Instantiate(cardPrefab, parent);
        var view = go.GetComponent<ItemCardView>();

        System.Action<ItemData> onClick = (item) =>
        {
            ShowModal(item, source);
        };

        if (view != null) view.SetData(data, onClick);
        else Debug.LogWarning("ShopBagController: ItemCardView ������Ʈ�� ã�� ���߽��ϴ�.");
    }

    private void ShowModal(ItemData item, ItemSource source)
    {
        {
            if (itemModal == null || item == null) return;



            // �� �б�
            itemModal.SetConfirmLabel(source == ItemSource.Shop ? "����" : "�Ǹ�");

            // �ִ밪 ��� ����
            int min = 1;
            int max;
            if (source == ItemSource.Shop)
            {
                // �ܰ� 0 ���
                int unit = Mathf.Max(1, item.price);
                // ���� �������� �� �� �ִ� �ִ� ����
                max = Mathf.Max(1, playerCoins / unit);
                // ���� ��� �ִٸ�: max = Mathf.Min(max, item.shopStock);
            }
            else
            {
                // ���� ���� ����
                int owned = GetOwnedCount(item); // �Ʒ� �ӽ� ���� ����
                max = Mathf.Max(1, owned);
            }

            // ��� ǥ��
            itemModal.Show(item, (confirmedItem) =>
            {
                int qty = itemModal.GetSelectedQuantity(); // ��޿��� ���õ� ���� �б�
                Debug.Log($"Ȯ��: {confirmedItem.name}, ���� {qty}, {(source == ItemSource.Shop ? "����" : "�Ǹ�")}");

                // ���� �ܰ迡�� Purchase/Sell ���� �� �Ʒ� ȣ�� ����
                // if (source == ItemSource.Shop) Purchase(confirmedItem, qty);
                // else Sell(confirmedItem, qty);
                // RefreshAll();
            });

            // 4) ��޿� ���� ���� ����(ǥ�� ����)
            int initial = 1;
            int ownedCountForDisplay = (source == ItemSource.Bag) ? GetOwnedCount(item) : 0;
            itemModal.ConfigureQuantity(source, min, max, initial, playerCoins, ownedCountForDisplay);
        }
    }
    private int GetOwnedCount(ItemData item)
    { // bagItems �� ���� ������ ���� �ջ� ������ �ִٸ� ���⿡ ���� // ����� ���� �̱��� �������� 1 ��ȯ
      return 1; }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    // ����: �׽�Ʈ�� ���� ������
    private List<ItemData> CreateDummy(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"������ {i + 1}",
                price = 100 + i * 10,
                icon = null
            });
        }
        return list;
    }
}