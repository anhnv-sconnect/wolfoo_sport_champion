using UnityEngine;

public class GridControlSize : MonoBehaviour
{
    [Tooltip("is vertical")]
    [SerializeField] bool isVertical;

    [Tooltip("Number of item each row")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int numberOfItemInRow;
    [Tooltip("Number of item each col")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int numberOfItemInCol;

    [Tooltip("Distance bw two rows")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float spaceOfRow;
    [Tooltip("Distance bw two cols")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float spaceOfCol;

    [SerializeField] float marginAnchor;

    [Tooltip("Distance to the top")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int topDistance;
    [Tooltip("Distance to the left")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int leftDistance;

    [Tooltip("Distance to the bot")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int botDistance;
    [Tooltip("Distance to the right")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int rightDistance;

    [SerializeField] float heightItem;
    [SerializeField] float widthItem;

    [Tooltip("The nearest parent has constant width")]
    [SerializeField] RectTransform mainParent;

    [Tooltip("Is content has const width or not, if not width of content is the different of main parent and real width")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] bool isConstWidth;
    [Tooltip("Width of content")]
    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float widthOfContent;

    [Tooltip("Is content has const heigh or not, if not heigh of content is the different of main parent and real heigh")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] bool isConstHeigh;
    [Tooltip("Heigh of content")]
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float heighOfContent;

    [Tooltip("is alignment center or left")]
    [SerializeField] bool isAlignmentCenter;

    RectTransform rectTrans;
    RectTransform RectTrans
    {
        get
        {
            if (rectTrans == null)
            {
                rectTrans = GetComponent<RectTransform>();
            }
            return rectTrans;
        }
    }

    [Space]
    [Header("Calculated value")]

    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float realWidth;
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float realHeigh;

    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float anchorsLeft;
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float anchorsTop;

    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float anchorsLeftLastRow;
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] float anchorsTopLastCol;

    [SerializeField] float anchorsSpace;

    [DrawIf(nameof(isVertical), true, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int lastRow;
    [DrawIf(nameof(isVertical), false, DrawIfAttribute.DisablingType.DontDraw)]
    [SerializeField] int lastCol;

    public void ResetAllItems()
    {
        if (isVertical)
        {
            RectTrans.pivot = new Vector2(0.5f, 1);
            RectTrans.anchorMin = new Vector2(0, 1);
            RectTrans.anchorMax = new Vector2(1, 1);
        }
        else
        {
            RectTrans.pivot = new Vector2(0, 0.5f);
            RectTrans.anchorMin = new Vector2(0, 0);
            RectTrans.anchorMax = new Vector2(0, 1);
        }

        var itemAmount = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                itemAmount--;
            }
        }

        if (itemAmount == 0)
        {
            RectTrans.sizeDelta = Vector2.zero;
            return;
        }

        if (isVertical)
		{
            realWidth = isConstWidth ? -mainParent.sizeDelta.x + widthOfContent : widthOfContent;

            var contentSize = mainParent.sizeDelta.x - widthOfContent;
            anchorsLeft = numberOfItemInRow > 1 ? (widthItem / 2 / contentSize) + marginAnchor : 0.5f;
            anchorsSpace = numberOfItemInRow > 1 ? (1 - (anchorsLeft * 2)) / (numberOfItemInRow - 1) : 0;

            lastRow = (itemAmount - 1) / numberOfItemInRow;

            var surplus = itemAmount % numberOfItemInRow;
            var numberItemsOfLastRow = surplus == 0 ? numberOfItemInRow : surplus;
            anchorsLeftLastRow = numberItemsOfLastRow > 1
                ? 0.5f - ((numberItemsOfLastRow - 1) * anchorsSpace / 2) : 0.5f;
        }
        else
		{
            realHeigh = isConstHeigh ? -mainParent.sizeDelta.y + heighOfContent : heighOfContent;

            var contentSize = mainParent.sizeDelta.y - heighOfContent;
            anchorsTop = numberOfItemInCol > 1 ? (heightItem / 2 / contentSize) + marginAnchor : 0.5f;
            anchorsSpace = numberOfItemInCol > 1 ? (1 - (anchorsTop * 2)) / (numberOfItemInCol - 1) : 0;

            lastCol = (itemAmount - 1) / numberOfItemInCol;

            var surplus = itemAmount % numberOfItemInCol;
            var numberItemsOfLastCol = surplus == 0 ? numberOfItemInCol : surplus;
            anchorsTopLastCol = numberItemsOfLastCol > 1
                ? 0.5f - ((numberItemsOfLastCol - 1) * anchorsSpace / 2) : 0.5f;
        }

        var index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
                continue;

            SetAnchors(transform.GetChild(i).gameObject, index);
            index++;
        }
    }

    void SetAnchors(GameObject obj, int orderNumber)
    {
        if (isVertical)
        {
            var col = orderNumber % numberOfItemInRow;
            var row = orderNumber / numberOfItemInRow;

            RectTrans.sizeDelta = new Vector2(realWidth
                , ((row + 1) * (heightItem + spaceOfRow)) - spaceOfRow + topDistance + botDistance);

            float xValue = (anchorsSpace * col) + ((isAlignmentCenter && row == lastRow) ? anchorsLeftLastRow : anchorsLeft);
            var objTrans = obj.GetComponent<RectTransform>();
            objTrans.anchorMin = new Vector2(xValue, 1);
            objTrans.anchorMax = new Vector2(xValue, 1);
            objTrans.anchoredPosition = new Vector2(0, (-heightItem / 2) - ((heightItem + spaceOfRow) * row) - topDistance);
        }
        else
        {
            var col = orderNumber / numberOfItemInCol;
            var row = orderNumber % numberOfItemInCol;

            RectTrans.sizeDelta = new Vector2(
                ((col + 1) * (widthItem + spaceOfCol)) - spaceOfCol + leftDistance + rightDistance, realHeigh);

            float yValue = (anchorsSpace * row) + ((isAlignmentCenter && col == lastCol) ? anchorsTopLastCol : anchorsTop);
            var objTrans = obj.GetComponent<RectTransform>();
            objTrans.anchorMin = new Vector2(0, (1 - yValue));
            objTrans.anchorMax = new Vector2(0, (1 - yValue));
            objTrans.anchoredPosition = new Vector2(widthItem / 2 + (widthItem + spaceOfCol) * col + leftDistance, 0);
        }
    }

    private void OnEnable()
    {
        //ResetAllItems();
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(InitValue))]
    void InitValue()
    {
		ResetAllItems();

        _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
        (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
#endif
}