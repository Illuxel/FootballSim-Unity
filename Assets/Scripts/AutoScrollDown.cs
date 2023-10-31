using UnityEngine;
using UnityEngine.UI;

public class AutoScrollDown : MonoBehaviour
{
    public ScrollRect scrollRect;
    static bool autoScrollWorking = true;

    private void Start()
    {
       
        InvokeRepeating("ScrollToBottom", 0f, 0.1f);
    }
    public void AutoScrollToggle(bool toggle)
    {
        autoScrollWorking = toggle;
    }
    private void ScrollToBottom()
    {  
        if (scrollRect.content != null && autoScrollWorking == true)
        {
            if (scrollRect.content.rect.height > scrollRect.viewport.rect.height)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }
    
}
