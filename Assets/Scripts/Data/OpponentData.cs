using UnityEngine;

namespace LudumDare55
{
    [CreateAssetMenu(fileName = "OpponentData", menuName = "Data/OpponentData")]
    public class OpponentData : ScriptableObject
    {
        public string PersonName;
        public BookData ChosenBook;

        public Color _tileColA;
        public Color _tileColB;
        
        public Color _bgColA;
        public Color _bgColB;

        [TextArea(3, 10)] public string FirstLine;
        [TextArea(3, 10)] public string YourResponse;
        [TextArea(3, 10)] public string SecondLine;
        [TextArea(3, 10)] public string FinalResponse;
    }
}
