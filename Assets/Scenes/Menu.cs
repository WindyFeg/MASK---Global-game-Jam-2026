using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;  // NEW Input System

public class Menu : MonoBehaviour
{
    public GameObject tipContainer;
    public Button tipInteractBtn;
    public bool isShowTip = true;

    private bool started = false;

    private void Start()
    {
        if (tipContainer != null) tipContainer.SetActive(false);

        if (tipInteractBtn != null)
        {
            tipInteractBtn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }

    private void Update()
    {
        if (started) return;

        if (AnyStartInputThisFrame())
        {
            started = true;
            gameObject.SetActive(false);
        }
    }

    private bool AnyStartInputThisFrame()
    {
        // Keyboard: any key
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        // Mouse: click
        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
            return true;

        // Touch: tap (mobile)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        // Gamepad: vài nút phổ biến
        if (Gamepad.current != null &&
            (Gamepad.current.startButton.wasPressedThisFrame ||
             Gamepad.current.buttonSouth.wasPressedThisFrame ||
             Gamepad.current.buttonEast.wasPressedThisFrame))
            return true;

        return false;
    }

    private void ShowTipContainer()
    {
        // if (!isShowTip)
        // {
        //     gameObject.SetActive(false);
        //     return;
        // }

        // if (tipContainer != null) tipContainer.SetActive(true);
    }
}
