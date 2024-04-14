using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class BookCover : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _genre;
        [SerializeField] private Image _bg;

        public void SetBook(BookData book)
        {
            _title.text = book.BookName;
            _genre.text = book.Genre;
            
            _title.color = book.TitleColor;
            _genre.color = book.GenreColor;

            _bg.color = book.BookColor;
        }
    }
}
