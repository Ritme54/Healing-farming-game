using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class ItemCardView : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText; 
    public TMP_Text priceText; 
    public Button rootButton;
    private ItemData data;
    private System.Action<ItemData> onClick;

    public void SetData(ItemData d, System.Action<ItemData> onClickHandler)
    {
        data = d;
        onClick = onClickHandler;

        if (iconImage) iconImage.sprite = d.icon;
        if (nameText) nameText.text = d.name;
        if (priceText) priceText.text = string.Format("{0:N0} ÄÚÀÎ", d.price);

        if (rootButton)
        {
            rootButton.onClick.RemoveAllListeners();
            rootButton.onClick.AddListener(() => onClick?.Invoke(data));
        }
    }
   
}
