using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{

    [SerializeField]
    private Image foregroundImage;

    private float updateSpeed = 0.5f;

    void Awake()
    {
        GetComponentInParent<MinionController>().OnHealthPercentChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float percent)
    {
        StartCoroutine(ChangeToPercent(percent));
    }

    private IEnumerator ChangeToPercent(float percent)
    {
        float preChangePercent = foregroundImage.fillAmount;
        float elapsed = 0f;

        while(elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePercent, percent, elapsed / updateSpeed);
            yield return null;
        }

        foregroundImage.fillAmount = percent;
    }

}
