using UnityEngine;
using UnityEngine.EventSystems;

public class LetterTouchHandler : MonoBehaviour, IBeginDragHandler, IPointerEnterHandler
{
    int dragging = -1;

    void Start()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            dragging = 0;
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragging = 1;
            }
            if (Input.GetMouseButtonUp(0))
            {
                dragging = 0;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.OSXEditor) return;
        Debug.Log("Dragging in Editor");
        string myName = eventData.pointerDrag.name;
        FindObjectOfType<WordSearchGameManager>().UpdateLine(myName);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dragging == 1)
        {
            Debug.Log("Dragging in Editor");
            string myName = eventData.pointerEnter.name;
            FindObjectOfType<WordSearchGameManager>().UpdateLine(myName);
        }
        else if (dragging == -1)
        {
            Debug.Log("Dragging in Device");
            string myName = eventData.pointerEnter.name;
            FindObjectOfType<WordSearchGameManager>().UpdateLine(myName);
        }
    }
}
