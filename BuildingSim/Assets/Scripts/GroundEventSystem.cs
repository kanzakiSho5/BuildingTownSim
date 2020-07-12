using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class GroundEventSystem : MonoBehaviour, IPointerClickHandler,IDragHandler
{
    private bool isClicked = false;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (GameManager.Instance.CullentCreateType)
        {
            case CreateType.Move:
                break;
            case CreateType.CreateLoad:
                NodeCreator.Instance.CreateNode(eventData.pointerPressRaycast.worldPosition);
                break;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (GameManager.Instance.CullentCreateType)
        {
            case CreateType.Move:
                Debug.Log(eventData.pointerCurrentRaycast.worldPosition);
                NodesManager.Instance.MoveSelectNode(eventData.pointerCurrentRaycast.worldPosition);
                break;
            
        }
    }
}
