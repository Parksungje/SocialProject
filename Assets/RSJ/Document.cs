using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Document : MonoBehaviour
{
    DaySO today;

    int allotment = 0;

    [SerializeField] private ContainerScript _container;
    [SerializeField] private GameStatusSO stat;

    #region 정보 담은 거
    [SerializeField] private List<Button> checkButton;

    [SerializeField] private TextMeshProUGUI naming;
    [SerializeField] private TextMeshProUGUI sexuality;
    [SerializeField] private TextMeshProUGUI Univ;
    [SerializeField] private TextMeshProUGUI Major;
    [SerializeField] private TextMeshProUGUI Grade;
    [SerializeField] private TextMeshProUGUI Gradu;
    [SerializeField] private TextMeshProUGUI prev;
    [SerializeField] private TextMeshProUGUI Job;
    [SerializeField] private TextMeshProUGUI Period;
    [SerializeField] private TextMeshProUGUI Call;
    #endregion

    public void Awake()
    {
        _container._dayList[stat.day] = today;
    }

    public void DocumentInit()
    {
        if(today != null)
        {
            naming.text = today.DoccumentInfo[allotment].infoName;
            sexuality.text = today.DoccumentInfo[allotment].Sexuality;
            Univ.text = today.DoccumentInfo[allotment].univ;
            Major.text = today.DoccumentInfo[allotment].Major;
            Grade.text = today.DoccumentInfo[allotment].Grade.ToString();
            Gradu.text = today.DoccumentInfo[allotment].Graduation.ToString();
            prev.text = today.DoccumentInfo[allotment].prevCom;
            Job.text = today.DoccumentInfo[allotment].Job;
            Period.text = today.DoccumentInfo[allotment].Period.ToString();
            Call.text = today.DoccumentInfo[allotment].Call;
        }
    }

    public void CheckSus(int a)
    {
        checkButton[a].TryGetComponent(out TextMeshProUGUI meshpro);
        meshpro.color = Color.red;
    }


}
