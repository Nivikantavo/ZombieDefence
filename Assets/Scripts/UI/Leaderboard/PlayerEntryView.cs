using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text _rank;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _nickname;
    [SerializeField] private Image _background;

    public void Initialize(int rank, int score, string nickname, Sprite background)
    {
        _rank.text = rank.ToString();
        SetRecordInFormat((float)score, _score);
        _background.sprite = background;
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Anonymous";
        }

        _nickname.text = nickname;
    }

    private void SetRecordInFormat(float time, TMP_Text text)
    {
        float[] timersValue = new float[]
        {
            Mathf.FloorToInt(time / 60),
            Mathf.FloorToInt(time % 60),
            Mathf.FloorToInt((time * 1000) % 100)
        };

        text.text = string.Format("{00:00}:{1:00}:{2:00}", timersValue[0], timersValue[1], timersValue[2]);
    }
}
