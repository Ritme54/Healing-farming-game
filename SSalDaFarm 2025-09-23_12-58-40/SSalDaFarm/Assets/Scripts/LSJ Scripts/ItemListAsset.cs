using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Game/Item List", order = 1)] public class ItemListAsset : ScriptableObject { public ItemListEntry[] entries; }

[System.Serializable]
public class ItemListEntry
{
    public ItemDataAsset item;
    public int overridePrice = -1; // -1이면 item.price 사용
}