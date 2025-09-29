using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIMove : MonoBehaviour
{
    [Range(0, 1)] private int targetPosnum;
    [SerializeField] private Image obj;
    [SerializeField] private List<Transform> targetPostion;
    private bool uiFixed = false;

    public void MoveUI()
    {
        if (!uiFixed)
        {
            if (targetPosnum == 0)
                targetPosnum++;
            else
                targetPosnum--;

            obj.rectTransform.DOMove(targetPostion[targetPosnum].position, 0.4f).SetEase(Ease.OutBack);
        }
    }

    public void SetFixed()
    {
        uiFixed = true;
    }

    public void UnFixed()
    {
        uiFixed = false;
    }

    public void AutoMoveUI(Image Img)
        => Img.rectTransform.DOMove(targetPostion[targetPosnum].position, 0.4f).SetEase(Ease.OutBack);
}
