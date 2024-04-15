using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class Book3D : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _spineText;
        [SerializeField] private TMP_Text _genre;
        [SerializeField] private Image _bg;
        
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private MeshRenderer _mesh;
        
        [SerializeField] private BookData _defaultBook;

        private void Start()
        {
            if (_defaultBook != null)
                SetBook(_defaultBook);
        }
    
        public void SetBook(BookData book)
        {
            _title.text = book.BookName;
            _spineText.text = book.BookName;
            _genre.text = book.Genre.ToUpper();
            
            _title.color = book.TitleColor;
            _spineText.color = book.TitleColor;
            _genre.color = book.GenreColor;

            _bg.sprite = book.CoverImage;
            
            Material mat = new Material(_baseMaterial);
            mat.color = book.BookColor;

            var matList = new List<Material>();
            _mesh.GetSharedMaterials(matList);
            matList[1] = mat;
            _mesh.SetSharedMaterials(matList);
        }
    }
}