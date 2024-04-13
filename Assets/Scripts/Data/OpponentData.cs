using UnityEngine;

namespace LudumDare55
{
    [CreateAssetMenu(fileName = "OpponentData", menuName = "Data/OpponentData")]
    public class OpponentData : ScriptableObject
    {
        public string PersonName;
        public BookData ChosenBook;

        [TextArea(3, 10)] public string FirstLine;
        [TextArea(3, 10)] public string YourResponse;
        [TextArea(3, 10)] public string SecondLine;
        [TextArea(3, 10)] public string FinalResponse;
    }
}
