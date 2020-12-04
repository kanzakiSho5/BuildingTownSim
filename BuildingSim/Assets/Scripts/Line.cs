using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Node startNode;
    private int? startNodeNumber;
    private Node endNode;
    private int? endNodeNumber;
    private Vector3 handlePos;

    private Bezier lineNodePosition;

    [Tooltip("道の分割数")] 
    [SerializeField] private int detail = 4;
    
    [Tooltip("道の幅")]
    [SerializeField] private float loadLength = 2;

    #region public Method
    public void SetNode(Node start, Node end)
    {
        //Debug.Log(startNode);
        startNode = start;
        endNode = end;
        handlePos = Vector3.Lerp(start.position, end.position, 0.5f);
        startNode.OnChangePosition += OnChangePositionHandler;
        endNode.OnChangePosition += OnChangePositionHandler;
        lineNodePosition = new Bezier(startNode.position, endNode.position, handlePos, detail * 2 + 2);
    }
    
    public void SetNode(Node start, Node end, Vector3 handlePos)
    {
        // Debug.Log(startNode);
        startNode = start;
        endNode = end;
        this.handlePos = handlePos;
        startNode.OnChangePosition += OnChangePositionHandler;
        endNode.OnChangePosition += OnChangePositionHandler;
        
        lineNodePosition = new Bezier(startNode.position, endNode.position, this.handlePos, detail * 2 + 2);
        Debug.Log(lineNodePosition.Detail);
    }
    #endregion

    #region private Method
    private void OnChangePositionHandler()
    {
        Transform startSideRoadBone = transform.GetChild(0).GetChild(1);
        Transform endSideRoadBone = transform.GetChild(0).GetChild(0);
        // string str = "";
        
        lineNodePosition.SetRoot(startNode.position, endNode.position, handlePos);
        transform.GetChild(0).position = lineNodePosition.Positions[detail + 1];
        float dir = GetDirection(startNode.position, endNode.position) * Mathf.Rad2Deg;
        transform.GetChild(0).eulerAngles = Vector3.down * dir;
        
        for (int i = 1; i <= detail; i++)
        {
            var startSideIndex = (detail + 1) + i;
            var endSideIndex = (detail + 1) - i;
            
            startSideRoadBone.position = lineNodePosition.Positions[startSideIndex];
            endSideRoadBone.position   = lineNodePosition.Positions[endSideIndex];
            
            startSideRoadBone.eulerAngles = Vector3.down * (Mathf.Rad2Deg * GetDirection(lineNodePosition.Positions[startSideIndex], lineNodePosition.Positions[startSideIndex - 1]));
            endSideRoadBone.eulerAngles   = Vector3.down * (Mathf.Rad2Deg * GetDirection(lineNodePosition.Positions[endSideIndex], lineNodePosition.Positions[endSideIndex + 1]));
            
            if (i == detail)
            {
                //startSideRoadBone.eulerAngles += Vector3.up * 90;
                //endSideRoadBone.eulerAngles += Vector3.down * 90;
                //Debug.LogFormat("startSideRoadBone : {0}, endSideRoadBone :{1}",startSideRoadBone.name, endSideRoadBone.name);
                if(startNodeNumber == null)
                    startNodeNumber = startNode.AddLine(new lineInfo(endSideRoadBone, loadLength));
                else
                    startNode.ChangeLine(new lineInfo(endSideRoadBone, loadLength, startNodeNumber));
                
                if(endNodeNumber == null)
                    endNodeNumber = endNode.AddLine(new lineInfo(startSideRoadBone, loadLength));
                else
                    endNode.ChangeLine(new lineInfo(startSideRoadBone, loadLength, endNodeNumber));
            }
            
            startSideRoadBone = startSideRoadBone.GetChild(0);
            endSideRoadBone = endSideRoadBone.GetChild(0);
        }
        // Debug.Log(str);
    }

    private float GetDirection(Vector3 start, Vector3 lookAt)
    {
        float rad = Mathf.Atan2(lookAt.z - start.z, lookAt.x - start.x);
        return rad;
    }
    #endregion
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!startNode || !endNode) return;
        Gizmos.color = Color.red;
        foreach (var nodePos in lineNodePosition.Positions)
        {
            Gizmos.DrawSphere(nodePos,0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(handlePos, .2f);
    }
#endif
}
