using UnityEngine;

public class Fuse : MonoBehaviour
{
    [SerializeField] private Bomb _bomb;

    private void OnEnable()
    {
        _bomb.gameObject.SetActive(true);
    }

    public void BlowYoureselfUp()
    {
        _bomb.BlowUp();
    }
}
