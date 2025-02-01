using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("Death Pop Up")]
    [SerializeField] GameObject deathPopUpGameObject;
    [SerializeField] CanvasGroup deathPopUpCanvasGroup;

    public void SendDeathPopUp()
    {
        deathPopUpGameObject.SetActive(true);
        StartCoroutine(FadeOutPopUpOverTime(deathPopUpCanvasGroup, 5, 1));
    }

    private IEnumerator FadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {
            canvas.alpha = 1;

            Debug.Log("Timer : " + duration + " Delay : " + delay);
            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }

            float timer = 0;

            Debug.Log("Timer : " + timer + " Delay : " + delay);

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);

                yield return null;
            }

            Debug.Log("Timer : " + timer + " Delay : " + delay);
        }

        canvas.alpha = 0;
        
        yield return null;
    }
}
