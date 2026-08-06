using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProductView : MonoBehaviour
{
    public string ProductID => _product?.id;
    public string ProductName => _item.Name;

    [SerializeField] private RawImage _productImage;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _priceLable;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Item _item;

    private CatalogProduct _product;
    private bool _bought = false;
    private Coroutine _downloadImageCoroutine;

    public event UnityAction<Item, string> PoductViewClick;

    public CatalogProduct Product
    {
        set
        {
            _product = value;
            if (_product == null)
                return;

            _priceText.text = string.IsNullOrEmpty(_product.priceValue) == false
                ? _product.priceValue
                : _product.price;

            if (string.IsNullOrEmpty(value.imageURI) == false
                && Uri.IsWellFormedUriString(value.imageURI, UriKind.Absolute))
            {
                if (_downloadImageCoroutine != null)
                    StopCoroutine(_downloadImageCoroutine);

                _downloadImageCoroutine = StartCoroutine(DownloadAndSetProductImage(value.imageURI));
            }
        }
    }

    private void OnEnable()
    {
        _sellButton.onClick.AddListener(OnPurchaseButtonClick);
    }

    private void OnDisable()
    {
        _sellButton.onClick.RemoveListener(OnPurchaseButtonClick);
    }

    private IEnumerator DownloadAndSetProductImage(string imageUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                _productImage.texture = DownloadHandlerTexture.GetContent(request);
        }

        _downloadImageCoroutine = null;
    }

    public void OnPurchaseButtonClick()
    {
        if(_bought == false && string.IsNullOrEmpty(ProductID) == false)
        {
            PoductViewClick?.Invoke(_item, ProductID);
        }
    }

    public void OnSellSuccessfully()
    {
        _bought = true;
        Renderer();
    }

    public void Renderer()
    {
        _priceLable.SetActive(!_bought);
    }
}
