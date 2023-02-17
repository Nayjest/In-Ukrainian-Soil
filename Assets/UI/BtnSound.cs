using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler
{
    private bool registeredClick = false;
    private bool registeredSelect = false;
    private bool hovered = false;
    private float selectedTime = 0;
    private Button btn;
    private Toggle tgl;

    public AudioClip Select;
    public AudioClip Click;
    public AudioSource Src;

    
    public void Awake()
    {
        btn = GetComponent<Button>();
        tgl = GetComponent<Toggle>();
        if (btn) btn.onClick.AddListener(() => { registeredClick = true; });
    }

    public void Start()
    {
        if (tgl)
        {
            tgl.onValueChanged.AddListener((x) =>
            {
                if (tgl.isOn) registeredClick = true;
            });
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        registeredSelect = true;
        selectedTime = Time.unscaledTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        registeredClick = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!hovered) registeredSelect = true;
    }

    void LateUpdate()
    {
        if (btn && !btn.interactable) return;
        if (registeredClick)
        {
            if (Src && Click)
                Src.PlayOneShot(Click);
            else 
                SFXManager.Inst.Play("UI.BtnClick");
            registeredClick = false;
            registeredSelect = false;
        }
        // start sound only if was selected during >1 frame
        if (registeredSelect && Time.unscaledTime - selectedTime >= 1f/10000f)
        {
            if (Src && Select)
                Src.PlayOneShot(Select);
            else
                SFXManager.Inst.Play("UI.BtnSelect");

            registeredSelect = false;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        registeredSelect = false;
    }
}
