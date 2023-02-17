using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

[RequireComponent(typeof(Selectable))]
public class SelectableAffectChildColor : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public Image Img;
    public TMP_Text Txt;
    public SpriteRenderer Sprite;

    Color baseColor;
    Selectable btn;

    public bool UseCustomColors;

    [ShowIf("UseCustomColors")]
    public ColorBlock CustomColors;
    ColorBlock colors => UseCustomColors ? CustomColors : btn.colors;

    bool interactableDelay;

    bool selected = false;
    bool highlighted = false;    

    bool HasTarget => Txt != null || Img != null || Sprite != null;
    void Awake()
    {   
        if (!HasTarget) Txt = GetComponentInChildren<TMP_Text>();
        if (!HasTarget) Img = transform.GetChild(0).GetComponent<Image>();
        
        if (!HasTarget) this.enabled = false;
        //baseColor = txt.color;
        baseColor = Color.white;
        btn = gameObject.GetComponent<Selectable>();
        interactableDelay = btn.interactable;        
        SetNormalColor();
        
        // For click by keyboard
        var realBtn = gameObject.GetComponent<Button>();
        if (realBtn)
        {
            realBtn.onClick.AddListener(OnNotMouseClick);
        }        
    }
   

    void OnNotMouseClick()
    {
        if (highlighted) return;
        if (c != null) StopCoroutine(c);
        c = StartCoroutine(Blink(
            FinalColor(colors.pressedColor),
            FinalColor(colors.selectedColor)
        ));
    }

    void SetNormalColor()
    {
        
        var color = FinalColor(colors.normalColor);
        if (Txt) Txt.color = color;
        if (Img) Img.color = color;
        if (Sprite) Sprite.color = color;
    }

    Color FinalColor(Color c)
    {
        return (btn.interactable ? c : colors.disabledColor) * baseColor * colors.colorMultiplier;
    }

    void Update()
    {
      
        if (btn.interactable != interactableDelay)
        {
            SwitchTo(FinalColor(selected ? colors.selectedColor : colors.normalColor));
        }
        interactableDelay = btn.interactable;         
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlighted = true;
        SwitchTo(FinalColor(colors.highlightedColor));        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SwitchTo(FinalColor(colors.pressedColor));        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SwitchTo(FinalColor(colors.highlightedColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
        SwitchTo(FinalColor(selected ? colors.selectedColor : colors.normalColor));
    }

    IEnumerator SwitchColor(Color target)
    {
        if (!HasTarget) yield break;
        var c = Txt?Txt.color:(Img?Img.color:Sprite.color);
        float t = 0;
        float l = colors.fadeDuration;
        float i;
        Color newC;
        while (t < l)
        {
            // @todo duration is incorrect
            i = t / l;
            t += Time.fixedUnscaledDeltaTime;
            newC = new Color(
                Mathf.Lerp(c.r,target.r, i),
                Mathf.Lerp(c.g, target.g, i),
                Mathf.Lerp(c.b, target.b, i),
                Mathf.Lerp(c.a, target.a, i)
            );
            if (Txt) Txt.color = newC;
            if (Img) Img.color = newC;
            if (Sprite) Sprite.color = newC;
            yield return new WaitForEndOfFrame();
        }
        if (Txt) Txt.color = target;
        if (Img) Img.color = target;
        if (Sprite) Sprite.color = target;
        yield return null;
    }

    private Coroutine c;
    void SwitchTo(Color target)
    {
        if (c != null) StopCoroutine(c);
        c = StartCoroutine(SwitchColor(target));
        
    }

    IEnumerator Blink(Color c1, Color c2)
    {
        yield return  StartCoroutine(SwitchColor(c1));
        yield return StartCoroutine(SwitchColor(c2));
    }

    public void OnSelect(BaseEventData eventData)
    {        
        selected = true;        
        SwitchTo(FinalColor(colors.selectedColor));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        //Debug.LogError("DeSelect");
        SwitchTo(FinalColor(colors.normalColor));
    }
}