using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStatusSO", menuName = "Scriptable Objects/GameStatusSO", order = -1)]
public class GameStatusSO : ScriptableObject
{
    public int day;
    public List<int> maxAlloment;

    public int loyalty;
    public int conscience;

    public int playerMoney;
}
