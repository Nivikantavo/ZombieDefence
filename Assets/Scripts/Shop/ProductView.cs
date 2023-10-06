using Agava.YandexGames;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProductView : MonoBehaviour
{
    public string ProductID => _product.id;

    [SerializeField] private RawImage _productImage;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _priceLable;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Item _item;

    private CatalogProduct _product;
    private bool _bought = false;

    public event UnityAction<Item, string> PoductViewClick;

    public CatalogProduct Product
    {
        set
        {
            _product = value;
            _priceText.text = _product.priceValue.ToString();
            if (Uri.IsWellFormedUriString(value.imageURI, UriKind.Absolute))
                StartCoroutine(DownloadAndSetProductImage(value.imageURI));
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
        var remoteImage = new RemoteImage(imageUrl);
        remoteImage.Download();

        while (!remoteImage.IsDownloadFinished)
            yield return null;

        if (remoteImage.IsDownloadSuccessful)
            _productImage.texture = remoteImage.Texture;
    }

    public void OnPurchaseButtonClick()
    {
        if(_bought == false)
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
