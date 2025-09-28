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

        Debug.Log(typeof(ItemData).FullName);

        RefreshAll();
    }

    private void Toast(string msg) { Debug.LogWarning($"[Toast] {msg}"); }

    public void OnClickShopItem(ItemData item)
    {
        ShowModal(item, ItemSource.Shop);

    }

    public void OnClickBagItem(ItemData item)
    {
        ShowModal(item, ItemSource.Bag);
    }

    private ItemData CloneItem(ItemData src)
    {
        { return new  ItemData { id = src.id, name = src.name, price = src.price, icon = src.icon, description = src.description }; }
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
        Debug.Log($"[Card Bind] {name}, price={0}");
    }

    private void ShowModal(ItemData item, ItemSource source)
    {
        {
            if (itemModal == null || item == null) return;



            // �� �б�
            itemModal.SetConfirmLabel(source == ItemSource.Shop ? "����" : "�Ǹ�");


            int unit = Mathf.Max(1, item.price);
            int min = 1;
            int max = (source == ItemSource.Shop)
                ? Mathf.Max(1, playerCoins / unit)
                : Mathf.Max(1, GetOwnedCount(item));

            // ��� ǥ��
            itemModal.Show(item, (confirmedItem) =>
            {
                int qty = itemModal.GetSelectedQuantity(); // ��޿��� ���õ� ���� �б�
                Debug.Log($"Ȯ��: {confirmedItem.name}, ���� {qty}, {(source == ItemSource.Shop ? "����" : "�Ǹ�")}");


                bool ok = (source == ItemSource.Shop)
                    ? Purchase(confirmedItem, qty)
                    : Sell(confirmedItem, qty);

                if (ok) RefreshAll();
            });
            int initial = 1;
            int ownedCountForDisplay = (source == ItemSource.Bag) ? GetOwnedCount(item) : 0;
            itemModal.ConfigureQuantity(source, min, max, initial, playerCoins, ownedCountForDisplay);
        }
    }
    private int GetOwnedCount(ItemData item)
   { if (item == null || string.IsNullOrEmpty(item.id)) return 0; int count = 0; for (int i = 0; i < bagItems.Count; i++) if (bagItems[i] != null && bagItems[i].id == item.id) count++; return count; }

    // �߰�: ����(���� ���� �� ���� ��Ʈ�� �߰�)
    private bool Purchase(ItemData item, int qty)
    {
        int unit = Mathf.Max(1, item.price);
        long total = (long)unit * qty;

        if (playerCoins < total)
        {
            Toast("������ �����մϴ�.");
            return false;
        }

        playerCoins -= (int)total;
        for (int i = 0; i < qty; i++)
            bagItems.Add(CloneItem(item));

        Debug.Log($"[Purchase] {item.name} x{qty}, cost={total:N0}, coins={playerCoins:N0}");
        return true;
    }
        private bool Sell(ItemData item, int qty)
    {
        int owned = GetOwnedCount(item);
        if (owned < qty)
        {
            Toast("���� ������ �����մϴ�.");
            return false;
        }

        int unit = Mathf.Max(1, item.price);
        long total = (long)unit * qty;

        int removed = 0;
        for (int i = bagItems.Count - 1; i >= 0 && removed < qty; i--)
        {
            var it = bagItems[i];
            if (it != null && it.id == item.id)
            {
                bagItems.RemoveAt(i);
                removed++;
            }
        }

        playerCoins += (int)total;
        Debug.Log($"[Sell] {item.name} x{qty}, gain={total:N0}, coins={playerCoins:N0}");
        return true;
    }



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