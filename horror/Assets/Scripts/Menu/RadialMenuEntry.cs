using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private RawImage icon;
    private RectTransform rect;
    private Curse ability;
    private float cost;

    public void SetLabel(string pText)
    {
        label.text = pText +  " (" + cost + ")";
    }

    public void SetIcon(Texture pIcon)
    {
        icon.texture = pIcon;
    }

    public void SetCost(float pCost)
    {
        cost = pCost;
        ability.SetCost(pCost);
    }

    public void SetAbility(Curse pAbility)
    {
        ability = pAbility;
    }

    private void Start()
    {
        rect = icon.GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOComplete();
        rect.DOScale(Vector3.one * 1.2f, .3f).SetEase(Ease.OutQuad);
        this.GetComponentInParent<RadialMenu>().GetCurseManager().currentAbility = ability;
        Debug.Log(ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOComplete();
        rect.DOScale(Vector3.one * 0.8f, .3f).SetEase(Ease.OutQuad);
    }
}
