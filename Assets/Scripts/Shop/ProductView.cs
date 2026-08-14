using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProductView : MonoBehaviour
{
    public string BaseProductId => _baseProductId;
    public string ProductID => _offeredProductId;
    public string ProductName => _item != null ? _item.Name : _baseProductId;

    [SerializeField] private string _baseProductId;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _priceLable;
    [SerializeField] private Image _currencyImage;
    [SerializeField] private TMP_Text _rewardAmountText;
    [SerializeField] private Image _upgradeImage;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Item _item;

    private CatalogProduct _product;
    private string _offeredProductId;
    private bool _soldOut;
    private bool _consumable;
    private Coroutine _downloadCurrencyIconCoroutine;
    private IapPurchaseService _iap;

    private static readonly Dictionary<string, Sprite> CurrencyIconSprites = new Dictionary<string, Sprite>();

    public event UnityAction<Item, string> PoductViewClick;

    private void Awake()
    {
        ItemView itemView = GetComponent<ItemView>();
        if (itemView != null)
            itemView.enabled = false;

        if (_currencyImage == null)
            _currencyImage = FindCurrencyImage();

        if (_rewardAmountText == null)
            _rewardAmountText = FindRewardAmountText();
    }

    private void OnEnable()
    {
        _sellButton.onClick.AddListener(OnPurchaseButtonClick);
        BindService();
        Refresh();
    }

    private void OnDisable()
    {
        _sellButton.onClick.RemoveListener(OnPurchaseButtonClick);
        UnbindService();
    }

    public void SetIapService(IapPurchaseService iap)
    {
        UnbindService();
        _iap = iap;
        BindService();
        Refresh();
    }

    public void Configure(string baseProductId)
    {
        _baseProductId = baseProductId;
        Refresh();
    }

    public void Refresh()
    {
        if (_iap == null && SaveSystem.Instance != null)
            _iap = SaveSystem.Instance.IapPurchases;

        IapCatalog catalog = _iap != null ? _iap.Catalog : null;
        IapProductDefinition definition = catalog != null ? catalog.GetById(_baseProductId) : null;
        _consumable = definition != null && definition.IsConsumable;
        _offeredProductId = _iap != null ? _iap.GetOfferedProductId(_baseProductId) : _baseProductId;
        IapProductDefinition offeredDefinition = catalog != null ? catalog.GetById(_offeredProductId) : definition;
        _soldOut = _consumable == false && string.IsNullOrEmpty(_offeredProductId);

        ApplyRewardAmount(offeredDefinition ?? definition);

        if (string.IsNullOrEmpty(_offeredProductId) == false
            && _iap != null
            && _iap.TryGetCatalogProduct(_offeredProductId, out CatalogProduct catalogProduct))
        {
            ApplyCatalogProduct(catalogProduct, offeredDefinition);
        }
        else
        {
            _product = null;
            if (_priceText != null)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                _priceText.text = string.Empty;
#else
                _priceText.text = offeredDefinition != null ? offeredDefinition.EditorPriceLabel : string.Empty;
#endif
            }

            SetCurrencyIconVisible(true);
        }

        Render();
    }

    private void ApplyCatalogProduct(CatalogProduct product, IapProductDefinition definition)
    {
        _product = product;
        if (_product == null)
            return;

        bool useGam = PlaygamaPaymentsGateway.IsPlaygamaPlatform || product.LooksLikeUsd();
        if (_priceText != null)
        {
            _priceText.enableWordWrapping = useGam == false;
            _priceText.overflowMode = TextOverflowModes.Overflow;
            _priceText.text = useGam ? product.ToGamPriceLabel() : FormatPriceLabel(product);
        }

        SetCurrencyIconVisible(useGam == false);
        if (useGam)
        {
            if (_downloadCurrencyIconCoroutine != null)
            {
                StopCoroutine(_downloadCurrencyIconCoroutine);
                _downloadCurrencyIconCoroutine = null;
            }
        }
        else
        {
            string currencyIconUrl = product.priceCurrencyImage;
            if (string.IsNullOrEmpty(currencyIconUrl) && _iap != null)
                currencyIconUrl = _iap.CurrencyIconUrl;

            ApplyCurrencyIcon(currencyIconUrl);
        }

        ApplyRewardAmount(definition);
    }

    private void ApplyRewardAmount(IapProductDefinition definition)
    {
        if (_rewardAmountText == null)
            _rewardAmountText = FindRewardAmountText();

        if (_rewardAmountText == null)
            return;

        bool showAmount = definition != null && definition.IsConsumable && definition.CoinAmount > 0;
        _rewardAmountText.gameObject.SetActive(showAmount);
        if (showAmount)
            _rewardAmountText.text = definition.CoinAmount.ToString();
    }

    private void ApplyCurrencyIcon(string imageUrl)
    {
        if (_currencyImage == null)
            _currencyImage = FindCurrencyImage();

        if (_currencyImage == null)
            return;

        imageUrl = NormalizeImageUrl(imageUrl);
        if (string.IsNullOrEmpty(imageUrl))
            return;

        if (CurrencyIconSprites.TryGetValue(imageUrl, out Sprite cached) && cached != null)
        {
            _currencyImage.sprite = cached;
            _currencyImage.preserveAspect = true;
            return;
        }

        if (_downloadCurrencyIconCoroutine != null)
            StopCoroutine(_downloadCurrencyIconCoroutine);

        _downloadCurrencyIconCoroutine = StartCoroutine(DownloadAndSetCurrencyIcon(imageUrl));
    }

    private IEnumerator DownloadAndSetCurrencyIcon(string imageUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && _currencyImage != null)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                CurrencyIconSprites[imageUrl] = sprite;
                _currencyImage.sprite = sprite;
                _currencyImage.preserveAspect = true;
            }
        }

        _downloadCurrencyIconCoroutine = null;
    }

    private void SetCurrencyIconVisible(bool visible)
    {
        if (_currencyImage == null)
            _currencyImage = FindCurrencyImage();

        if (_currencyImage != null)
            _currencyImage.gameObject.SetActive(visible);
    }

    private static string FormatPriceLabel(CatalogProduct product)
    {
        if (product == null)
            return string.Empty;

        if (string.IsNullOrEmpty(product.priceValue) == false)
            return product.priceValue;

        return product.price ?? string.Empty;
    }

    private Image FindCurrencyImage()
    {
        Image[] images = GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null)
                continue;

            string imageName = images[i].gameObject.name;
            if (imageName == "CurrencyIcon" || imageName.StartsWith("Yan"))
                return images[i];
        }

        return null;
    }

    private TMP_Text FindRewardAmountText()
    {
        TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] != null && labels[i].gameObject.name.StartsWith("AmountText"))
                return labels[i];
        }

        return null;
    }

    private static string NormalizeImageUrl(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            return imageUrl;

        if (imageUrl.StartsWith("//"))
            return "https:" + imageUrl;

        return null;
    }

    private void OnPurchaseButtonClick()
    {
        if (_soldOut == false && string.IsNullOrEmpty(ProductID) == false)
            PoductViewClick?.Invoke(_item, ProductID);
    }

    private void Render()
    {
        if (_priceLable != null)
        {
            bool showPrice = _soldOut == false;
#if UNITY_WEBGL && !UNITY_EDITOR
            showPrice = showPrice && _product != null;
#endif
            _priceLable.SetActive(showPrice);
        }

        if (_upgradeImage != null)
        {
            bool showUpgrade = _soldOut == false
                && _consumable == false
                && string.IsNullOrEmpty(_offeredProductId) == false
                && _offeredProductId != _baseProductId;
            _upgradeImage.gameObject.SetActive(showUpgrade);
        }
    }

    private void BindService()
    {
        if (_iap == null && SaveSystem.Instance != null)
            _iap = SaveSystem.Instance.IapPurchases;

        if (_iap == null)
            return;

        _iap.StateChanged -= Refresh;
        _iap.StateChanged += Refresh;
    }

    private void UnbindService()
    {
        if (_iap != null)
            _iap.StateChanged -= Refresh;
    }
}
