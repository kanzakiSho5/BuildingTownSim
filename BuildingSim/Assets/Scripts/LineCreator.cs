using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class LineCreator : MonoBehaviour
{
    public static LineCreator Instance { get; private set; }
    public bool isOnCreate = false;
    [FormerlySerializedAs("currentHandlePos")] public Vector3 CurrentHandlePos;


    private Vector3 lastHandlePos = Vector3.zero;
    private Vector3 handlePos = Vector3.zero;

    [FormerlySerializedAs("LoadObj")] [SerializeField] private GameObject loadObj;
    [FormerlySerializedAs("LoadAssister")] [SerializeField] private GameObject loadAssister;
    [FormerlySerializedAs("GuideLine")] [SerializeField] private GameObject guideLine;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void Update()
    {
        if(NodesManager.Instance)
            if(NodesManager.Instance.ActiveNode)
                DrawCreateLoadAssister();
    }

    public void OnCreateRoad(Node startNode, Node endNode)
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
            
            handlePos = GameManager.GetIntersection(rootCenter, rotatedRootCenter, startNode.position, handlePos);
        }
        
        GameObject obj = Instantiate(loadObj, handlePos, Quaternion.identity);
        obj.GetComponent<Line>().SetNode(startNode, endNode, handlePos);
        //Debug.Log("Create Road: "+ startNode.position +", "+ endNode.position);
    }

    public void OnCreateRoad(Node startNode, Node endNode, Vector3 handlePosition)
    {
        handlePos = handlePosition;
        GameObject obj = Instantiate(loadObj, handlePosition, Quaternion.identity);
        obj.GetComponent<Line>().SetNode(startNode, endNode, handlePosition);
    }

    public void OnChangeHandlePos()
    {
        handlePos = Vector3.zero;
    }

    public Vector3 GetHandlePos()
    {
        return handlePos;
    }

    private void DrawCreateLoadAssister()
    {
        var activeNodePos = NodesManager.Instance.ActiveNode.position;
        var cursorPos = GameManager.Instance.CursorPos;
        Vector3 rootCenter = Vector3.Lerp(activeNodePos, cursorPos, .5f);
        
        var assisterBone = loadAssister.transform.GetChild(0);
        
        

        var dir = Mathf.Atan2(cursorPos.z - activeNodePos.z, cursorPos.x - activeNodePos.x);
        var eulerAngles = Vector3.down * ((dir - Mathf.PI * .5f) * Mathf.Rad2Deg);
        
        assisterBone.position = activeNodePos;
        assisterBone.eulerAngles = eulerAngles;
        
        assisterBone.GetChild(0).position = cursorPos;
        assisterBone.GetChild(0).eulerAngles = eulerAngles;
        
        var activeHandlePos = handlePos - activeNodePos;
        activeHandlePos = new Vector3(-activeHandlePos.x, activeHandlePos.y, -activeHandlePos.z) + activeNodePos;
        if (handlePos == Vector3.zero)
            activeHandlePos = Vector3.Lerp(cursorPos, activeNodePos, .5f);
        Vector3 tmpCursorPos = cursorPos - rootCenter;
        Vector3 rotatedRootCenter = new Vector3(-tmpCursorPos.z, tmpCursorPos.y, tmpCursorPos.x ) + rootCenter;
        
        CurrentHandlePos = GameManager.GetIntersection(rootCenter, rotatedRootCenter,activeNodePos, activeHandlePos);
        // Debug.Log(currentHandlePos +", "+activeNodePos);
        guideLine.transform.GetChild(0).eulerAngles = new Vector3(
            90,
            -Mathf.Atan2(CurrentHandlePos.z - activeNodePos.z, CurrentHandlePos.x - activeNodePos.z) * Mathf.Rad2Deg,
            0f);
        Vector3 tempPos = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            var startLerp = Vector3.Lerp(activeNodePos, CurrentHandlePos, i*.1f);
            var endLerp = Vector3.Lerp(CurrentHandlePos, cursorPos, i*.1f);
            var bezierPos = Vector3.Lerp(startLerp,endLerp, i*.1f);
            
            if(float.IsNaN(bezierPos.x) ||float.IsNaN(bezierPos.y) ||float.IsNaN(bezierPos.z)) 
                break; 
            
            guideLine.transform.GetChild(i).position = bezierPos;

            if (i == 0)
            {
                tempPos = bezierPos;
                continue;
            }

            var lineDir = Mathf.Atan2(tempPos.z - bezierPos.z, tempPos.x - bezierPos.x) * Mathf.Rad2Deg;
            guideLine.transform.GetChild(i).eulerAngles = new Vector3(90f, -lineDir,0f);

            tempPos = bezierPos;
        }
    }

    private void OnDrawGizmos()
    {
        if(!NodesManager.Instance)
            return;
        
        // 実行前エラー回避
        if(!NodesManager.Instance.ActiveNode)
            return;
        
        // StartNodeとEndNodeをおきかえ
        Vector3 endNodePos = NodesManager.Instance.ActiveNode.position;
        Vector3 cursorPos = GameManager.Instance.CursorPos;
        
        
        // 直角に回転させる原点
        Vector3 rootCenter = Vector3.Lerp(endNodePos, cursorPos, .5f);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(rootCenter, 1f);

        //float cursorLength = Mathf.Pow(endNodePos.x - cursorPos.x, 2) + Mathf.Pow(endNodePos.y - cursorPos.y, 2);
        
        Vector3 handleVec = handlePos - endNodePos;
        handleVec = new Vector3(-handleVec.x, handleVec.y, -handleVec.z) + endNodePos;

        Vector3 tmpCursorPos = cursorPos - rootCenter;
        Vector3 rotatedRootCenter = new Vector3(-tmpCursorPos.z, tmpCursorPos.y, tmpCursorPos.x ) + rootCenter;
        
        
        
        //Gizmos.DrawSphere(rotatedRootCenter, .5f);

        //Debug.Log(handleVec +", "+ endNodePos);
        Vector3 newHandlePos = GameManager.GetIntersection(rootCenter, rotatedRootCenter,endNodePos, handleVec);
        
        
        Gizmos.DrawSphere(newHandlePos, .5f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(endNodePos, handleVec);
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
