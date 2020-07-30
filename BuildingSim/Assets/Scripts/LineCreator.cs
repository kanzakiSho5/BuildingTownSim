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
        lastHandlePos = handlePos;
        GameObject obj = Instantiate(LoadObj, endNode.position, Quaternion.identity);
        
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
            
            handlePos = GetIntersection(rootCenter, rotatedRootCenter, startNode.position,lastHandlePos);
        }
        obj.GetComponent<Line>().SetNode(startNode, endNode, handlePos);
    }

    public void Update()
    {
        
    }

    public void OnChangeHandlePos()
    {
        handlePos = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        // 実行前エラー回避
        if(!NodesManager.Instance.ActiveNode)
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

        Debug.Log(handleVec +", "+ endNodePos);
        Vector3 newHandlePos = GetIntersection(rootCenter, rotatedRootCenter,endNodePos, handlePos);
        
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

    private Vector3 GetIntersection(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        /*
            a = (p2[1] - p1[1]) / (p2[0] - p1[0])
            b = p1[1] - a * p1[0] 

            c = (p4[1] - p3[1]) / (p4[0] - p3[0])
            d = p3[1] - c * p3[0] 
         */
        
        float a = (pos2.z - pos1.z) / (pos2.x - pos1.x);
        float b = pos1.z - a * pos1.x;
        float c = (pos4.z - pos3.z) / (pos4.x - pos3.x);
        float d = pos3.z - c * pos3.x;

        Vector3 ret = new Vector3((d - b) / (a - c), .1f, (a * d - b * c) / (a - c));
        Debug.Log(ret);
        return ret;
    }
}
