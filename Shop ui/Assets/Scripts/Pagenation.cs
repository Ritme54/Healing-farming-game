using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pagenation : MonoBehaviour
{
    [Header("UI References")] public Transform gridParent; // ShopGrid 또는 BagGrid
    public GameObject cardPrefab; // ItemCard 프리팹
    public TMP_Text pageText; // "1 / N"
    public Button prevButton; public Button nextButton;
    [Header("Layout")]
    public int rows = 4;
    public int cols = 4;

    [Header("Data")]
    public List<ItemData> allItems = new List<ItemData>();

    private int itemsPerPage;
    private int currentPage = 0;
    private int totalPages = 1;

    private void Awake()
    {
        itemsPerPage = rows * cols;

        if (prevButton) prevButton.onClick.AddListener(OnClickPrev);
        if (nextButton) nextButton.onClick.AddListener(OnClickNext);
    }

    private void Start()
    {
        // 데모용 더미 데이터가 필요하면 여기서 채워도 됩니다.
        // if (allItems.Count == 0) allItems = CreateDummy(40);

        RecalculatePages();
        RefreshPage();
    }

    public void SetItems(List<ItemData> items)
    {
        allItems = items ?? new List<ItemData>();
        currentPage = 0;
        RecalculatePages();
        RefreshPage();
    }

    private void RecalculatePages()
    {
        int count = Mathf.Max(0, allItems.Count);
        totalPages = Mathf.Max(1, Mathf.CeilToInt(count / (float)itemsPerPage));
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);
    }

    private void RefreshPage()
    {
        // 기존 카드 제거
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }

        // 현재 페이지 범위 계산
        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, allItems.Count);

        // 카드 생성/바인딩
        for (int i = start; i < end; i++)
        {
            var go = Instantiate(cardPrefab, gridParent);
            var view = go.GetComponent<ItemCardView>();

            // 클릭 시 상세/수량 팝업으로 연결할 콜백
            System.Action<ItemData> onClick = (item) =>
            {
                // TODO: 상세 패널/수량 팝업 열기
                Debug.Log($"카드 클릭: {item.name} / {item.price}");
            };

            if (view != null) view.SetData(allItems[i], onClick);
        }

        // 남은 칸을 비워도 되지만, 굳이 placeholder를 채울 필요는 없습니다.
        // 페이지 표시/버튼 상태 갱신
        if (pageText) pageText.text = $"{currentPage + 1} / {totalPages}";
        if (prevButton) prevButton.interactable = currentPage > 0;
        if (nextButton) nextButton.interactable = currentPage < totalPages - 1;
    }

    private void OnClickPrev()
    {
        if (currentPage <= 0) return;
        currentPage--;
        RefreshPage();
    }

    private void OnClickNext()
    {
        if (currentPage >= totalPages - 1) return;
        currentPage++;
        RefreshPage();
    }

    // 필요 시 테스트용 더미 데이터
    private List<ItemData> CreateDummy(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"아이템 {i + 1}",
                price = 100 + i * 25,
                icon = null
            });
        }
        return list;
    }
}