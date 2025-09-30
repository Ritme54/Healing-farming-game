using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Card Prefabs (ī�� ������)")]
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private GameObject bagCardPrefab;


    [Tooltip("������ �� ���")]
    public ItemModal itemModal;

    [Header("Player (�÷��̾�)")]
    [Tooltip("���� Coins")]
    public int playerCoins = 10000;

    // ���� ���: �̹� �ܰ�� 4��2 ����(8ĭ)
    [SerializeField] private int shopVisibleCount = 5; // ������
    [SerializeField] private int bagVisibleCount = 8; // �����

    [SerializeField] private UiToast uiToast;

    [SerializeField] private TMP_Text WalletText; // ModalOverlay �� ���� �ؽ�Ʈ
    [SerializeField] private TMP_Text WalletTextbag; // BagTitle �� ���� �ؽ�Ʈ


    private void Start()
    {
        Debug.Log($"[SBC] Before Init -> shop:{shopItems?.Count}, bag:{bagItems?.Count}");

        // �ʼ� ���� ����(���� ���ܿ�)
        if (shopGrid == null) Debug.LogWarning("ShopBagController: shopGrid ������ ������ϴ�.");
        if (bagGrid == null) Debug.LogWarning("ShopBagController: bagGrid ������ ������ϴ�.");
        if (shopCardPrefab == null) Debug.LogWarning("ShopBagController: shopCardPrefab ������ ������ϴ�.");
        if (bagCardPrefab == null) Debug.LogWarning("ShopBagController: bagCardPrefab ������ ������ϴ�.");

        if (itemModal == null) Debug.LogWarning("ShopBagController: itemModal ������ ������ϴ�.");

        if (shopItems.Count == 0) shopItems = CreateDummy(20);

        if (bagItems.Count == 0) bagItems = CreateDummy(8);

        RefreshCoinsUI();

        RefreshAll();
    }


    private void Toast(string message, bool warn = false)
    {
        if (warn) Debug.LogWarning("[Toast]message");
        else
            Debug.Log("[Toast] {message}");
    uiToast?.Show(message);
}
 
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
        {
            return new ItemData
            {
                id = src.id,
                name = src.name,
                price = src.price,
                icon = src.icon,
                description = src.description
            };
        }
    }


    private void RefreshCoinsUI()
    { string formatted = $"{playerCoins:N0}Coins"; 
        if (WalletText != null) WalletText.text = formatted;
        if (WalletTextbag != null) WalletTextbag.text = formatted; }

    public void RefreshAll()
    {
        RefreshCoinsUI();
        RefreshShopGrid();
        RefreshBagGrid();
    }

    private void RefreshShopGrid()
    {
        if (!shopGrid) return;
        ClearChildren(shopGrid);
        int count = Mathf.Min(shopVisibleCount, shopItems.Count);
        for (int i = 0; i < count; i++)
        {
            var data = shopItems[i];
            if (data == null) continue;

            CreateCard(shopGrid, data, ItemSource.Shop);
        }
    }

    private void RefreshBagGrid()
    {
        if (!bagGrid) return;
        ClearChildren(bagGrid);

        int count = Mathf.Min(bagVisibleCount, bagItems.Count);
        for (int i = 0; i < count; i++)
        {
            var data = bagItems[i];
            if (data == null) continue;

            CreateCard(bagGrid, data, ItemSource.Bag);
        }
    }

    // ���� ī�� ����/���ε� ����
    private void CreateCard(Transform parent, ItemData data, ItemSource source)
    {
        if (data == null || parent == null) { Debug.LogWarning("[CreateCard] parent �Ǵ� data�� null�Դϴ�."); return; }
        // 1) Ŭ�� �ݹ� ���� ����
        System.Action<ItemData> onClick = (item) =>
        {
            ShowModal(item, source);
        };

        // 2) ������ ����
        GameObject prefab = null;
        if (source == ItemSource.Shop)
            prefab = shopCardPrefab;   // ������ ������
        else
            prefab = bagCardPrefab;    // ����� ������

        if (prefab == null)
        {
            Debug.LogError($"[CreateCard] �������� ������ϴ�. source={source}");
            return;
        }

        // 3) ����
        var go = Instantiate(prefab, parent);

        // 4) �� Ÿ�� �б� + ������ ���ε�
        if (source == ItemSource.Shop)
        {
            var view = go.GetComponent<ShopItemCardView>();
            if (view == null)
            {
                Debug.LogError("[CreateCard] ShopItemCardView�� ã�� �� �����ϴ�. ���� ������ ������ Ȯ���ϼ���.");
                return;
            }
            view.SetData(data, onClick);
        }
        else
        {
            // ������ BagItemCardView �Ǵ� ���� ItemCardView �� �ϳ��� �����ϼ���.
            var view = go.GetComponent<BagItemCardView>();
            if (view == null)
            {
                Debug.LogError("[CreateCard] BagItemCardView�� ã�� �� �����ϴ�. ���� ������ ������ Ȯ���ϼ���.");
                return;
            }
            view.SetData(data, onClick);
        }

        // 5) ���� �α�
        Debug.Log($"[Card Bind] {data.name}, price={data.price}, source={source}");
    }


    private void ShowModal(ItemData item, ItemSource source)
    {
        {
            if (itemModal == null || item == null) return;



            // �� �б�
            itemModal.SetConfirmLabel(source == ItemSource.Shop ? "Buy" : "Sell");


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
            itemModal.ConfigureQuantity(source, min, max, initial);
        }
    }
    private int GetOwnedCount(ItemData item)
    { if (item == null || string.IsNullOrEmpty(item.id)) return 0; int count = 0; for (int i = 0; i < bagItems.Count; i++) if (bagItems[i] != null && bagItems[i].id == item.id) count++; return count; }

    // �߰�: ����(Coins ���� �� ���� ��Ʈ�� �߰�)
    private bool Purchase(ItemData item, int qty)
    {
        int unit = Mathf.Max(1, item.price);
        long total = (long)unit * qty;

        if (playerCoins < total)
        {
            Toast("Not Eneufdsafudsf.", warn: true);

            return false;
        }

        playerCoins -= (int)total;
        for (int i = 0; i < qty; i++)
            bagItems.Add(CloneItem(item));
        Debug.Log($"[Purchase] {item.name} x{qty}, cost={total:N0}, coins={playerCoins:N0}");
        Toast($"[Purchase] {item.name} x{qty}, cost={total:N0}, coins={playerCoins:N0}");

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
        Toast($"Sold {item.name} x{qty}, gain={total:N0}, coins={playerCoins:N0}");
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
        var list = new List<ItemData>(count); for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                id = "dummy_i",
                name = "dummy" + (i + 1),
                price = Random.Range(50, 500) * 10,
                icon = null, // ������ ������ null�� �׽�Ʈ
                description = "�׽�Ʈ�� ���� ������"

            });
        }
        return list;
    }
}