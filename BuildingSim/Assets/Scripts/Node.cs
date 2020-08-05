using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node : MonoBehaviour
{
    public delegate void OnChangePositionDelegate();
    public event OnChangePositionDelegate OnChangePosition;
    
    Vector3 temp;
    private List<lineInfo> conectLineInfo = new List<lineInfo>();

    public Vector3 position
    {
        get
        {
            Vector3 position = transform.position;
            position.y += .1f;
            return position;
        }
    }
    
    public bool isActive
    {
        get;
        private set;
    }
    
    public void Update()
    {
        
        if (temp != position)
        {
            if(OnChangePosition != null)
                OnChangePosition();
        }

        temp = position;
    }

    public int AddLine(lineInfo info)
    {
        // Debug.Log(info.position);
        Vector3 linePos = info.bone.position;
        Vector3 nodePos = transform.position;
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir);
        info.ChangeCount(conectLineInfo.Count);
        conectLineInfo.Add(info);
        if(conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            setIntersection();
        return (int)info.lineCount;
    }

    public void ChangeLine(lineInfo info)
    {
        Vector3 linePos = info.bone.position;
        Vector3 nodePos = transform.position;
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir);
        conectLineInfo[(int)info.lineCount] = info;
        if(conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            setIntersection();
    }

    public void OnClick()
    {
        
        if (GameManager.Instance.CullentCreateType == CreateType.CreateLoad)
        {
            LineCreator.Instance.OnCreateLoad(NodesManager.Instance.ActiveNode, this);
            Debug.Log(NodesManager.Instance.ActiveNode.position +", "+ this.position);
            temp = Vector3.zero;// Road位置更新
        }

        NodesManager.Instance.AllNodeOnActive(false);
        OnActive(true);
        LineCreator.Instance.OnChangeHandlePos();
    }

    public void OnActive(bool active)
    {
        isActive = active;
        NodesManager.Instance.OnChangeSelectNode();
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        // Debug.Log(pos);
    }

    private void SetWayAngle()
    {
        GameObject endRoad = transform.GetChild(0).gameObject;
        
        endRoad.SetActive(false);
        if(conectLineInfo.Count == 0)
            return;
        
        if (conectLineInfo.Count == 1)
        {
            // 終点処理
            endRoad.SetActive(true);
            lineInfo lineInfo = conectLineInfo[0];
            var dir = lineInfo.direction * Mathf.Rad2Deg;
            transform.GetChild(0).eulerAngles = Vector3.down * dir;
            Vector3 pos = transform.position;
            pos.y = .1f;
            lineInfo.bone.position = pos;
            return;
        }
        
        // 道が続くとき
        endRoad.SetActive(false);
    }
    
    private void setIntersection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        conectLineInfo.Sort(new lineInfoComparer());
        foreach (var lineInfo in conectLineInfo)
        {
            //Debug.Log(lineInfo.bone.gameObject.name +": "+ lineInfo.direction);
            var dir = lineInfo.direction * Mathf.Rad2Deg;
            lineInfo.bone.eulerAngles = Vector3.down * dir;
            Vector3 pos = transform.position;
            pos.y = .1f;
            lineInfo.bone.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.gray;
        if(isActive) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, .5f);
        Gizmos.color = Color.green;
        for (int i = 0; i < conectLineInfo.Count; i++)
        {
            // Debug.Log(conectLineInfo[i].position +", "+ transform.position);
            
            Vector3 linepos = new Vector3(Mathf.Cos(conectLineInfo[i].direction), 0, Mathf.Sin(conectLineInfo[i].direction)) + pos;
            
            Gizmos.DrawLine(transform.position, linepos);
        }
    }
}
