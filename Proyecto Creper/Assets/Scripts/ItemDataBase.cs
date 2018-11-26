using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;                            // Reference to the database for global use.

    private void Awake()
    {
        // Assign the reference to this instance;
        instance = this;
    }

    // Base class for items.
    public class Item
    {
        public enum ItemColors                                      // Possible colors of the item.
        { White, Red, Green, Blue, Yellow, Purple, Cyan };
        public ItemColors itemColor;                                // Actual color o fthe item.
        public Sprite sprite;                                       // Sprite to represent the item.
        public string name;                                         // Name of the item.        
        public int tier;                                            // Tier of the item.
        public string info;                                         // Info of the item.
        public float weight;                                        // Weight added when equipped.
        public float physical;                                      // Physical added when equipped.
        public float mental;                                        // Mental added when equipped.
        public float heat;                                          // Heat added when equipped.
        public float cold;                                          // Cold added when equipped.
        public float toxin;                                         // Toxin added when equipped.
        public float electricity;                                   // Electricity added when equipped.
        public float balance;                                       // Balance added when equipped.
    };

    // Base class for weapons.
    public class Weapon : Item
    {
        public enum WeaponType { None, Sword, Dagger, Shield, Pistol};
    };

    [System.Serializable]
    public class Sword : Weapon
    {
        public WeaponType type;
        public Sprite F_Weapon;
    };

    [System.Serializable]
    public class Dagger : Weapon
    {
        public WeaponType type;
        public Sprite F_Weapon;
    };

    [System.Serializable]
    public class Shield : Weapon
    {
        public WeaponType type;
        public Sprite B_Weapon;
    };

    [System.Serializable]
    public class Pistol : Weapon
    {
        public WeaponType type;
        public Sprite B_Weapon;
    };

    // Base class for armors.
    public class Armor : Item
    {
        public enum ArmorType { None, Light, Medium, Heavy };
    };

    [System.Serializable]
    public class Head : Armor
    {
        public ArmorType type;                                      // The type of the armor.
        public Sprite C_Neck;                                       // Sprite to equip in the character neck.
        public Sprite C_Head;                                       // Sprite to equip in the character head.
    };

    [System.Serializable]
    public class Chest : Armor
    {
        public ArmorType type;                                      // The type of the armor.
        public Sprite C_Torso_02;                                   // Sprite to equip in the character middle torso.
        public Sprite C_Torso_03;                                   // Sprite to equip in the character upper torso.
        public Sprite F_Arm_01;                                     // Sprite to equip in the character front upper arm.
        public Sprite B_Arm_01;                                     // Sprite to equip in the character back upper arm.
    };

    [System.Serializable]
    public class Hands : Armor
    {
        public ArmorType type;                                      // The type of the armor.
        public Sprite F_Arm_02;                                     // Sprite to equip in the character front lower arm.
        public Sprite F_Hand;                                       // Sprite to equip in the character front hand.
        public Sprite B_Arm_02;                                     // Sprite to equip in the character back lower arm.
        public Sprite B_Hand;                                       // Sprite to equip in the character back hand.
    };

    [System.Serializable]
    public class Legs : Armor
    {
        public ArmorType type;                                      // The type of the armor.
        public Sprite C_Torso_01;                                   // Sprite to equip in the character lower torso.
        public Sprite F_Leg_01;                                     // Sprite to equip in the character front upper lef.
        public Sprite F_Leg_02;                                     // Sprite to equip in the character front lower leg.
        public Sprite F_Feet_01;                                    // Sprite to equip in the character front upper feet.
        public Sprite F_Feet_02;                                    // Sprite to equip in the character front lower feet.
        public Sprite B_Leg_01;                                     // Sprite to equip in the character back upper leg.
        public Sprite B_Leg_02;                                     // Sprite to equip in the character back lower leg.
        public Sprite B_Feet_01;                                    // Sprite to equip in the character back upper feet.
        public Sprite B_Feet_02;                                    // Sprite to equip in the character back lower feet.
    };

    [System.Serializable]
    public class Rarity
    {
        public Color color;                                         // Color of rarity to display.
        public string name;                                         // Name of rarity to display
        [Range(0f, 1f)]
        public float chance;                                        // Chance to get this rarity.
    };

    [System.Serializable]
    public class Modification
    {
        public enum Stats
        { Weight, Physical, Mental, Heat, Cold, Toxic, Electricity, Balance };

        public string name;                                         // Name of the mod to display.
        // Actual stats the mod provides.
        public float weight;
        public float physical;
        public float mental;
        public float heat;
        public float cold;
        public float toxic;
        public float electricity;
        public float balance;
    };

    [Header("Item Colors")]
    public Color whiteItem;                                         // Color applied to item color white.
    public Color redItem;                                           // Color applied to item color red.
    public Color greenItem;                                         // Color applied to item color green.
    public Color blueItem;                                          // Color applied to item color blue.
    public Color yellowItem;                                        // Color applied to item color yellow.
    public Color purpleItem;                                        // Color applied to item color purple.
    public Color cyanItem;                                          // Color applied to item color cyan.

    [Header("Preview Colors")]
    public Color better;                                            // Color applied to better preview stats.
    public Color worse;                                             // Color applied to worse preview stats.    

    [Header("Items")]
    public Head[] heads;                                            // All the heads.    
    public Chest[] chests;                                          // All the chests.    
    public Hands[] hands;                                           // All the hands.    
    public Legs[] legs;                                             // All the legs.
    public Sword[] swords;                                          // all the swords.
    public Dagger[] daggers;                                        // all the swords.
    public Shield[] shields;                                        // all the shields.
    public Pistol[] pistols;                                        // all the pistols.

    [Header("Rarities")]
    public Rarity[] rarities;                                       // All the rarities.

    [Header("Modifications")]
    public Modification baseMod;                                    // A base mod representing no modification.
    [Space]
    public int minR0Mods;
    public int maxR0Mods;
    public int minR1Mods;
    public int maxR1Mods;
    public int minR2Mods;
    public int maxR2Mods;
    [Range(0, 1)]
    public float extraModChance;
    [Space]
    public float t1MinWeightMult;
    public float t1MaxWeightMult;
    public float t1MinPhysicalMult;
    public float t1MaxPhysicalMult;
    public float t1MinMentalMult;
    public float t1MaxMentalMult;
    public float t1MinHeatMult;
    public float t1MaxHeatMult;
    public float t1MinColdMult;
    public float t1MaxColdMult;
    public float t1MinToxicMult;
    public float t1MaxToxicMult;
    public float t1MinElectricalMult;
    public float t1MaxElectricalMult;
    public float t1MinBalanceMult;
    public float t1MaxBalanceMult;
}

