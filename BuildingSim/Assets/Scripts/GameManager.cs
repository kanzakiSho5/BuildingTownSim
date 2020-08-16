using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreateType
{
    Selector,
    Move,
    CreateLoad,
    CreateBuilding
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public CreateType CullentCreateType;
    public bool isOnCreate = false;
    public Vector3 SelectorPos;
    public Vector3 CursorPos
    {
        get
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray,200f);

            foreach (var hit in hits)
            {
                //Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "ground")
                {
                    Vector3 pos = hit.point;
                    pos.y += .1f;
                    return pos;
                }
            }

            return Vector3.zero;
        }
    }
    
    [SerializeField]
    private GameObject Selector;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Update()
    {
        //if (isOnCreate)
        {
            Selector.transform.position = CursorPos;
        }
    }

    public void ChangeCreateType(CreateType createType)
    {
        CullentCreateType = createType;
    }
    
    public static Vector3 GetIntersection(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        /*
            a = (p2[1] - p1[1]) / (p2[0] - p1[0])
            b = p1[1] - a * p1[0] 

            c = (p4[1] - p3[1]) / (p4[0] - p3[0])
            d = p3[1] - c * p3[0] 
         */
        /*
        float a = (pos2.z - pos1.z) / (pos2.x - pos1.x);
        float b = pos1.z - a * pos1.x;
        float c = (pos4.z - pos3.z) / (pos4.x - pos3.x);
        float d = pos3.z - c * pos3.x;

        Vector3 ret = new Vector3((d - b) / (a - c), .1f, (a * d - b * c) / (a - c));
        //Debug.Log(ret);
        return ret;
        */

        // 外積での計算法に変更
        float S1 = ((pos4.x - pos3.x) * (pos1.z - pos3.z) - (pos4.z - pos3.z) * (pos1.x - pos3.x)) / 2;
        float S2 = ((pos4.x - pos3.x) * (pos3.z - pos2.z) - (pos4.z - pos3.z) * (pos3.x - pos2.x)) / 2;
        
        Vector3 ret = new Vector3(
                pos1.x + (pos2.x - pos1.x) * S1 / (S1 + S2),
                .1f,
                pos1.z + (pos2.z - pos1.z) * S1 / (S1 + S2)
            );
        
        return ret;
    }
}
