using UnityEngine;
using UnityEngine.InputSystem;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.Core
{
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Test Scene Setup")]
        [SerializeField] private bool autoSetup = true;
        [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 1, 0);
        [SerializeField] private Vector3 groundScale = new Vector3(20, 1, 20);

        private void Start()
        {
            if (autoSetup)
            {
                SetupTestScene();
            }
        }

        public void SetupTestScene()
        {
            CreateGround();
            CreatePlayer();
            CreateTestEnemies();
            CreateWalls();
            SetupCamera();
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = groundScale;
            ground.tag = "Ground";
            
            // Add a simple material color
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.3f, 0.8f, 0.3f); // Green ground
        }

        private void CreatePlayer()
        {
            // Create player GameObject
            GameObject player = new GameObject("Player");
            player.transform.position = playerSpawnPosition;
            player.tag = "Player";

            // Add visual representation (capsule)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "PlayerVisual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.GetComponent<Renderer>().material.color = Color.blue;
            
            // Remove the collider from visual (we'll use CharacterController)
            Destroy(visual.GetComponent<Collider>());

            // Add CharacterController
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.center = new Vector3(0, 1, 0);
            controller.height = 2;
            controller.radius = 0.5f;

            // Add PlayerInput component
            PlayerInput playerInput = player.AddComponent<PlayerInput>();
            
            // Add PlayerController
            PlayerController playerController = player.AddComponent<PlayerController>();

            Debug.Log("Player created successfully!");
        }

        private void CreateTestEnemies()
        {
            // Create a few test enemies
            for (int i = 0; i < 3; i++)
            {
                GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
                enemy.name = $"TestEnemy_{i}";
                enemy.tag = "Enemy";
                enemy.transform.position = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));
                enemy.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        private void CreateWalls()
        {
            // Create boundary walls
            Vector3[] wallPositions = {
                new Vector3(0, 2, 10),   // North wall
                new Vector3(0, 2, -10),  // South wall
                new Vector3(10, 2, 0),   // East wall
                new Vector3(-10, 2, 0)   // West wall
            };

            Vector3[] wallScales = {
                new Vector3(20, 4, 1),   // North wall
                new Vector3(20, 4, 1),   // South wall
                new Vector3(1, 4, 20),   // East wall
                new Vector3(1, 4, 20)    // West wall
            };

            for (int i = 0; i < wallPositions.Length; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i}";
                wall.transform.position = wallPositions[i];
                wall.transform.localScale = wallScales[i];
                wall.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                wall.tag = "Wall";
            }
        }

        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Set camera for top-down view (Diablo style)
                mainCamera.transform.position = new Vector3(0, 12, -8);
                mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
                
                // Add camera controller
                if (mainCamera.GetComponent<ALittleFolkTale.Core.CameraController>() == null)
                {
                    mainCamera.gameObject.AddComponent<ALittleFolkTale.Core.CameraController>();
                }
            }
        }

        // Helper method to quickly test in editor
        [ContextMenu("Setup Test Scene")]
        public void SetupTestSceneFromEditor()
        {
            SetupTestScene();
        }
    }
}