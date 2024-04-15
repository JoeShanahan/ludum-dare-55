using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LudumDare55
{
    public class BookSelectItem : MonoBehaviour, ISelectHandler
    {
        public BookData BookData => _bookData;
        
        [SerializeField] private Book3D _book;
        [SerializeField] private BookData _bookData;

        
        //Do this when the selectable UI object is selected.
        public void OnSelect(BaseEventData eventData)
        {
            FindFirstObjectByType<NewMainMenuController>().OnBookSelected(this);
        }
        
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
