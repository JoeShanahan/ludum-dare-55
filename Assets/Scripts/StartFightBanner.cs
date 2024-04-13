
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class StartFightBanner : MonoBehaviour
    {
        [SerializeField] private TMP_Text _topText;
        [SerializeField] private TMP_Text _bottomText;
        [SerializeField] private Mask _mask;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            var rect = transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
            _mask.enabled = false;

            _topText.transform.localScale = Vector3.zero;
            _bottomText.transform.localScale = Vector3.zero;
            
            PlayBannerAnimation();
        }

        public void PlayBannerAnimation()
        {
            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            var rect = transform as RectTransform;
            Vector2 newSize = new Vector2(rect.sizeDelta.x, 200);
            rect.DOSizeDelta(newSize, 0.5f).SetEase(Ease.OutExpo);
            _mask.enabled = false;

            _topText.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack, 3).SetDelay(0.1f);
            _bottomText.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack, 2).SetDelay(0.15f);
            
            yield return new WaitForSeconds(2);

            _mask.enabled = true;

            newSize = new Vector2(rect.sizeDelta.x, 0);
            rect.DOSizeDelta(newSize, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
