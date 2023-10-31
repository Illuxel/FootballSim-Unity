using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsAnimation : MonoBehaviour
{
    public Button FirstButton;
    public Button FifthButton;
    public List<Button> ButtonsToChange;
    public GameObject ButtonToDisable;
    public Image BlackBackImage;
    public float ChangeSpeed = 1.0f;
    public float MaxChangeDistance = 720.0f;
    public float FadeDuration = 2.0f;
    private List<Vector2> originalPositions;
    private List<float> originalWidths;
    private Vector2 originalFifthButtonSize; 
    private Vector2 originalFifthButtonPosition;
    private float currentChangeDistance = 0;
    private float targetAlpha = 0.8f;
    private bool changing = false;
    private static Status animStatus = Status.IsClose;
    private void Start()
    {
        ButtonToDisable.GetComponent<Button>().onClick.AddListener(StartChanging);
        FirstButton.onClick.AddListener(StartChanging);
        ButtonToDisable.SetActive(false);
        originalFifthButtonSize = FifthButton.GetComponent<RectTransform>().sizeDelta;
        originalFifthButtonPosition = FifthButton.GetComponent<RectTransform>().anchoredPosition;
        originalPositions = new List<Vector2>();
        originalWidths = new List<float>();
    }

    private void Update()
    {
        if(animStatus == Status.IsOpening)
        {
            if (changing)
            {
                currentChangeDistance += ChangeSpeed * Time.deltaTime;

                for (int i = 0; i < ButtonsToChange.Count; i++)
                {
                    Vector2 newSize = ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta;
                    newSize.x = originalWidths[i] + currentChangeDistance;
                    ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta = newSize;

                    Vector2 newPosition = originalPositions[i];
                    newPosition.x += currentChangeDistance / 2;
                    ButtonsToChange[i].GetComponent<RectTransform>().anchoredPosition = newPosition;
                }

                Vector2 newSizeFifth = FifthButton.GetComponent<RectTransform>().sizeDelta;
                newSizeFifth.x -= ChangeSpeed * Time.deltaTime;

                Vector2 newPositionFifth = FifthButton.GetComponent<RectTransform>().anchoredPosition;
                newPositionFifth.x += ChangeSpeed * Time.deltaTime / 2;

                newSizeFifth.x = Mathf.Max(newSizeFifth.x, 0f);
                FifthButton.GetComponent<RectTransform>().sizeDelta = newSizeFifth;
                FifthButton.GetComponent<RectTransform>().anchoredPosition = newPositionFifth;

                if (currentChangeDistance >= MaxChangeDistance)
                {
                    for (int i = 0; i < ButtonsToChange.Count; i++)
                    {
                        Vector2 newSize = ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta;
                        newSize.x = 720;
                        ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta = newSize;

                        Vector2 newPosition = originalPositions[i];
                        newPosition.x += currentChangeDistance / 2;
                        ButtonsToChange[i].GetComponent<RectTransform>().anchoredPosition = newPosition;
                    }
                        ButtonToDisable.SetActive(true);
                    currentChangeDistance = MaxChangeDistance;
                    changing = false;
                    FifthButton.interactable = true;
                    FirstButton.interactable = true;
                    foreach (Button button in ButtonsToChange)
                    {
                        button.interactable = true;
                    }
                    animStatus = Status.IsOpen;
                }             
            }         
        }
        else if(animStatus == Status.IsClosing)
        {
            if (currentChangeDistance < MaxChangeDistance)
            {
                currentChangeDistance += ChangeSpeed * Time.deltaTime;

                for (int i = 0; i < ButtonsToChange.Count; i++)
                {
                    Vector2 newSize = ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta;
                    newSize.x = originalWidths[i] - currentChangeDistance;
                    ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta = newSize;

                    Vector2 newPosition = originalPositions[i];
                    newPosition.x -= currentChangeDistance / 2;
                    ButtonsToChange[i].GetComponent<RectTransform>().anchoredPosition = newPosition;
                }
            }          
            Vector2 newPositionFifth = FifthButton.GetComponent<RectTransform>().anchoredPosition;
            newPositionFifth.x -= ChangeSpeed * Time.deltaTime / 2;
            Vector2 newSizeFifth = FifthButton.GetComponent<RectTransform>().sizeDelta;
            newSizeFifth.x += ChangeSpeed * Time.deltaTime; 
            newSizeFifth.x = Mathf.Min(newSizeFifth.x, 720);

            FifthButton.GetComponent<RectTransform>().sizeDelta = newSizeFifth;
            FifthButton.GetComponent<RectTransform>().anchoredPosition = newPositionFifth;

            if (newSizeFifth.x == 720)
            {
                animStatus = Status.IsClose;
                FirstButton.interactable = true;
                FifthButton.interactable = true;
                foreach (Button button in ButtonsToChange)
                {
                    button.interactable = true;
                }
            }
        }   
    }
    private void updateButtons()
    {
        originalFifthButtonSize = FifthButton.GetComponent<RectTransform>().sizeDelta;
        originalFifthButtonPosition = FifthButton.GetComponent<RectTransform>().anchoredPosition;
        originalPositions = new List<Vector2>();       
        originalWidths = new List<float>();

        foreach (var button in ButtonsToChange)
        {
            originalWidths.Add(button.GetComponent<RectTransform>().sizeDelta.x);
            originalPositions.Add(button.GetComponent<RectTransform>().anchoredPosition);  
        }
    }
    private void StartChanging()
    {
        updateButtons();
        FirstButton.interactable = false;
        FifthButton.interactable = false;
        foreach (Button button in ButtonsToChange)
        {            
            button.interactable = false;           
        }
        if (animStatus ==   Status.IsClose)
        {
            animStatus = Status.IsOpening;
            targetAlpha = 0.8f;
        }
        else if(animStatus == Status.IsOpen)
        {
            ButtonToDisable.SetActive(false);
            animStatus = Status.IsClosing;
            targetAlpha = 0f;
        }
        currentChangeDistance = 0.0f;
        changing = true;
        StartCoroutine(FadeImage());
    }
   
    private IEnumerator FadeImage()
    {
        float startAlpha = BlackBackImage.color.a;
        float elapsedTime = 0;

        while (elapsedTime < FadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / FadeDuration);   
            BlackBackImage.color =  new Color(BlackBackImage.color.r, BlackBackImage.color.g, BlackBackImage.color.b, alpha);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        BlackBackImage.color = new Color(BlackBackImage.color.r, BlackBackImage.color.g, BlackBackImage.color.b, targetAlpha);
    }
   
    public void BackToDefault()
    {
        animStatus = Status.IsClose;
        BlackBackImage.color = new Color(BlackBackImage.color.r, BlackBackImage.color.g, BlackBackImage.color.b, 0);

        for (int i = 0; i < ButtonsToChange.Count; i++)
        {
            ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidths[i], ButtonsToChange[i].GetComponent<RectTransform>().sizeDelta.y);
            ButtonsToChange[i].GetComponent<RectTransform>().anchoredPosition = originalPositions[i];            
        }
        FifthButton.GetComponent<RectTransform>().sizeDelta = originalFifthButtonSize;
        FifthButton.GetComponent<RectTransform>().anchoredPosition = originalFifthButtonPosition;
    }
    public enum Status 
    {
        IsClose = 0,
        IsClosing = 1,
        IsOpening = 2,
        IsOpen = 3
    }

}
