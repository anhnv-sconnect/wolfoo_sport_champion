using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SCN.HUD
{
    public class RippleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public bool invokeOnce = false;
        public float activeScale = 1.2f;
        public bool interactable = true;
        public bool playSoundClick = true;
        public UnityEvent onClick;

        bool invoked = false;
        const float ZoomOutTime = 0.1f;
        const float ZoomInTime = 0.1f;

        Vector3 baseScale = new Vector3(1.0f, 1.0f, 1.0f);

        void Start()
        {
            baseScale = transform.localScale;
        }

        void OnEnable()
        {
            ResetInvokeState();
        }

        public void ResetInvokeState()
        {
            invoked = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable)
            {
                StartCoroutine("StartClick");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopCoroutine("StartClick");
            if (interactable)
            {
                transform.localScale = baseScale;
            }
        }

        public void OnPointerClick(PointerEventData eventdata)
        {
            if (interactable && (!invokeOnce || !invoked))
            {
                if (playSoundClick)
                {

                }
                onClick.Invoke();
                invoked = true;
            }
        }

        IEnumerator StartClick()
        {
            float tCounter = 0;

            while (tCounter < ZoomOutTime)
            {
                tCounter += Time.deltaTime;
                transform.localScale = Vector3.Lerp(baseScale, baseScale * activeScale, tCounter / ZoomOutTime);
                yield return null;
            }
        }

        IEnumerator StartExit()
        {
            float tCounter = 0;

            while (tCounter < ZoomInTime)
            {
                tCounter += Time.deltaTime;
                transform.localScale = Vector3.Lerp(baseScale * activeScale, baseScale, tCounter / ZoomInTime);
                yield return null;
            }
        }

    }
}