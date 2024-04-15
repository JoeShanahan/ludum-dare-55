using UnityEngine;

namespace LudumDare55
{
    [CreateAssetMenu(fileName = "OpponentData", menuName = "Data/OpponentData")]
    public class OpponentData : ScriptableObject
    {
        public string PersonName;
        public string GenreDescription;
        public BookData ChosenBook;
        public AudioClip ThemeSong;

        public Sprite Sprite;
        
        public Color _tileColA;
        public Color _tileColB;
        public Color _tileColC;
        public Color _tileColD;
        
        public Color _bgColA;
        public Color _bgColB;

        public Sprite sprite;

        public Texture2D BackgroundTex;

        [TextArea(3, 10)] public string FirstLine;
        [TextArea(3, 10)] public string YourResponse;
        [TextArea(3, 10)] public string SecondLine;
        [TextArea(3, 10)] public string FinalResponse;
    }
}
