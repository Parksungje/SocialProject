using UnityEngine;
using UnityEngine.UI;

public class CheckDocument : MonoBehaviour
{
    [SerializeField] private Button accpetButton;
    [SerializeField] private Button refuseButton;


    private InfoSO _infomation;
    private TempDoccumentSO _doccument;

    public void SetDoccument(TempDoccumentSO doccument)
    {
        _doccument = doccument;
    }

    public void CompareDocumment()
    {

    }
}
