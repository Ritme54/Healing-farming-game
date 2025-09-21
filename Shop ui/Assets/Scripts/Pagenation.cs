using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pagenation : MonoBehaviour
{
    [Header("UI References")] public Transform gridParent; // ShopGrid �Ǵ� BagGrid
    public GameObject cardPrefab; // ItemCard ������
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
        // ����� ���� �����Ͱ� �ʿ��ϸ� ���⼭ ä���� �˴ϴ�.
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
        // ���� ī�� ����
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }

        // ���� ������ ���� ���
        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, allItems.Count);

        // ī�� ����/���ε�
        for (int i = start; i < end; i++)
        {
            var go = Instantiate(cardPrefab, gridParent);
            var view = go.GetComponent<ItemCardView>();

            // Ŭ�� �� ��/���� �˾����� ������ �ݹ�
            System.Action<ItemData> onClick = (item) =>
            {
                // TODO: �� �г�/���� �˾� ����
                Debug.Log($"ī�� Ŭ��: {item.name} / {item.price}");
            };

            if (view != null) view.SetData(allItems[i], onClick);
        }

        // ���� ĭ�� ����� ������, ���� placeholder�� ä�� �ʿ�� �����ϴ�.
        // ������ ǥ��/��ư ���� ����
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

    // �ʿ� �� �׽�Ʈ�� ���� ������
    private List<ItemData> CreateDummy(int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new ItemData
            {
                name = $"������ {i + 1}",
                price = 100 + i * 25,
                icon = null
            });
        }
        return list;
    }
}