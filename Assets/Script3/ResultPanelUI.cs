using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel kết quả: Game Over / Win. Chứa panel, TextMeshPro (lý do thắng/thua), nút Replay.
/// Replay reset game trong cùng scene (không load scene).
/// </summary>
public class ResultPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button replayButton;

    private void Awake()
    {
        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayClicked);
        Hide();
    }

    public void ShowGameOver(string reason)
    {
        if (messageText != null)
            messageText.text = "GAME OVER\n\n" + reason;
        if (panel != null)
            panel.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void ShowWin(string reason)
    {
        if (messageText != null)
            messageText.text = "YOU WIN\n\n" + reason;
        if (panel != null)
            panel.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void OnReplayClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.ResetGame();
        else
            Hide();
    }
}
