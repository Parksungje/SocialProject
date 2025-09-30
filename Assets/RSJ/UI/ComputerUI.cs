using UnityEngine;

public class ComputerUI : MonoBehaviour
{
    [SerializeField] private GameObject WindowPan;
    [SerializeField] private GameObject MessageTitle;
    [SerializeField] private GameObject InMessageTitle;
    [SerializeField] private DaySO MessageContent;

    public void TurnOnMessage()
    {
        //���⼭ ���� ����
        MessageCreating();
        WindowPan.SetActive(true);
    }

    public void TurnOffMessage()
    {
        // ���⼭ ���� �Ⱥ��̰� �ϱ�
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
