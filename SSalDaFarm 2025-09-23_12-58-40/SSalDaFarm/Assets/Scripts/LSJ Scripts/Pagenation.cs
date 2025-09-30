using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;

public class Pagenation : MonoBehaviour
{
    [SerializeField] private ItemModal itemModal; // 상세/수량 팝업 연결용
    [SerializeField] private bool isShop; // 이 스크립트 인스턴스가 상점용인지 가방용인지 인스펙터에서 체크
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private GameObject bagCardPrefab;
    [Header("UI References")] public Transform gridParent; // ShopGrid 또는 BagGrid
    public TMP_Text pageText; // "1 / N"
    public Button prevButton; public Button nextButton;
    [Header("Layout")]
    public int rows = 1;
    public int cols = 5;

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

        if (isShop) Debug.Log("[Pagenation]isShop=TRUE,instantiate:shopCardPrefab?.name"); 
        else
            Debug.Log("[Pagenation] isShop=FALSE, instantiate: {bagCardPrefab?.name}");

        // Clear
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        // Range
        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, allItems.Count);

        for (int i = start; i < end; i++)
        {
            var prefab = isShop ? shopCardPrefab : bagCardPrefab;
            if (prefab == null)
            {
                Debug.LogError("카드 프리팹이 비어 있습니다. 인스펙터에서 연결해 주세요.");
                break;
            }

            var go = Instantiate(prefab, gridParent);

            // 공통 클릭 콜백
            System.Action<ItemData> onClick = (item) =>
            {
                if (itemModal != null)
                {
                    itemModal.Show(item, (confirmed) =>
                    {
                        // TODO: 수량 팝업 등 연결
                        Debug.Log($"모달 확인: {confirmed.name} / {confirmed.price}");
                    });
                }
            };

            if (isShop)
            {
               var view = go.GetComponent<ShopItemCardView>();
               if (view == null)
               {
                   Debug.LogWarning("ShopItemCardView 카드에 없습니다. 프리팹 구성을 확인해 주세요.");
               }
               else view.SetData(allItems[i], onClick);
            }
            else
            {
                // 가방은 기존 뷰를 사용한다고 가정
                var view = go.GetComponent<BagItemCardView>();
                if (view == null)
                {
                    Debug.LogWarning("BagItemCardView가 카드에 없습니다. 프리팹 구성을 확인해 주세요.");
                }
                else view.SetData(allItems[i], onClick);
            }
        }

        // UI 상태
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
}