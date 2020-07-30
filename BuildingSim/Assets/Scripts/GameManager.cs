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
}
