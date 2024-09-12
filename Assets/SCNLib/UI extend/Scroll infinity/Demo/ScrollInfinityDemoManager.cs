using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using SCN.ActionLib;
using SCN.Common;
using DG.Tweening;

namespace SCN.UIExtend
{
	public class ScrollInfinityDemoManager : MonoBehaviour
	{
		[SerializeField] Canvas canvas;
		[SerializeField] VerticalScrollInfinity verScrollInfinity;
		[SerializeField] HorizontalScrollInfinity horScrollInfinity;

		[SerializeField] RectTransform[] detectArea;
		int[] count = new int[3];

		bool isDragIn;

		private void Awake()
		{
			detectArea[0].GetComponent<Image>().color = Color.red;
			detectArea[1].GetComponent<Image>().color = Color.green;
			detectArea[2].GetComponent<Image>().color = Color.blue;

            //var rectDetect = new RectDetection[3];
            //rectDetect[0] = new RectDetection(detectArea[0], canvas.transform.localScale.x);
            //rectDetect[1] = new RectDetection(detectArea[1], canvas.transform.localScale.x);
            //rectDetect[2] = new RectDetection(detectArea[2], canvas.transform.localScale.x);

            //ScrollInfinityDemoItem.OnPointerUpAfterDrag = item =>
            //{
            //    var temp = item.Order % 3;
            //    if (rectDetect[temp].CheckIn(item.RectTrans))
            //    {
            //        count[temp]++;
            //    }
            //    else
            //    {
            //        count[temp]--;
            //    }
            //    detectArea[temp].GetComponentInChildren<Text>().text = count[temp].ToString();

            //    if (isDragIn)
            //    {
            //        isDragIn = false;
            //        detectArea[temp].DOKill();
            //        detectArea[temp].localScale = Vector3.one;
            //    }
            //};

            //ScrollInfinityDemoItem.OnItemDragging = item =>
            //{
            //    var temp = item.Order % 3;
            //    if (rectDetect[temp].CheckIn(item.RectTrans))
            //    {
            //        if (isDragIn)
            //        {
            //            return;
            //        }

            //        isDragIn = true;
            //        DOTweenManager.Instance.TweenScaleTime(detectArea[temp], Vector3.one * 1.3f, 0.3f)
            //            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            //    }
            //    else
            //    {
            //        if (!isDragIn)
            //        {
            //            return;
            //        }

            //        isDragIn = false;
            //        detectArea[temp].DOKill();
            //        detectArea[temp].localScale = Vector3.one;
            //    }
            //};
        }
	}
}