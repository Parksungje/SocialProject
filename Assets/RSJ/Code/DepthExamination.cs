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
    [SerializeField] private ErrorType _type;
    [SerializeField] private GameStatusSO _stat;
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

    public void HideCheckDisplay()
    {
        _checkIcon?.SetActive(false);
    }

    public void CheckAnswer()
    {
        if(_doccument._errorType == _type && _doccument._errorType == ErrorType.None ||
            (_doccument.acceptUnfair && _doccument._errorType != ErrorType.counterfeit) ||
            (_doccument.falureUnfair && _doccument._errorType == ErrorType.None))
        {
            correct++;
            if(_doccument.acceptUnfair)
            {
                //_stat.loyalty += 니 원하는 만큼;
                //_stat.conscience -= 니 원하는 만큼;
            }
            else if (_doccument.falureUnfair)
            {
                //_stat.loyalty += 니 원하는 만큼;
                //_stat.conscience -= 니 원하는 만큼;
            }
            Debug.Log("정답추");
        }
        else
        {
            incorrect++;
            Debug.Log("땡 ㅋㅋㅋㅋ");
        }
    }

    public void CheckWrong()
    {
        if (_doccument._errorType == _type && _doccument._errorType != ErrorType.None || _doccument.falureUnfair )
        {
            correct++;
            if (_doccument.acceptUnfair)
            {
                //_stat.loyalty -= 니 원하는 만큼;
                //_stat.conscience += 니 원하는 만큼;
            }
            else if (_doccument.falureUnfair)
            {
                //_stat.loyalty += 니 원하는 만큼;
                //_stat.conscience -= 니 원하는 만큼;
            }
            Debug.Log("정답추");
        }
        else
        {
            incorrect++;
            Debug.Log("땡 ㅋㅋㅋㅋ");
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
