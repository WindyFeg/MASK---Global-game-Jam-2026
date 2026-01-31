using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject tipContainer;
    public Button startBtn;
    public Button tipInteractBtn;
    public bool isShowTip = true;

    private void Start()
    {
        tipContainer.SetActive(false);
        if (tipInteractBtn != null)
        {
            tipInteractBtn.onClick.AddListener(() =>
            {
                // Hide the menu
                gameObject.SetActive(false);
            });
        }
        startBtn.onClick.AddListener(() =>
        {
            ShowTipContainer();
        });
    }

    private void ShowTipContainer()
    {
        if (!isShowTip)
        {
            this.gameObject.SetActive(false);
        }
        tipContainer.SetActive(true);
    }

}