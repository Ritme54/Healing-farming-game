using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]

    public class ItemData
    {
        public string name; 
        public int price;
        public Sprite icon;
        public string description;
        public string id;        
    }

    public static class ItemDataExtensions
    {
        public static List<ItemData> CreateDummy(int count)
        {
            var list = new List<ItemData>();
            for (int i = 1; i <= count; i++)
            {
                var item = new ItemData
                {
                    id = Guid.NewGuid().ToString(),
                    name = "Item " + i,
                    price = UnityEngine.Random.Range(100, 10000),
                    description = "This is a description for Item " + i,
                    icon = null // 아이콘은 필요에 따라 설정
                };
                list.Add(item);
            }
            return list;
        }
    }



}


