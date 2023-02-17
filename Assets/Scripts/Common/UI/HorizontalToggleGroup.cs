using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalToggleGroup : MonoBehaviour
{
    public HorizontalToggleGroup Upper;
    public HorizontalToggleGroup Bottom;
    public bool Selected;
    void Start()
    {
        if (Selected) Select();
    }

    public void Select()
    {
        Selected = true;
        //GetComponent<ToggleGroup>().ActiveToggles.F
    }
    // Update is called once per frame
    void Update()
    {
        if (Selected)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) && Bottom != null)
            {
                Bottom.Select();
            }
        }
    }
}
