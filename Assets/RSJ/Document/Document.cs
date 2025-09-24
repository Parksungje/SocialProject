using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class Document : MonoBehaviour
{
    [SerializeField] private List<DaySO> today;
    public TempDoccumentSO tempDoccument;
    [SerializeField] private GameStatusSO stat;
    [SerializeField] private List<GameObject> _imageList;

    public int allotment = 0;

    public static Document instance;

    private void OnEnable()
    {
        if(instance == null)
            instance = this;
    }

    #region 정보 담은 거
    [SerializeField] private TextMeshProUGUI naming;
    [SerializeField] private TextMeshProUGUI Address;
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

    public void DocumentInit()
    {
        if(today != null)
        {
            naming.text = today[stat.day].DoccumentInfo[allotment].infoName;
            sexuality.text = today[stat.day].DoccumentInfo[allotment].Sexuality;
            Univ.text = today[stat.day].DoccumentInfo[allotment].univ;
            Major.text = today[stat.day].DoccumentInfo[allotment].Major;
            Grade.text = today[stat.day].DoccumentInfo[allotment].Grade.ToString();
            Gradu.text = today[stat.day].DoccumentInfo[allotment].Graduation.ToString();
            prev.text = today[stat.day].DoccumentInfo[allotment].prevCom;
            Job.text = today[stat.day].DoccumentInfo[allotment].Job;
            Period.text = today[stat.day].DoccumentInfo[allotment].Period.ToString();
            Call.text = today[stat.day].DoccumentInfo[allotment].Call;

            tempDoccument = today[stat.day].DoccumentInfo[allotment];
        }

        SetImage();
    }

    private void SetImage()
    {
        Debug.Assert(tempDoccument != null, "tempDoccumnt didn't set");

        int a = 0;
        foreach (var item in _imageList)
        {
            item.SetActive(false);
            item.GetComponent<Image>().sprite = tempDoccument._imageList[a].sprite;
            a++;
        }
    }

    public void RandomMoveImage()
    {
        float x = Random.Range(40f, 200f);
        float y = Random.Range(40f, 200f);

        foreach(GameObject item in _imageList)
        {
            item.SetActive(true);
            item.transform.DOMove(new Vector3(x, y), 0.5f).SetEase(Ease.OutSine);
        }
    }

    public void RollbackImagePos()
    {
        foreach (GameObject item in _imageList)
        {
            item.SetActive(false);
            item.transform.position = new Vector3(0, 0, 0);
        }
    }
}
