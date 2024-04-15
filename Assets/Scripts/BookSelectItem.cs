using System;
using UnityEngine;

namespace LudumDare55
{
    public class BookSelectItem : MonoBehaviour
    {
        public BookData BookData => _bookData;
        
        [SerializeField] private Book3D _book;
        [SerializeField] private BookData _bookData;

        public void OnEnable()
        {
            _book.SetBook(_bookData);
        }

        public void OnBtnPress()
        {
            FindFirstObjectByType<NewMainMenuController>().BtnPressBookData(_bookData);
        }
    }
}
