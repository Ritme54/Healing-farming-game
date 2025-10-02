using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataAsset", menuName = "Game/Item Data Asset", order = 0)]
public class ItemDataAsset : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;
    public Sprite icon;
    public int price;
    // 기존 POCO 모델(Assets.Scripts.ItemData)로 변환하는 헬퍼
    public Assets.Scripts.ItemData ToRuntime()
    {
        return new Assets.Scripts.ItemData
        {
            id = id,
            name = itemName,
            description = description,
            icon = icon,
            price = price
        };
    }
}
