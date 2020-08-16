using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(NodeMeshRenderer))]
public class Node : MonoBehaviour
{
    public delegate void OnChangePositionDelegate();
    public event OnChangePositionDelegate OnChangePosition;
    
    Vector3 temp;
    private List<lineInfo> _conectLineInfo = new List<lineInfo>();
    private NodeMeshRenderer _meshRenderer;

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

    private void OnEnable()
    {
        Debug.Log("OnCreatedRoad");
        _meshRenderer = GetComponent<NodeMeshRenderer>();

    }

    private void Update()
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
        info.ChangeCount(_conectLineInfo.Count);
        _conectLineInfo.Add(info);
        if(_conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            SetIntersection();
        return (int)info.lineCount;
    }

    public void ChangeLine(lineInfo info)
    {
        Vector3 linePos = info.bone.position;
        Vector3 nodePos = transform.position;
        // TODO: 一番親のBoneの角度が0になる問題の修正
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir);
        _conectLineInfo[(int)info.lineCount] = info;
        if(_conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            SetIntersection();
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
        if(_conectLineInfo.Count == 0)
            return;
        
        if (_conectLineInfo.Count == 1)
        {
            // 終点処理
            endRoad.SetActive(true);
            lineInfo lineInfo = _conectLineInfo[0];
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
    
    private void SetIntersection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _conectLineInfo.Sort(new lineInfoComparer());
        foreach (var lineInfo in _conectLineInfo)
        {
            //Debug.Log(lineInfo.bone.gameObject.name +": "+ lineInfo.direction);
            // Boneの方向調整
            var dir = lineInfo.direction * Mathf.Rad2Deg;
            lineInfo.bone.eulerAngles = Vector3.down * dir;
            Vector3 pos = transform.position;
            pos.y = .1f;
            lineInfo.bone.position = pos;
            
            // Meshの張る頂点の計算
            
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.gray;
        if(isActive) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, .5f);
        Gizmos.color = Color.green;
        for (int i = 0; i < _conectLineInfo.Count; i++)
        {
            // Debug.Log(conectLineInfo[i].position +", "+ transform.position);
            
            Vector3 linepos = new Vector3(Mathf.Cos(_conectLineInfo[i].direction), 0, Mathf.Sin(_conectLineInfo[i].direction)) + pos;
            
            Gizmos.DrawLine(transform.position, linepos);
        }
    }
}
