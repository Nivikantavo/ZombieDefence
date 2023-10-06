using UnityEngine;

public class ImproveItem : Item
{
    public int ImproveStep => _improveStep;

    [SerializeField] private int _improveStep;
}
