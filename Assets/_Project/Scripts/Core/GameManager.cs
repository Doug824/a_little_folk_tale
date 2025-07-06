using UnityEngine;
using UnityEngine.SceneManagement;

namespace ALittleFolkTale.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Game State")]
        [SerializeField] private GameState currentGameState;
        [SerializeField] private bool isPaused = false;

        [Header("Player Reference")]
        [SerializeField] private GameObject playerPrefab;
        private GameObject currentPlayer;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            OnSceneLoaded();
        }

        private void OnSceneLoaded()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            if (playerPrefab != null && currentPlayer == null)
            {
                GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
                Vector3 spawnPosition = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
                
                currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                currentPlayer.name = "Player";
            }
        }

        public void PauseGame(bool pause)
        {
            isPaused = pause;
            Time.timeScale = pause ? 0f : 1f;
        }

        public void SaveGame()
        {
            Debug.Log("Game Saved!");
        }

        public void LoadGame()
        {
            Debug.Log("Game Loaded!");
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    [System.Serializable]
    public class GameState
    {
        public string currentScene;
        public Vector3 playerPosition;
        public int playerHealth;
        public int playerStamina;
    }
}