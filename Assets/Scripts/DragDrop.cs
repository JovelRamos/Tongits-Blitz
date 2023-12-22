using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems; // Required for drag and drop

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform dragRectTransform;
    private CanvasGroup canvasGroup;
    public RectTransform discardPile; // Assign this in the inspector
    private Vector2 originalPosition;
    private Vector2 dragOffset;
    public Action<GameObject> OnCardDiscarded; // Action to notify when a card is discarded
    public bool IsInDiscardPile { get; private set; } = false; // Flag to track if the card is in discard pile




    void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = dragRectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // Allows the drop to be detected
        canvasGroup.alpha = 0.6f; // Make the card semi-transparent while dragging

        // Calculate offset from cursor to the card's pivot
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPointerPosition);
            dragOffset = localPointerPosition - dragRectTransform.anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPointerPosition);
            dragRectTransform.anchoredPosition = localPointerPosition - dragOffset;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f; // Return the card to full opacity

        if (!RectTransformUtility.RectangleContainsScreenPoint(discardPile, eventData.position))
        {
            dragRectTransform.anchoredPosition = originalPosition;
        }
        else
        {

            IsInDiscardPile = true; // Set the flag
            OnCardDiscarded?.Invoke(gameObject);

            // Remove existing children from discard pile
            if (discardPile.childCount > 0)
            {
                var toRemove = discardPile.GetChild(0).gameObject;
                Destroy(toRemove);
            }

            // Set the card as the new child of discard pile
            IsInDiscardPile = true; // Set the flag

            transform.SetParent(discardPile);
            dragRectTransform.anchoredPosition = Vector2.zero; // Adjust if needed based on layout
            Debug.Log("Card dropped on discard pile");
        }
    }

}
