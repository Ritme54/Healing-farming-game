using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopBagController : MonoBehaviour
{
    [Header("Data (데이터)")][Tooltip("상점에 표시할 아이템 목록")] public List<ItemData> shopItems = new List<ItemData>();



    [Tooltip("가방에 표시할 아이템 목록")]
    public List<ItemData> bagItems = new List<ItemData>();

    [Header("UI Refs (참조)")]
    [Tooltip("상단 ShopGrid(상점칸) Transform")]
    public Transform shopGrid;

    [Tooltip("하단 BagGrid(가방칸) Transform")]
    public Transform bagGrid;

    [Tooltip("ItemCard(아이템 카드) 프리팹")]
    public GameObject cardPrefab;

    [Tooltip("아이템 상세 모달")]
    public ItemModal itemModal;

    [Header("Player (플레이어)")]
    [Tooltip("보유 코인")]
    public int playerCoins = 10000;

    // 내부 상수: 이번 단계는 4×2 고정(8칸)
    private const int VisibleCount = 8;




    private void Start()
    {
        Debug.Log($"[SBC] Before Init -> shop:{shopItems?.Count}, bag:{bagItems?.Count}");

        // 필수 참조 점검(빠른 진단용)
        if (shopGrid == null) Debug.LogWarning("ShopBagController: shopGrid 참조가 비었습니다.");
        if (bagGrid == null) Debug.LogWarning("ShopBagController: bagGrid 참조가 비었습니다.");
        if (cardPrefab == null) Debug.LogWarning("ShopBagController: cardPrefab 참조가 비었습니다.");
        if (itemModal == null) Debug.LogWarning("ShopBagController: itemModal 참조가 비었습니다.");

        // 데모용 더미 데이터 (필요 시 사용)
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
            if (data == null) continue; // null 방어

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
            if (data == null) continue; // null 방어

            CreateCard(bagGrid, data, ItemSource.Bag);
        }
    }

    // 공통 카드 생성/바인딩 헬퍼
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



            // 라벨 분기
            itemModal.SetConfirmLabel(source == ItemSource.Shop ? "구매" : "판매");


            int unit = Mathf.Max(1, item.price);
            int min = 1;
            int max = (source == ItemSource.Shop)
                ? Mathf.Max(1, playerCoins / unit)
                : Mathf.Max(1, GetOwnedCount(item));

            // 모달 표시
            itemModal.Show(item, (confirmedItem) =>
            {
                int qty = itemModal.GetSelectedQuantity(); // 모달에서 선택된 수량 읽기
                Debug.Log($"확인: {confirmedItem.name}, 수량 {qty}, {(source == ItemSource.Shop ? "구매" : "판매")}");


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

    // 추가: 구매(코인 차감 → 개별 엔트리 추가)
    private bool Purchase(ItemData item, int qty)
    {
        int unit = Mathf.Max(1, item.price);
        long total = (long)unit * qty;

        if (playerCoins < total)
        {
            Toast("코인이 부족합니다.");
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
            Toast("보유 수량이 부족합니다.");
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

    // 선택: 테스트용 더미 데이터
    private List<ItemData> CreateDummy(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"아이템 {i + 1}",
                price = 100 + i * 10,
                icon = null
            });
        }
        return list;
    }
}