using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public enum ItemSource
{
    Shop, // 상점
    Bag // 가방
}

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
        // 필수 참조 점검(빠른 진단용)
        if (shopGrid == null) Debug.LogWarning("ShopBagController: shopGrid 참조가 비었습니다.");
        if (bagGrid == null) Debug.LogWarning("ShopBagController: bagGrid 참조가 비었습니다.");
        if (cardPrefab == null) Debug.LogWarning("ShopBagController: cardPrefab 참조가 비었습니다.");
        if (itemModal == null) Debug.LogWarning("ShopBagController: itemModal 참조가 비었습니다.");

        // 데모용 더미 데이터 (필요 시 사용)
        // if (shopItems.Count == 0) shopItems = CreateDummy(20);
        // if (bagItems.Count  == 0) bagItems  = CreateDummy(8);

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
        else Debug.LogWarning("ShopBagController: ItemCardView 컴포넌트를 찾지 못했습니다.");
    }

    private void ShowModal(ItemData item, ItemSource source)
    {
        if (itemModal == null)
        {
            Debug.LogWarning("ItemModal 참조가 비어 있습니다. 인스펙터에서 연결해 주세요.");
            return;
        }
        if (item == null)
        {
            Debug.LogWarning("ShowModal: item이 null입니다.");
            return;
        }

        // 모달의 확인 버튼 라벨을 출처에 따라 변경(구매/판매)
        itemModal.SetConfirmLabel(source == ItemSource.Shop ? "구매" : "판매");

        // 모달 표시 및 확인 콜백 연결
        itemModal.Show(item, (confirmedItem) =>
        {
            if (confirmedItem == null) return;

            Debug.Log($"모달 확인: {confirmedItem.name} ({(source == ItemSource.Shop ? "구매" : "판매")})");
            // TODO: 다음 단계에서 QuantityPopup.Show(...) 연결
        });
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