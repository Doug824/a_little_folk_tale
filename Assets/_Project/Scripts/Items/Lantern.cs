using UnityEngine;
using ALittleFolkTale.Characters;
using ALittleFolkTale.Core;

namespace ALittleFolkTale.Items
{
    public class Lantern : MonoBehaviour, IInteractable
    {
        [Header("Lantern Settings")]
        [SerializeField] private bool isLit = false;
        [SerializeField] private string lanternID;
        
        [Header("Visual")]
        [SerializeField] private GameObject lightObject;
        [SerializeField] private ParticleSystem glowParticles;
        [SerializeField] private Material litMaterial;
        [SerializeField] private Material unlitMaterial;
        
        [Header("Save Settings")]
        [SerializeField] private bool isSavePoint = true;
        
        private MeshRenderer meshRenderer;
        
        private void Awake()
        {
            if (string.IsNullOrEmpty(lanternID))
            {
                lanternID = $"Lantern_{transform.position.x}_{transform.position.z}";
            }
            
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        private void Start()
        {
            UpdateVisuals();
        }
        
        public void Interact(PlayerController player)
        {
            if (!isLit)
            {
                LightLantern();
                
                if (isSavePoint)
                {
                    SaveGame();
                }
            }
        }
        
        private void LightLantern()
        {
            isLit = true;
            UpdateVisuals();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("LanternLight");
            }
        }
        
        private void UpdateVisuals()
        {
            if (lightObject != null)
                lightObject.SetActive(isLit);
                
            if (glowParticles != null)
            {
                if (isLit)
                    glowParticles.Play();
                else
                    glowParticles.Stop();
            }
            
            if (meshRenderer != null)
            {
                meshRenderer.material = isLit ? litMaterial : unlitMaterial;
            }
        }
        
        private void SaveGame()
        {
            Debug.Log($"Game saved at lantern: {lanternID}");
            GameManager.Instance.SaveGame();
        }
        
        public void SetLitState(bool lit)
        {
            isLit = lit;
            UpdateVisuals();
        }
        
        public string GetID()
        {
            return lanternID;
        }
    }
}