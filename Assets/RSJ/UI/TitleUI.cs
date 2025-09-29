using DG.Tweening;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject settingPan;
    private bool active = false;

    public void ChangeScene()
    {

    }

    public void ExitGame() => Application.Quit();

    public void ChangeSettingsActive()
    {
        active = !active;
        settingPan.SetActive(active);
    }
}
