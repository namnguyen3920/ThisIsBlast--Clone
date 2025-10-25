using UnityEngine;

public class UIManager : Singleton_Mono_Method<UIManager>
{
    public RectTransform uiFinishLevelPanel;
    public RectTransform uiTutorialLevelPanel;
    public RectTransform uiGameOverPanel;
    public void ShowFinishLevelPanel(bool show)
    {
        ShowPanel(uiFinishLevelPanel, show);
    }
    public void ShowTutorialPanel(bool show)
    {
        ShowPanel(uiTutorialLevelPanel, show);
    }
    public void ShowGameOverPanel(bool show)
    {
        ShowPanel(uiGameOverPanel, show);
    }
    public void OnContinueButton()
    {
        GameManager.d_Instance.LoadNextLevel();
    }
    public void OnRestartButton()
    {
        uiGameOverPanel.gameObject.SetActive(false);
        GameManager.d_Instance.RestartLevel();
    }
    public void OnTryAgainButton()
    {
        uiGameOverPanel.gameObject.SetActive(false);
        GameManager.d_Instance.TryAgain();
    }

    // HELPER
    private void ShowPanel(RectTransform panel, bool show)
    {
        panel.gameObject.SetActive(show);
    }
}
