using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using Assets.Scripts;

public class BagItemCardView : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText; 
    public TMP_Text priceText; 
    public Button rootButton;
    private ItemData data;
    private System.Action<ItemData> onClick;

    public void SetData(ItemData d, System.Action<ItemData> onClickHandler)
    {
        Debug.Log($"[BagItemCardView] Bind -> name:{d?.name}, price:{d?.price}, icon:{(d?.icon ? d.icon.name : "null")}"); data = d; onClick = onClickHandler;


        if (iconImage) iconImage.sprite = d.icon;
        if (nameText) nameText.text = string.IsNullOrWhiteSpace(d.name) ? "(이름 없음)" : d.name;
        if (priceText) priceText.text = string.Format("{0:N0} Coin", Mathf.Max(0, d.price));

        if (rootButton)
        {
            rootButton.onClick.RemoveAllListeners();
            rootButton.onClick.AddListener(() => onClick?.Invoke(data));
        }
    }

}
