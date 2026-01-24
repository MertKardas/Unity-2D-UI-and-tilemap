using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
    public class PlayerSaveData
    {
        public StateData StateData;
        public Vector2 position = new Vector2();
        //Movement

        public Vector2 velocity = new Vector2();
        public float MaxSpeed;
        public float acceleration;
        public float deceleration;
        //Health
        public int health;
        public int maxHealth;

        public int coin;
        public float damage;
        public List<InventoryItem> items;
    }