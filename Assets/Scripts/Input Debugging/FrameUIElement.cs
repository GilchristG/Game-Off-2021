using UnityEngine;
using UnityEngine.UI;

public class FrameUIElement : MonoBehaviour
{
    public Image direction;
    public Image lightBtn;
    public Image mediumBtn;
    public Image heavyBtn;

    public Sprite[] elementIcons;


    public void Setup(InputFrame frame)
    {
        direction.sprite = elementIcons[frame.inputs[0]];

        lightBtn.gameObject.SetActive(frame.inputs[1] == 1);
        mediumBtn.gameObject.SetActive(frame.inputs[2] == 1);
        heavyBtn.gameObject.SetActive(frame.inputs[3] == 1);
    }
}
