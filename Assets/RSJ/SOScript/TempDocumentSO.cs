using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TempDocumentSO", menuName = "Scriptable Objects/TempDocumentSO", order = 0)]
public class TempDoccumentSO : ScriptableObject
{
    public University universityEnum;
    public Region RegionEnum;
    public ErrorType _errorType;
    public List<Sprite> _imageList;

    public bool acceptUnfair;
    public bool falureUnfair;
    public string region;
    public string univ;
    public string infoName;
    public string Sexuality;
    public string Call;
    public string Major;
    public int Grade;
    public int Graduation;
    public string prevCom;
    public string Job;
    public int Period;

    public void OnValidate()
    {
        switch (universityEnum)
        {
            case University.VNU:
                univ = "VNU";
                break;
            case University.CIT:
                univ = "CIT";
                break;
            case University.OCC:
                univ = "OCC";
                break;
            case University.CCC:
                univ = "CCC";
                break;
        }

        switch (RegionEnum)
        {
            case Region.EMA:
                region = "EMA";
                break;
            case Region.CIZ:
                region = "CIZ";
                break;
            case Region.LP:
                region = "LP";
                break;
            case Region.PSU:
                region = "PSU";
                break;
            case Region.EPC:
                region = "EPC";
                break;
        }
    }
}
