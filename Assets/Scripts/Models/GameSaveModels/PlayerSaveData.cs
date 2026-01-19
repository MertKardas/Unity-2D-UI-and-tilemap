using System;
using System.Collections.Generic;
[Serializable]
    public class PlayerSaveData
    {
        public StateData StateData;
        public float[] position = new float[2];
        //Movement

        public float[] velocity = new float[2];
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