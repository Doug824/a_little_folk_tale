using UnityEngine;

namespace ALittleFolkTale.Combat
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "A Little Folk Tale/Combat/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        public string enemyName;
        public EnemyType enemyType;
        public Sprite portrait;

        [Header("Stats")]
        public int maxHealth = 50;
        public int damage = 10;
        public float moveSpeed = 3f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.5f;
        public float detectionRange = 10f;

        [Header("Behavior")]
        public AIBehaviorType behaviorType;
        public float patrolRadius = 5f;
        public float chaseDistance = 15f;

        [Header("Loot")]
        public LootTable lootTable;
        public int experienceReward = 10;

        [Header("Visual")]
        public GameObject modelPrefab;
        public RuntimeAnimatorController animatorController;

        [Header("Audio")]
        public AudioClip[] attackSounds;
        public AudioClip[] hurtSounds;
        public AudioClip deathSound;
    }

    public enum EnemyType
    {
        Melee,
        Ranged,
        Flying,
        Boss
    }

    public enum AIBehaviorType
    {
        Passive,
        Aggressive,
        Defensive,
        Patrol
    }

    [System.Serializable]
    public class LootTable
    {
        public LootItem[] possibleLoot;

        [System.Serializable]
        public class LootItem
        {
            public Items.ItemData item;
            [Range(0f, 100f)]
            public float dropChance;
            public int minQuantity = 1;
            public int maxQuantity = 1;
        }
    }
}