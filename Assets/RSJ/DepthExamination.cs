using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DepthExamination : MonoBehaviour
{
    [SerializeField] private GameObject _checkIcon;
    [SerializeField] private TempDoccumentSO _doccument;
    [SerializeField] private bool checkDocument;
    [SerializeField]private ErrorType _type;
    public int correct;
    public int incorrect;

    public void SetCheckDisplay(GameObject chk)
    {
        if (_checkIcon == null)
        {
            _checkIcon = chk;
            _checkIcon.SetActive(true);
        }
        else
        {
            _checkIcon.SetActive(false);
            chk.SetActive(true);
            _checkIcon = chk;
        }
    }

    public void CheckAnswer()
    {
        if(_doccument._errorType == _type)
        {
            correct++;
            Debug.Log("薑港蹺");
        }
        else
        {
            incorrect++;
            Debug.Log("雄 六六六六");
        }
    }

    public void SetTypeofError(int num)
    {
        switch (num)
        {
            case 1:
                _type = ErrorType.None;
                break;
            case 2:
                _type = ErrorType.Incorrect;
                break;
            case 3:
                _type = ErrorType.typist;
                break;
            case 4:
                _type = ErrorType.counterfeit;
                break;
            case 5:
                _type = ErrorType.Discrimination;
                break;
        }
    }

    public void Highlighting(TextMeshProUGUI text)
    {
        if (text.color == Color.red)
            text.color = Color.black;
        else
            text.color = Color.red;
    }

    public void SetDocument()
    {
        _doccument = Document.instance.tempDoccument;
    }
}
