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

        [TextArea(3, 10)]
        public string LongDescription;

        public List<SummonData> Summons;

        public int PageCount
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
        }
    }

    [Serializable]
    public class SummonData
    {
        public string Name;
        public Sprite Sprite;
        public int Count = 1;
        public bool DoesStartWith;
        
        [TextArea(3, 10)]
        public string Description;
        
        [Header("Behaviour")]
        public List<BoardAction> ActionQueue;
        public AttackArea AttackArea;
        public AttackMode AttackMode;
    }

    public enum AttackMode
    {
        Never,
        Bump,
        InRange,
        OnDeath
    }

    public enum AttackArea
    {
        InFront,
        Sides,
        Circle,
        Ranged,
        Row,
        Column
    }
    
    public enum BoardAction
    {
        Wait,
        Move,
        DoubleMove,
        Attack,
        Bounce,
        HalfBounce,
        AttackMove
    }
}