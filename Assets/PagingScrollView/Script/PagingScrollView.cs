using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace doublsb.UI
{
    public class PagingScrollView : MonoBehaviour, IEndDragHandler
    {
        [Header("Option")]

        [Range(1, 10)]
        public int visibleCount;
        public float padding = 40;

        [Header("Page Indexer Color")]
        public Color enabledColor;
        public Color disabledColor;

        [HideInInspector]
        public int pageIndex;

        [SerializeField]
        public GameObject pageIndexer;

        [SerializeField]
        public Transform pageIndexerParent;

        private float _itemSize;
        private int _pageCount;
        private float[] _pivots;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private RectTransform _rect;

        [SerializeField]
        private RectTransform _content;

        public void forceUpdate()
        {
            _getItemSize();
            _organize();
            _calculate_pageCount();
            _initialize_pageIndexer();
        }

        public void forceUpdateAtEditor()
        {
            _getItemSize();
            _organize();
            _calculate_pageCount();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(_snap());
        }

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _rect = GetComponent<RectTransform>();
            _content = _scrollRect.content;
        }

        private void Start()
        {
            forceUpdate();
        }

        private void _calculate_pageCount()
        {
            _pageCount = Mathf.FloorToInt((_content.childCount - 1) / visibleCount) + 1;
        }

        private void _getItemSize()
        {
            if(_content.childCount > 0)
                _itemSize = _content.GetChild(0).GetComponent<RectTransform>().rect.width;
        }

        private void _organize()
        {
            float pivot = _itemSize * 0.5f + padding;
            float width = _rect.rect.width;

            float distance 
                = (width - (_itemSize * visibleCount) - padding * 2) / (visibleCount - 1) + _itemSize;

            if (visibleCount == 1)
            {
                for (int i = 0; i < _content.childCount; i++)
                {
                    _content.GetChild(i).localPosition = new Vector2(pivot + (width * i), 0);
                }
            }

            else
            {
                for (int i = 0; i < _content.childCount; i++)
                {
                    _content.GetChild(i).localPosition
                        = new Vector2(pivot + (distance * (i % visibleCount)), 0);

                    if ((i + 1) % visibleCount == 0) pivot += width;
                }
            } 
        }

        private void _pageCheck()
        {
            _pivots = new float[_pageCount];

            for (int i = 0; i < _pivots.Length; i++)
            {
                _pivots[i] = Mathf.Abs(_content.anchoredPosition.x + (_rect.rect.width * i));
            }

            pageIndex = Array.IndexOf(_pivots, Mathf.Min(_pivots));

            _setPageColor();
        }

        private IEnumerator _snap()
        {
            yield return new WaitForSeconds(0.25f);

            _pageCheck();
            _setPageColor();

            var target = _rect.rect.width * pageIndex;

            _scrollRect.StopMovement();

            while (Mathf.Abs(_content.anchoredPosition.x - target) > 1)
            {
                _content.anchoredPosition 
                    = new Vector2
                    (
                        Mathf.Lerp(_content.anchoredPosition.x, -target, Time.deltaTime * 10f), 
                        0
                    );

                yield return null;
            }
        }

        private void _initialize_pageIndexer()
        {
            for (int i = 1; i < pageIndexerParent.childCount; i++)
            {
                Destroy(pageIndexerParent.GetChild(i).gameObject);
            }

            for (int i = 1; i < _pageCount; i++)
            {
                Instantiate(pageIndexer, pageIndexerParent);
            }

            if (pageIndex > _pageCount - 1) pageIndex = _pageCount - 1;

            _setPageColor();
        }

        private void _setPageColor()
        {
            for (int i = 0; i < pageIndexerParent.childCount; i++)
            {
                if(i == pageIndex)
                {
                    pageIndexerParent.GetChild(i).GetComponent<Image>().color = enabledColor;
                }

                else
                {
                    pageIndexerParent.GetChild(i).GetComponent<Image>().color = disabledColor;
                }
            }
        }
    }
}