using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ALittleFolkTale.Core;

namespace ALittleFolkTale.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button backButton;

        [Header("Book Animation")]
        [SerializeField] private GameObject bookObject;
        [SerializeField] private Animator bookAnimator;
        [SerializeField] private float bookAnimationDuration = 2f;

        [Header("Scene Transition")]
        [SerializeField] private string firstSceneName = "TutorialPath";
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;

        private EventSystem eventSystem;

        private void Start()
        {
            eventSystem = EventSystem.current;
            
            SetupButtons();
            ShowMainMenu();
            
            if (startButton != null)
            {
                eventSystem.SetSelectedGameObject(startButton.gameObject);
            }
        }

        private void SetupButtons()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartGame);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnShowSettings);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitGame);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackToMainMenu);
        }

        private void OnStartGame()
        {
            StartCoroutine(StartGameSequence());
        }

        private System.Collections.IEnumerator StartGameSequence()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            if (bookObject != null && bookAnimator != null)
            {
                bookObject.SetActive(true);
                bookAnimator.SetTrigger("OpenBook");
                yield return new WaitForSeconds(bookAnimationDuration);
            }

            if (fadeCanvasGroup != null)
            {
                float elapsedTime = 0f;
                while (elapsedTime < fadeDuration)
                {
                    fadeCanvasGroup.alpha = elapsedTime / fadeDuration;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                fadeCanvasGroup.alpha = 1f;
            }

            GameManager.Instance.LoadScene(firstSceneName);
        }

        private void OnShowSettings()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
            
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                
                Button firstButton = settingsPanel.GetComponentInChildren<Button>();
                if (firstButton != null)
                {
                    eventSystem.SetSelectedGameObject(firstButton.gameObject);
                }
            }
        }

        private void OnBackToMainMenu()
        {
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                
                if (startButton != null)
                {
                    eventSystem.SetSelectedGameObject(startButton.gameObject);
                }
            }
        }

        private void OnQuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        private void OnDestroy()
        {
            if (startButton != null)
                startButton.onClick.RemoveListener(OnStartGame);
            
            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OnShowSettings);
            
            if (quitButton != null)
                quitButton.onClick.RemoveListener(OnQuitGame);
            
            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackToMainMenu);
        }
    }
}