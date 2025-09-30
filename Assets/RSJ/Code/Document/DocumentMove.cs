using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DocumentMove : MonoBehaviour
{
    [SerializeField] private GameObject folder;
    [SerializeField] private Transform targetPos;
    [SerializeField] private Transform originPos;
    [SerializeField] private GameStatusSO day;
    [SerializeField] private GameObject btn;

    private void Awake()
    {
        folder.transform.DOMove(targetPos.position, 1f).SetEase(Ease.OutSine);
    }

    private void Update()
    {
        if(day.maxAlloment[day.day] - 1 <= Document.instance.allotment)
            btn.SetActive(false);

        if (day.maxAlloment[day.day] <= Document.instance.allotment)
        {
            //���⼭ ȭ����ȯ �ϰ� ����
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Document"))
        {
            btn.SetActive(true);
            Document.instance.allotment++;
            Destroy(collider.gameObject);
        }
    }

    public void CallNextDocu()
    {
        btn.SetActive(false);
        folder.transform.DOMove(targetPos.position, 1f).SetEase(Ease.OutSine);
        Document.instance.allotment++;
    }

    public void RollbackFolderPos()
    {
        folder.transform.position = originPos.position;
    }

    public void OnCallND()
    {
        btn.SetActive(true);
        //Document.instance.allotment++;
    }
}
