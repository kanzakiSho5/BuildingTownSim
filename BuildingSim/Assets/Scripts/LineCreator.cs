using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public static LineCreator Instance { get; private set; }
    public bool isOnCreate = false;


    private Vector3 lastHandlePos = Vector3.zero;
    private Vector3 handlePos = Vector3.zero;

    [SerializeField] private GameObject LoadObj;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void OnCreateLoad(Node startNode, Node endNode)
    {
        // handleの位置
        if (handlePos == Vector3.zero) // 初期値
        {
            handlePos = Vector3.Lerp(startNode.position, endNode.position, .5f);
        }
        else
        {
            Vector3 rootCenter = Vector3.Lerp(endNode.position, startNode.position, .5f);

            Vector3 tmpCenterPos = endNode.position - rootCenter;
            Vector3 rotatedRootCenter = new Vector3(-tmpCenterPos.z, tmpCenterPos.y, tmpCenterPos.x) + rootCenter;
            
            handlePos = GameManager.GetIntersection(rootCenter, rotatedRootCenter, startNode.position,handlePos);
        }
        GameObject obj = Instantiate(LoadObj, handlePos, Quaternion.identity);
        Debug.Log("Create Road: "+ startNode.position +", "+ endNode.position);
        
        obj.GetComponent<Line>().SetNode(startNode, endNode, handlePos);
    }

    public void OnChangeHandlePos()
    {
        handlePos = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        // 実行前エラー回避
        if(!NodesManager.Instance)
            return;
        
        // StartNodeとEndNodeをおきかえ
        Vector3 endNodePos = NodesManager.Instance.ActiveNode.position;
        Vector3 cursorPos = GameManager.Instance.CursorPos;
        Vector3 handleVec = lastHandlePos;
        
        // 直角に回転させる原点
        Vector3 rootCenter = Vector3.Lerp(endNodePos, cursorPos, .5f);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(rootCenter, 1f);

        //float cursorLength = Mathf.Pow(endNodePos.x - cursorPos.x, 2) + Mathf.Pow(endNodePos.y - cursorPos.y, 2);
        

        Vector3 tmpCursorPos = cursorPos - rootCenter;
        Vector3 rotatedRootCenter = new Vector3(-tmpCursorPos.z, tmpCursorPos.y, tmpCursorPos.x ) + rootCenter;
        
        //Gizmos.DrawSphere(rotatedRootCenter, .5f);

        //Debug.Log(handleVec +", "+ endNodePos);
        Vector3 newHandlePos = GameManager.GetIntersection(rootCenter, rotatedRootCenter,endNodePos, handlePos);
        
        Gizmos.DrawSphere(newHandlePos, .5f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(endNodePos, handlePos);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rootCenter, rotatedRootCenter);
        
        /*
        // endNodeからrootCenterまでの長さ
        float rootLength =
            Mathf.Sqrt(Mathf.Pow(endNodePos.x - cusorPos.x, 2) + Mathf.Pow(endNodePos.y - cusorPos.y, 2));
        float lastHandleLength =
            Mathf.Sqrt(Mathf.Pow(handleVec.x - endNodePos.x, 2) + Mathf.Pow(handleVec.y - endNodePos.y, 2));

        float cosAngle = (((cusorPos.x - endNodePos.x) * (handleVec.x - endNodePos.x)) + ((cusorPos.y - endNodePos.y) * (handleVec.y - endNodePos.y))) / (rootLength * lastHandleLength);
        
        // endNodeからHandleまでの長さ
        float handleLength = rootLength / cosAngle;
        
        Vector3 newHandlePos = new Vector3(handleLength * cosAngle, .1f, handleLength * Mathf.Sin(Mathf.Acos(cosAngle)));
        
        Gizmos.DrawLine(endNodePos, newHandlePos);
        
        
        Debug.LogFormat("{0}, {1}",handleLength, cosAngle);
        */
        
        //float newHandlePos = new Vector3();
        
        //Gizmos.DrawSphere();
        

    }
}
