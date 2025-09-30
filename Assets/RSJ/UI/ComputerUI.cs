using UnityEngine;

public class ComputerUI : MonoBehaviour
{
    [SerializeField] private GameObject WindowPan;
    [SerializeField] private GameObject MessageTitle;
    [SerializeField] private GameObject InMessageTitle;
    [SerializeField] private DaySO MessageContent;

    public void TurnOnMessage()
    {
        //여기서 내용 설정
        MessageCreating();
        WindowPan.SetActive(true);
    }

    public void TurnOffMessage()
    {
        // 여기서 내용 안보이게 하기
        WindowPan.SetActive(false);
    }

    public void TurnOnWnd(GameObject pan)
    {
        pan.SetActive(true);
    }

    public void TurnOffWnd(GameObject pan)
    {
        pan.SetActive(false);
    }

    public void SetMessage(int number)
    {
        switch(number)
        {
            case 1:
                {

                }
                break;
            case 2:
                {

                }
                break;
            case 3:
                {

                }
                break;
        }
    }

    private void MessageCreating()
    {

    }
}
