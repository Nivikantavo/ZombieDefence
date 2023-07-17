using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage
{
    public int LevelsCount => _levelsCount;
    public int CurrentLevelNumber => _currentLevelNumber;
    public int Number => _number;

    [SerializeField] private int _levelsCount;
    [SerializeField] private int _currentLevelNumber;
    [SerializeField] private int _number;
}
