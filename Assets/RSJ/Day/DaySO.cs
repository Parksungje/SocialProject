using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaySO", menuName = "SO/DaySO", order = 1)]

public class DaySO : ScriptableObject
{
    public List<InfoSO> CurrectInfo;
    public List<TempDoccumentSO> DoccumentInfo;

    public int day = 0;
    public int Correct = 0;
    public int incorrect = 0;
}
