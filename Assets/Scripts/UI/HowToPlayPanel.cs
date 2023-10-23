using Agava.WebUtility;
using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayPanel : MonoBehaviour
{
    [SerializeField] private List<GameObject> PCInputImages;
    [SerializeField] private List<GameObject> MobileInputImages;

    private void Start()
    {
        if (Device.IsMobile)
        {
            foreach (var image in MobileInputImages)
            {
                image.SetActive(true);
            }
            foreach (var image in PCInputImages)
            {
                image.SetActive(false);
            }
        }
    }
}
