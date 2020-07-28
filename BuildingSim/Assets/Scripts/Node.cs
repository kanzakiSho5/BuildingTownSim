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
            // TODO: 動かしてない側のconectLineInfoの初期化
            conectLineInfo = new List<lineInfo>();
            if(OnChangePosition != null)
                OnChangePosition();
        }

        temp = position;
    }

    public void AddLine(lineInfo info)
    {
        
        Debug.Log(info.position);
        if (info.lineCount == null)
        {
            info.ChangeCount(conectLineInfo.Count);
            conectLineInfo.Add(info);
        }
        else
        {
            conectLineInfo[(int)info.lineCount] = info;
        }

    }

    public void OnClick()
    {
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
        Debug.Log(pos);
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
            Debug.Log(conectLineInfo[i].position +", "+ transform.position);
            Gizmos.DrawLine(transform.position, conectLineInfo[i].position);
        }
    }
}
