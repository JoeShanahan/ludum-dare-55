using System;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    [CreateAssetMenu(menuName = "Data/" + nameof(BookData))]
    public class BookData : ScriptableObject
    {
        public string BookName;

        public string Genre;

        public Color BookColor;
        public Color TitleColor;
        public Color GenreColor;
        public Sprite CoverImage;
        
        [TextArea(3, 10)]
        public string LongDescription;

        public List<SummonData> Summons;

        
        [ContextMenu("Check!")]
        public void CheckForInvalid()
        {
            foreach (SummonData dat in Summons)
            {
                foreach (var itm in dat.ActionQueue)
                {
                    if (itm == BoardAction.AOEAttack)
                    {
                        if (dat.AoePrefab == null)
                        {
                            Debug.LogWarning($"Missing AOE prefab on {dat.Name}");
                        }
                    }
                }
            }
        }
        
        /*public int PageCount
        {
            get
            {
                int total = 0;
                
                foreach (SummonData dat in Summons)
                {
                    total += dat.Count;
                }

                return total;
            }
        }*/
    }
    
    

    [Serializable]
    public class SummonData
    {
        public string Name;
        public Sprite Sprite;
        //public int Count = 1;
        public int Weight = 1;
        public bool DoesStartWith;

        [Range(0, 4)]
        public int HealthPoints = 2;
        
        [Range(0, 4)]
        public int Attack = 1;

        [Range(0, 1)] 
        public int Shield;
        
        [TextArea(3, 10)]
        public string Description;
        
        [Header("Behaviour")]
        public List<BoardAction> ActionQueue;
        public AttackArea AttackArea;
        public GameObject AoePrefab;
    }


    public enum AttackArea
    {
        OneByOne,
        Sides,
        Circle,
        Row,
        Column,
        ThreeByOne,
        TwoByOne,
        OneByThree,
        Cardinals
    }
    
    public enum BoardAction
    {
        Wait,
        Move,
        DoubleMove,
        AOEAttack,
        X_Bounce,       // X means not to be used in the editor
        X_HalfBounce    // X means not to be used in the editor
    }
}