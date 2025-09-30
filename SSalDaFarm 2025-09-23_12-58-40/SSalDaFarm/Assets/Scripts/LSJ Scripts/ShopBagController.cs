using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Card Prefabs (카드 프리팹)")]
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private GameObject bagCardPrefab;


    [Tooltip("아이템 상세 모달")]
    public ItemModal itemModal;

    [Header("Player (플레이어)")]
    [Tooltip("보유 Coins")]
    public int playerCoins = 10000;

    // 내부 상수: 이번 단계는 4×2 고정(8칸)
    [SerializeField] private int shopVisibleCount = 5; // 상점용
    [SerializeField] private int bagVisibleCount = 8; // 가방용

    [SerializeField] private UiToast uiToast;

    [SerializeField] private TMP_Text WalletText; // ModalOverlay 내 지갑 텍스트
    [SerializeField] private TMP_Text WalletTextbag; // BagTitle 내 지갑 텍스트


    private void Start()
    {
        Debug.Log($"[SBC] Before Init -> shop:{shopItems?.Count}, bag:{bagItems?.Count}");

        // 필수 참조 점검(빠른 진단용)
        if (shopGrid == null) Debug.LogWarning("ShopBagController: shopGrid 참조가 비었습니다.");
        if (bagGrid == null) Debug.LogWarning("ShopBagController: bagGrid 참조가 비었습니다.");
        if (shopCardPrefab == null) Debug.LogWarning("ShopBagController: shopCardPrefab 참조가 비었습니다.");
        if (bagCardPrefab == null) Debug.LogWarning("ShopBagController: bagCardPrefab 참조가 비었습니다.");

        if (itemModal == null) Debug.LogWarning("ShopBagController: itemModal 참조가 비었습니다.");

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

    // 공통 카드 생성/바인딩 헬퍼
    private void CreateCard(Transform parent, ItemData data, ItemSource source)
    {
        if (data == null || parent == null) { Debug.LogWarning("[CreateCard] parent 또는 data가 null입니다."); return; }
        // 1) 클릭 콜백 먼저 정의
        System.Action<ItemData> onClick = (item) =>
        {
            ShowModal(item, source);
        };

        // 2) 프리팹 선택
        GameObject prefab = null;
        if (source == ItemSource.Shop)
            prefab = shopCardPrefab;   // 상점용 프리팹
        else
            prefab = bagCardPrefab;    // 가방용 프리팹

        if (prefab == null)
        {
            Debug.LogError($"[CreateCard] 프리팹이 비었습니다. source={source}");
            return;
        }

        // 3) 생성
        var go = Instantiate(prefab, parent);

        // 4) 뷰 타입 분기 + 데이터 바인딩
        if (source == ItemSource.Shop)
        {
            var view = go.GetComponent<ShopItemCardView>();
            if (view == null)
            {
                Debug.LogError("[CreateCard] ShopItemCardView를 찾을 수 없습니다. 상점 프리팹 구성을 확인하세요.");
                return;
            }
            view.SetData(data, onClick);
        }
        else
        {
            // 가방은 BagItemCardView 또는 기존 ItemCardView 중 하나로 통일하세요.
            var view = go.GetComponent<BagItemCardView>();
            if (view == null)
            {
                Debug.LogError("[CreateCard] BagItemCardView를 찾을 수 없습니다. 가방 프리팹 구성을 확인하세요.");
                return;
            }
            view.SetData(data, onClick);
        }

        // 5) 진단 로그
        Debug.Log($"[Card Bind] {data.name}, price={data.price}, source={source}");
    }


    private void ShowModal(ItemData item, ItemSource source)
    {
        {
            if (itemModal == null || item == null) return;



            // 라벨 분기
            itemModal.SetConfirmLabel(source == ItemSource.Shop ? "Buy" : "Sell");


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
            itemModal.ConfigureQuantity(source, min, max, initial);
        }
    }
    private int GetOwnedCount(ItemData item)
    { if (item == null || string.IsNullOrEmpty(item.id)) return 0; int count = 0; for (int i = 0; i < bagItems.Count; i++) if (bagItems[i] != null && bagItems[i].id == item.id) count++; return count; }

    // 추가: 구매(Coins 차감 → 개별 엔트리 추가)
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
        Toast($"Sold {item.name} x{qty}, gain={total:N0}, coins={playerCoins:N0}");
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
        var list = new List<ItemData>(count); for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                id = "dummy_i",
                name = "dummy" + (i + 1),
                price = Random.Range(50, 500) * 10,
                icon = null, // 아이콘 없으면 null로 테스트
                description = "테스트용 더미 아이템"

            });
        }
        return list;
    }
}