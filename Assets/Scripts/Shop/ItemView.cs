using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    public string ItemName => _item.Name;

    [SerializeField] private Sprite _background;
    [SerializeField] private Sprite _itemIcon;

    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _upgradeImage;
    [SerializeField] private TMP_Text _priceText;

    [SerializeField] private GameObject _priceLable;
    [SerializeField] private Item _item;
    [SerializeField] private Button _sellButton;

    public event UnityAction<Item> ViewClick;

    private void OnEnable()
    {
        Render();
        _sellButton.onClick.AddListener(OnSellButtonClick);
        _item.ItemBought += OnItemBought;
    }

    private void OnDisable()
    {
        _sellButton.onClick.RemoveListener(OnSellButtonClick);
        _item.ItemBought -= OnItemBought;
    }

    public void MarkItemAsBought(int boughtCount)
    {
        _item.MarkAsBought(boughtCount);
    }

    private void OnSellButtonClick()
    {
        ViewClick?.Invoke(_item);
    }
    
    private void Render()
    {
        _backgroundImage.sprite = _background;
        _itemImage.sprite = _itemIcon;
        _priceText.text = _item.SellingPrice.ToString();

        if (_item.Purchases >= _item.NumberOfItems)
        {
            _priceLable.SetActive(false);
        }

        if(_upgradeImage != null)
        {
            if (_item.Purchases > 0 && _item.Purchases < _item.NumberOfItems)
            {
                _upgradeImage.gameObject.SetActive(true);
            }
            else
            {
                _upgradeImage.gameObject.SetActive(false);
            }
        }
    }

    private void OnItemBought()
    {
        Render();
    }
}
