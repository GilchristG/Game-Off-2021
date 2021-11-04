using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider leftBar;
    public Slider rightBar;

    public Slider leftHighlight;
    public Slider rightHighlight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateHealthBars(uint characterNum, float newHealth)
    {
        if (characterNum == 1)
        {
            leftBar.value = newHealth / Constants.MAX_HEALTH;
        }
        else if (characterNum == 2)
        {
            rightBar.value = newHealth / Constants.MAX_HEALTH;
        }

        StartCoroutine(updateDelayedHighlight(characterNum, newHealth));
    }

    private IEnumerator updateDelayedHighlight(uint characterNum, float newHealth)
    {
        yield return new WaitForSeconds(0.25f);

        if (characterNum == 1)
        {
            leftHighlight.value = newHealth / Constants.MAX_HEALTH;
        }
        else if (characterNum == 2)
        {
            rightHighlight.value = newHealth / Constants.MAX_HEALTH;
        }

        yield return 0;
    }
}
