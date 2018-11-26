using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Light2D;
using System.Collections;

public class PlayerInventory : MonoBehaviour
{    
    [Header("Stats")]
    // Stats of the player.   
    public float maxWeight = 100;
    public float weight;
    [Space]
    // Defense.
    public float maxPhysical = 100;
    public float physical;
    public float maxMental = 100;
    public float mental;
    public float maxHeat = 100;
    public float heat;
    public float maxCold = 100;
    public float cold;
    public float maxToxin = 100;
    public float toxin;
    public float maxElectricity = 100;
    public float electricity;
    public float maxBalance;
    public float balance;
    [Space]
    // Main Hand.
    public float weightMH;
    public float physicalMH;
    public float mentalMH;
    public float heatMH;
    public float coldMH;
    public float toxinMH;
    public float electricityMH;
    public float balanceMH;
    // Off Hand.
    public float weightOH;
    public float physicalOH;
    public float mentalOH;
    public float heatOH;
    public float coldOH;
    public float toxinOH;
    public float electricityOH;
    public float balanceOH;

    [Header("Will")]
    public Color willColor;                                             // Actual color of will.
    private Color previousWillColor;                                    // Previous color of will.
    private float colorLerp;                                            // Aux variable for will color lerps.
    public class EquippedColors
    {
        // Amount of items of each color.
        public int whiteItems;
        public int redItems;
        public int greenItems;
        public int blueItems;
        public int yellowItems;
        public int purpleItems;
        public int cyanItems;

        public enum ColorCombinations { RGB, R, G, B, RG, RB, GB };
        public ColorCombinations maxColor;

        // Sets every color to zero.
        public void ResetColors()
        {
            whiteItems = 0;
            redItems = 0;
            greenItems = 0;
            blueItems = 0;
            yellowItems = 0;
            purpleItems = 0;
            cyanItems = 0;
        }

        // Increments the amount of a color.
        public void AddColor(ItemDataBase.Item.ItemColors itemColor)
        {
            switch (itemColor)
            {
                case ItemDataBase.Item.ItemColors.White:
                    whiteItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Red:
                    redItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Green:
                    greenItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Blue:
                    blueItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Yellow:
                    yellowItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Purple:
                    purpleItems++;
                    break;
                case ItemDataBase.Item.ItemColors.Cyan:
                    cyanItems++;
                    break;
            }
        }

        // Sets a color based on the amount of colors equipped.
        public Color GetColorMix()
        {
            // Pprepare a color to mix.
            Color ret = Color.black;

            // Mix the colors.
            ret.r = 0.333f + (0.083f * redItems) + (0.083f * yellowItems) + (0.083f * purpleItems) + (0.083f * whiteItems);
            ret.g = 0.333f + (0.083f * greenItems) + (0.083f * yellowItems) + (0.083f * cyanItems) + (0.083f * whiteItems);
            ret.b = 0.333f + (0.083f * blueItems) + (0.083f * purpleItems) + (0.083f * cyanItems) + (0.083f * whiteItems);

            // Set max color.
            if (ret.r == ret.g && ret.g == ret.b)
                maxColor = ColorCombinations.RGB;
            else if (ret.r > ret.g && ret.r > ret.b)
                maxColor = ColorCombinations.R;
            else if (ret.g > ret.r && ret.g > ret.b)
                maxColor = ColorCombinations.G;
            else if (ret.b > ret.r && ret.b > ret.g)
                maxColor = ColorCombinations.B;
            else if (ret.r == ret.g && ret.r > ret.b)
                maxColor = ColorCombinations.RG;
            else if (ret.r == ret.b && ret.r > ret.g)
                maxColor = ColorCombinations.RB;
            else if (ret.g == ret.b && ret.g > ret.r)
                maxColor = ColorCombinations.GB;

            // Adjust maximum values to not lose brightness.
            switch (maxColor)
            {
                case ColorCombinations.RGB:
                    ret.r = 0.666f;
                    ret.g = 0.666f;
                    ret.b = 0.666f;
                    break;
                case ColorCombinations.R:
                    ret.r = 0.666f;
                    break;
                case ColorCombinations.G:
                    ret.g = 0.666f;
                    break;
                case ColorCombinations.B:
                    ret.b = 0.666f;
                    break;
                case ColorCombinations.RG:
                    ret.r = 0.666f;
                    ret.g = 0.666f;
                    break;
                case ColorCombinations.RB:
                    ret.r = 0.666f;
                    ret.b = 0.666f;
                    break;
                case ColorCombinations.GB:
                    ret.g = 0.666f;
                    ret.b = 0.666f;
                    break;
            }

            // Return the mix.
            return ret;
        }
    };
    public EquippedColors equippedColors = new EquippedColors();
    [Space]
    public LightSprite playerLight;                                     // The light of the player.
    public float minGlow = 25f;                                         // The minimum scale to glow down.
    public float maxGlow = 30f;                                         // The maximum scale to glow up.
    public float glowSpeed = 5f;                                        // The speed of the glow transition.
    private bool glowUp;                                                // The glow direction.
    [Space]
    // UI elements that change color with will color.
    public Image willHUDCounter;                                        
    public Image willHUDBar;
    public Image willPoints;
    public Image willIcon;

    public enum Slots                                                   // To manage individual slots.
    { Head, Chest, Hands, Legs, MainHand, OffHand };

    [Header("Armor")]
    public ItemDataBase.Armor.ArmorType headType;                       // Type of the head armor.
    public int headIndex = 0;                                           // What head the player has equiped.
    public ItemDataBase.Rarity headRarity;                              // Rarity of the head item.
    public ItemDataBase.Modification headMod;                           // Mod of the head item.
    // Sprites of the head item.
    public SpriteRenderer C_Neck;
    public SpriteRenderer C_Head;
    [Space]
    public ItemDataBase.Armor.ArmorType chestType;                      // Type of the chest armor.
    public int chestIndex = 0;                                          // What chest the player has equiped.
    public ItemDataBase.Rarity chestRarity;                             // Rarity of the chest item.
    public ItemDataBase.Modification chestMod;                          // Mod of the chest item.
    // Sprites of the chest item.
    public SpriteRenderer C_Torso_02;
    public SpriteRenderer C_Torso_03;
    public SpriteRenderer F_Arm_01;
    public SpriteRenderer B_Arm_01;
    [Space]
    public ItemDataBase.Armor.ArmorType handsType;                      // Type of the hands armor type.
    public int handsIndex = 0;                                          // What hands the player has equiped.
    public ItemDataBase.Rarity handsRarity;                             // Rarity of the hands item.
    public ItemDataBase.Modification handsMod;                          // Mod of the hands item.
    // Sprites of the hands item.
    public SpriteRenderer F_Arm_02;
    public SpriteRenderer F_Hand;
    public SpriteRenderer B_Arm_02;
    public SpriteRenderer B_Hand;
    [Space]
    public ItemDataBase.Armor.ArmorType legsType;                       // Type of the legs armor type.
    public int legsIndex = 0;                                           // What legs the player has equiped.
    public ItemDataBase.Rarity legsRarity;                              // Rarity of the legs item.
    public ItemDataBase.Modification legsMod;                           // Mod of the legs item.
    // Sprites of the legs item.
    public SpriteRenderer C_Torso_01;
    public SpriteRenderer F_Leg_01;
    public SpriteRenderer F_Leg_02;
    public SpriteRenderer F_Feet_01;
    public SpriteRenderer F_Feet_02;
    public SpriteRenderer B_Leg_01;
    public SpriteRenderer B_Leg_02;
    public SpriteRenderer B_Feet_01;
    public SpriteRenderer B_Feet_02;

    [Header("Weapons")]
    public ItemDataBase.Weapon.WeaponType mainHandType;                 // Type of the main hand weapon.
    public int mainHandIndex = 0;                                       // What weapon the player has equiped in the main hand.
    public ItemDataBase.Rarity mainHandRarity;                          // Rarity of the main hand item.
    public ItemDataBase.Modification mainHandMod;                       // Mod of the main hand item.
    // Sprites of the main hand item.
    public SpriteRenderer F_Weapon;
    [Space]
    public ItemDataBase.Weapon.WeaponType offHandType;                  // Type of the off hand weapon.
    public int offHandIndex = 0;                                        // What weapon the player has equiped in the off hand.
    public ItemDataBase.Rarity offHandRarity;                           // Rarity of the off hand item.
    public ItemDataBase.Modification offHandMod;                        // Mod of the off hand item.
    // Sprites of the off hand item.
    public SpriteRenderer B_Weapon;

    [Header("Menu")]
    public Canvas inventory;                                            // The canvas used for rendering the inventory.
    // Parts of the inventory to select.
    public GameObject stats;
    public GameObject headSlot;
    public GameObject chestSlot;
    public GameObject handsSlot;
    public GameObject legsSlot;
    public GameObject mainHandSlot;
    public GameObject offHandSlot;

    [Header("Toggles")]
    // Parts of the selections to toggle.    
    public GameObject weapons;
    public GameObject dmgStats;
    public GameObject defStats;
    public GameObject armor;
    public Text equipmentAction;
    public GameObject mainHandStats;
    public GameObject mainHandInfo;
    public Text mainHandLAction;
    public GameObject offHandStats;
    public GameObject offHandInfo;
    public Text offHandLAction;
    public GameObject headStats;
    public GameObject headInfo;
    public Text headLAction;
    public GameObject chestStats;
    public GameObject chestInfo;
    public Text chestLAction;
    public GameObject handsStats;
    public GameObject handsInfo;
    public Text handsLAction;
    public GameObject legsStats;
    public GameObject legsInfo;
    public Text legsLAction;

    // To know wich icons to show.
    private bool controllerButtons;                                     

    [Header("Input Icons")]
    // Input icons to use.
    public GameObject equipmentMK;
    public GameObject equipmentCtrl;
    public GameObject mainHandMK;
    public GameObject mainHandCtrl;
    public GameObject offHandMK;
    public GameObject offHandCtrl;
    public GameObject headMK;
    public GameObject headCtrl;
    public GameObject chestMK;
    public GameObject chestCtrl;
    public GameObject handsMK;
    public GameObject handsCtrl;
    public GameObject legsMK;
    public GameObject legsCtrl;

    // Elements of the inventory to fill with stats.
    [Header("Menu Stats")]
    // Armor.
    public Text totalWeight;
    public Text totalWeightPercent;
    public Text totalPhysical;
    public Text totalPhysicalPercent;
    public Text totalMental;
    public Text totalMentalPercent;
    public Text totalHeat;
    public Text totalHeatPercent;
    public Text totalCold;
    public Text totalColdPercent;
    public Text totalToxin;
    public Text totalToxinPercent;
    public Text totalElectricity;
    public Text totalElectricityPercent;
    public Text totalBalance;
    public Text totalBalancePercent;
    // Weapons.
    public Text totalMHAttack;
    public Text totalMHEfficiency;
    public Text totalMHBalance;
    public Text totalOHAttack;
    public Text totalOHEfficiency;
    public Text totalOHBalance;
    [Space]
    public Image headColor;
    public Image headSprite;
    public Text headName;
    public Text headQuality;
    public Text headModification;
    public Text headInfoText;
    public Text headWeight;
    public Text headWeightMod;
    public Text headPhysical;
    public Text headPhysicalMod;
    public Text headMental;
    public Text headMentalMod;
    public Text headHeat;
    public Text headHeatMod;
    public Text headCold;
    public Text headColdMod;
    public Text headToxin;
    public Text headToxinMod;
    public Text headElectricity;
    public Text headElectricityMod;
    public Text headBalance;
    public Text headBalanceMod;
    [Space]
    public Image chestColor;
    public Image chestSprite;
    public Text chestName;
    public Text chestQuality;
    public Text chestModification;
    public Text chestInfoText;
    public Text chestWeight;
    public Text chestWeightMod;
    public Text chestPhysical;
    public Text chestPhysicalMod;
    public Text chestMental;
    public Text chestMentalMod;
    public Text chestHeat;
    public Text chestHeatMod;
    public Text chestCold;
    public Text chestColdMod;
    public Text chestToxin;
    public Text chestToxinMod;
    public Text chestElectricity;
    public Text chestElectricityMod;
    public Text chestBalance;
    public Text chestBalanceMod;
    [Space]
    public Image handsColor;
    public Image handsSprite;
    public Text handsName;
    public Text handsQuality;
    public Text handsModification;
    public Text handsInfoText;
    public Text handsWeight;
    public Text handsWeightMod;
    public Text handsPhysical;
    public Text handsPhysicalMod;
    public Text handsMental;
    public Text handsMentalMod;
    public Text handsHeat;
    public Text handsHeatMod;
    public Text handsCold;
    public Text handsColdMod;
    public Text handsToxin;
    public Text handsToxinMod;
    public Text handsElectricity;
    public Text handsElectricityMod;
    public Text handsBalance;
    public Text handsBalanceMod;
    [Space]
    public Image legsColor;
    public Image legsSprite;
    public Text legsName;
    public Text legsQuality;
    public Text legsModification;
    public Text legsInfoText;
    public Text legsWeight;
    public Text legsWeightMod;
    public Text legsPhysical;
    public Text legsPhysicalMod;
    public Text legsMental;
    public Text legsMentalMod;
    public Text legsHeat;
    public Text legsHeatMod;
    public Text legsCold;
    public Text legsColdMod;
    public Text legsToxin;
    public Text legsToxinMod;
    public Text legsElectricity;
    public Text legsElectricityMod;
    public Text legsBalance;
    public Text legsBalanceMod;
    [Space]
    public Image mainHandColor;
    public Image mainHandSprite;
    public Text mainHandName;
    public Text mainHandQuality;
    public Text mainHandModification;
    public Text mainHandInfoText;
    public Text mainHandWeight;
    public Text mainHandWeightMod;
    public Text mainHandPhysical;
    public Text mainHandPhysicalMod;
    public Text mainHandMental;
    public Text mainHandMentalMod;
    public Text mainHandHeat;
    public Text mainHandHeatMod;
    public Text mainHandCold;
    public Text mainHandColdMod;
    public Text mainHandToxin;
    public Text mainHandToxinMod;
    public Text mainHandElectricity;
    public Text mainHandElectricityMod;
    public Text mainHandBalance;
    public Text mainHandBalanceMod;
    [Space]
    public Image offHandColor;
    public Image offHandSprite;
    public Text offHandName;
    public Text offHandQuality;
    public Text offHandModification;
    public Text offHandInfoText;
    public Text offHandWeight;
    public Text offHandWeightMod;
    public Text offHandPhysical;
    public Text offHandPhysicalMod;
    public Text offHandMental;
    public Text offHandMentalMod;
    public Text offHandHeat;
    public Text offHandHeatMod;
    public Text offHandCold;
    public Text offHandColdMod;
    public Text offHandToxin;
    public Text offHandToxinMod;
    public Text offHandElectricity;
    public Text offHandElectricityMod;
    public Text offHandBalance;
    public Text offHandBalanceMod;

    [Header("Item Preview")]
    public ItemDrop itemPrefab;
    private ItemDrop itemPreview;
    [Space]
    public Canvas preview;
    public Image itemColor;
    public Image itemSprite;
    public Text itemName;
    public Text itemQuality;
    public Text itemModName;
    public Text itemPhysical;
    public Text itemPhysicalMod;
    public Text itemMental;
    public Text itemMentalMod;
    public Text itemHeat;
    public Text itemHeatMod;
    public Text itemCold;
    public Text itemColdMod;
    public Text itemToxin;
    public Text itemToxinMod;
    public Text itemElectricity;
    public Text itemElectricityMod;
    public Text itemWeight;
    public Text itemWeightMod;
    public Text itemBalance;
    public Text itemBalanceMod;
    [Space]
    public GameObject itemMK;
    public GameObject itemCtrl;

    // Methods called by the player.

    public void Enable()
    {
        // Hide the preview if its active.
        if(preview.enabled)
            preview.enabled = false;
        // Show the inventory.
        inventory.enabled = true;
        // Allow navigation.
        EventSystem.current.sendNavigationEvents = true;
    }

    public void Disable()
    {
        // Show the preview if there is an item to preview.
        if(itemPreview != null)
            preview.enabled = true;
        // Hide the inventory.
        inventory.enabled = false;
        // Prevent navigation.
        EventSystem.current.sendNavigationEvents = false;
    }    

    public void SetupInventary()
    {
        // Equip nothing in the slots.
        EquipItem(Slots.Head, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
        EquipItem(Slots.Chest, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
        EquipItem(Slots.Hands, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
        EquipItem(Slots.Legs, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
        EquipItem(Slots.MainHand, ItemDataBase.Weapon.WeaponType.Sword, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
        EquipItem(Slots.OffHand, ItemDataBase.Weapon.WeaponType.Shield, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);

        // Set the players will color.
        RefreshWillColor();

        // Disable the equipment slots.
        headSlot.SetActive(false);
        chestSlot.SetActive(false);
        handsSlot.SetActive(false);
        legsSlot.SetActive(false);
        mainHandSlot.SetActive(false);
        offHandSlot.SetActive(false);

        // Refresh the menu total stats.
        RefreshMenuTotalStats();

        // Clear the preview item stats.
        ClearPreview();
    }

    public void EquipItem(Slots slot, ItemDataBase.Weapon.WeaponType wType, int index, ItemDataBase.Rarity rarity, ItemDataBase.Modification modification)
    {
        // Remove the old stats.
        ChangeStats(false, slot, 0);

        // Drop old item, set new item.
        switch (slot)
        {
            case Slots.Head:
                // Drop the actual item.
                if(headIndex != 0)
                    SpawnActual(slot, 0, headIndex, headRarity, headMod);
                // Set the armor type.
                headType = ItemDataBase.instance.heads[index].type;
                // Set the new index.
                headIndex = index;
                // Set the new rarity.
                headRarity = rarity;
                // Set the new mod.
                headMod = modification;
                // Set the new sprites.
                C_Neck.sprite = ItemDataBase.instance.heads[headIndex].C_Neck;
                C_Head.sprite = ItemDataBase.instance.heads[headIndex].C_Head;
                // Enable the slot.
                headSlot.SetActive(true);
                break;
            case Slots.Chest:
                // Drop the actual item.
                if (chestIndex != 0)
                    SpawnActual(slot, 0, chestIndex, chestRarity, chestMod);
                // Set the armor type.
                chestType = ItemDataBase.instance.chests[index].type;
                // Set the new index.
                chestIndex = index;
                // Set the new rarity.
                chestRarity = rarity;
                // Set the new mod.
                chestMod = modification;
                // Set the new sprites.
                C_Torso_02.sprite = ItemDataBase.instance.chests[chestIndex].C_Torso_02;
                C_Torso_03.sprite = ItemDataBase.instance.chests[chestIndex].C_Torso_03;
                F_Arm_01.sprite = ItemDataBase.instance.chests[chestIndex].F_Arm_01;
                B_Arm_01.sprite = ItemDataBase.instance.chests[chestIndex].B_Arm_01;
                // Enable the slot.
                chestSlot.SetActive(true);
                break;
            case Slots.Hands:
                // Drop the actual item.
                if (handsIndex != 0)
                    SpawnActual(slot, 0, handsIndex, handsRarity, handsMod);
                // Set the armor type.
                handsType = ItemDataBase.instance.hands[index].type;
                // Set the new index.
                handsIndex = index;
                // Set the new rarity.
                handsRarity = rarity;
                // Set the new mod.
                handsMod = modification;
                // Set the new sprites.
                F_Arm_02.sprite = ItemDataBase.instance.hands[handsIndex].F_Arm_02;
                F_Hand.sprite = ItemDataBase.instance.hands[handsIndex].F_Hand;
                B_Arm_02.sprite = ItemDataBase.instance.hands[handsIndex].B_Arm_02;
                B_Hand.sprite = ItemDataBase.instance.hands[handsIndex].B_Hand;
                // Enable the slot.
                handsSlot.SetActive(true);
                break;
            case Slots.Legs:
                // Drop the actual item.
                if (legsIndex != 0)
                    SpawnActual(slot, 0, legsIndex, legsRarity, legsMod);
                // Set the armor type.
                legsType = ItemDataBase.instance.legs[index].type;
                // Set the new index.
                legsIndex = index;
                // Set the new rarity.
                legsRarity = rarity;
                // Set the new mod.
                legsMod = modification;
                // Set the new sprites.
                C_Torso_01.sprite = ItemDataBase.instance.legs[legsIndex].C_Torso_01;
                F_Leg_01.sprite = ItemDataBase.instance.legs[legsIndex].F_Leg_01;
                F_Leg_02.sprite = ItemDataBase.instance.legs[legsIndex].F_Leg_02;
                F_Feet_01.sprite = ItemDataBase.instance.legs[legsIndex].F_Feet_01;
                F_Feet_02.sprite = ItemDataBase.instance.legs[legsIndex].F_Feet_02;
                B_Leg_01.sprite = ItemDataBase.instance.legs[legsIndex].B_Leg_01;
                B_Leg_02.sprite = ItemDataBase.instance.legs[legsIndex].B_Leg_02;
                B_Feet_01.sprite = ItemDataBase.instance.legs[legsIndex].B_Feet_01;
                B_Feet_02.sprite = ItemDataBase.instance.legs[legsIndex].B_Feet_02;
                // Enable the slot.
                legsSlot.SetActive(true);
                break;
            case Slots.MainHand:
                // Drop the actual item.
                if (mainHandIndex != 0)
                    SpawnActual(slot, mainHandType, mainHandIndex, mainHandRarity, mainHandMod);
                // Set the new index.
                mainHandIndex = index;
                // Set the new rarity.
                mainHandRarity = rarity;
                // Set the new mod.
                mainHandMod = modification;
                // Use the appropiate weapon type.
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        // Set the weapon type.
                        mainHandType = ItemDataBase.instance.swords[mainHandIndex].type;                        
                        // Set the new sprites.
                        F_Weapon.sprite = ItemDataBase.instance.swords[mainHandIndex].F_Weapon;                        
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        // Set the weapon type.
                        mainHandType = ItemDataBase.instance.daggers[mainHandIndex].type;
                        // Set the new sprites.
                        F_Weapon.sprite = ItemDataBase.instance.daggers[mainHandIndex].F_Weapon;
                        break;
                }
                // Enable the slot.
                mainHandSlot.SetActive(true);
                break;
            case Slots.OffHand:
                // Drop the actual item.
                if (offHandIndex != 0)
                    SpawnActual(slot, offHandType, offHandIndex, offHandRarity, offHandMod);
                // Set the new index.
                offHandIndex = index;
                // Set the new rarity.
                offHandRarity = rarity;
                // Set the new mod.
                offHandMod = modification;
                // Use the appropiate weapon type.
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        // Set the weapon type.
                        offHandType = ItemDataBase.instance.shields[offHandIndex].type;
                        // Set the new sprites.
                        B_Weapon.sprite = ItemDataBase.instance.shields[offHandIndex].B_Weapon;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        // Set the weapon type.
                        offHandType = ItemDataBase.instance.pistols[offHandIndex].type;
                        // Set the new sprites.
                        B_Weapon.sprite = ItemDataBase.instance.pistols[offHandIndex].B_Weapon;
                        break;
                }
                // Enable the slot.
                offHandSlot.SetActive(true);
                break;
        }

        // Add the new stats.
        ChangeStats(true, slot, wType);

        // Refresh the menu info.       
        RefreshMenuSlotStats(slot, wType);

        // Refresh the menu total stats.
        RefreshMenuTotalStats();
    }

    public void MakeSelected(GameObject selection)
    {
        // Deselects the actual selected item to make this one the new selected item.
        EventSystem.current.SetSelectedGameObject(selection);
    }

    public void ShowActions(GameObject selection)
    {
        // Enables the actions object of an inventory slot.
        selection.SetActive(true);
    }

    public void HideActions(GameObject selection)
    {
        // Disable the actions object of an inventory slot.
        selection.SetActive(false);
    }

    public void Toggle(int slotIndex)
    {
        // Get the slot.
        Slots slot = (Slots)slotIndex;
        // Switch between showing the stats or the info of the slot.
        switch (slot)
        {
            case Slots.Head:
                if (headStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    headInfo.SetActive(true);
                    headStats.SetActive(false);
                    // Change toggle text.
                    headLAction.text = "Stats";
                }
                else if (headInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    headInfo.SetActive(false);
                    headStats.SetActive(true);
                    // Change toggle text.
                    headLAction.text = "Info";
                }
                break;
            case Slots.Chest:
                if (chestStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    chestInfo.SetActive(true);
                    chestStats.SetActive(false);
                    // Change toggle text.
                    chestLAction.text = "Stats";
                }
                else if (chestInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    chestInfo.SetActive(false);
                    chestStats.SetActive(true);
                    // Change toggle text.
                    chestLAction.text = "Info";
                }
                break;
            case Slots.Hands:
                if (handsStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    handsInfo.SetActive(true);
                    handsStats.SetActive(false);
                    // Change toggle text.
                    handsLAction.text = "Stats";
                }
                else if (handsInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    handsInfo.SetActive(false);
                    handsStats.SetActive(true);
                    // Change toggle text.
                    handsLAction.text = "Info";
                }
                break;
            case Slots.Legs:
                if (legsStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    legsInfo.SetActive(true);
                    legsStats.SetActive(false);
                    // Change toggle text.
                    legsLAction.text = "Stats";
                }
                else if (legsInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    legsInfo.SetActive(false);
                    legsStats.SetActive(true);
                    // Change toggle text.
                    legsLAction.text = "Info";
                }
                break;
            case Slots.MainHand:
                if (mainHandStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    mainHandInfo.SetActive(true);
                    mainHandStats.SetActive(false);
                    // Change toggle text.
                    mainHandLAction.text = "Stats";
                }
                else if (mainHandInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    mainHandInfo.SetActive(false);
                    mainHandStats.SetActive(true);
                    // Change toggle text.
                    mainHandLAction.text = "Info";
                }
                break;
            case Slots.OffHand:
                if (offHandStats.activeInHierarchy)
                {
                    // Hide stats and show info.
                    offHandInfo.SetActive(true);
                    offHandStats.SetActive(false);
                    // Change toggle text.
                    offHandLAction.text = "Stats";
                }
                else if (offHandInfo.activeInHierarchy)
                {
                    // Hide info and show stats.
                    offHandInfo.SetActive(false);
                    offHandStats.SetActive(true);
                    // Change toggle text.
                    offHandLAction.text = "Info";
                }
                break;
        }
    }

    public void Drop(int slotIndex)
    {
        // Get the slot.
        Slots slot = (Slots)slotIndex;
        // Reset the slot toggles and equip nothing in the slot.
        switch (slot)
        {
            case Slots.Head:
                headStats.SetActive(true);
                headInfo.SetActive(false);
                headInfoText.text = "Info";
                EquipItem(slot, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
            case Slots.Chest:
                chestStats.SetActive(true);
                chestInfo.SetActive(false);
                chestInfoText.text = "Info";
                EquipItem(slot, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
            case Slots.Hands:
                handsStats.SetActive(true);
                handsInfo.SetActive(false);
                handsInfoText.text = "Info";
                EquipItem(slot, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
            case Slots.Legs:
                legsStats.SetActive(true);
                legsInfo.SetActive(false);
                legsInfoText.text = "Info";
                EquipItem(slot, 0, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
            case Slots.MainHand:
                mainHandStats.SetActive(true);
                mainHandInfo.SetActive(false);
                mainHandInfoText.text = "Info";
                EquipItem(slot, ItemDataBase.Weapon.WeaponType.Sword, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
            case Slots.OffHand:
                offHandStats.SetActive(true);
                offHandInfo.SetActive(false);
                offHandInfoText.text = "Info";
                EquipItem(slot, ItemDataBase.Weapon.WeaponType.Shield, 0, ItemDataBase.instance.rarities[0], ItemDataBase.instance.baseMod);
                break;
        }                
        // Refresh the player will color.
        RefreshWillColor();
        // Deselects the inventory slot.
        EventSystem.current.SetSelectedGameObject(null);
        // Hides the inventory slot.        
        switch (slot)
        {
            case Slots.Head:
                headSlot.SetActive(false);
                break;
            case Slots.Chest:
                chestSlot.SetActive(false);
                break;
            case Slots.Hands:
                handsSlot.SetActive(false);
                break;
            case Slots.Legs:
                legsSlot.SetActive(false);
                break;
            case Slots.MainHand:
                mainHandSlot.SetActive(false);
                break;
            case Slots.OffHand:
                offHandSlot.SetActive(false);
                break;
        }
    }

    public void CheckMouseButtons(int slotIndex)
    {
        // Emulates toogle or drop with the mouse.
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Toggle(slotIndex);
        else if (Input.GetKeyDown(KeyCode.Mouse1))
            Drop(slotIndex);
    }

    public void ToggleEquipment()
    {
        if(weapons.activeInHierarchy)
        {
            weapons.SetActive(false);
            dmgStats.SetActive(false);
            armor.SetActive(true);
            defStats.SetActive(true);
            equipmentAction.text = "Weapons";
        }
        else if(armor.activeInHierarchy)
        {
            weapons.SetActive(true);
            dmgStats.SetActive(true);
            armor.SetActive(false);
            defStats.SetActive(false);
            equipmentAction.text = "Armor";
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    // Methods used by the inventory.    

    private void ChangeStats(bool sign, Slots slot, ItemDataBase.Weapon.WeaponType wType)
    {
        // Check for removal or addition.
        int value = sign ? 1 : -1;

        switch (slot)
        {
            case Slots.Head:       
                // Item Stats.
                weight += ItemDataBase.instance.heads[headIndex].weight * value;
                physical += ItemDataBase.instance.heads[headIndex].physical * value;
                mental += ItemDataBase.instance.heads[headIndex].mental * value;
                heat += ItemDataBase.instance.heads[headIndex].heat * value;
                cold += ItemDataBase.instance.heads[headIndex].cold * value;
                toxin += ItemDataBase.instance.heads[headIndex].toxin * value;
                electricity += ItemDataBase.instance.heads[headIndex].electricity * value;
                balance += ItemDataBase.instance.heads[headIndex].balance * value;
                maxBalance += ItemDataBase.instance.heads[headIndex].balance * value;
                // Mod stats.
                weight += sign ? (-1 * ItemDataBase.instance.heads[headIndex].weight * headMod.weight) : (ItemDataBase.instance.heads[headIndex].weight * headMod.weight);
                physical += ItemDataBase.instance.heads[headIndex].physical * headMod.physical * value;
                mental += ItemDataBase.instance.heads[headIndex].mental * headMod.mental * value;
                heat += ItemDataBase.instance.heads[headIndex].heat * headMod.heat * value;
                cold += ItemDataBase.instance.heads[headIndex].cold * headMod.cold * value;
                toxin += ItemDataBase.instance.heads[headIndex].toxin * headMod.toxic * value;
                electricity += ItemDataBase.instance.heads[headIndex].electricity * headMod.electricity * value;
                balance += ItemDataBase.instance.heads[headIndex].balance * headMod.balance * value;
                maxBalance += ItemDataBase.instance.heads[headIndex].balance * headMod.balance * value;
                break;
            case Slots.Chest:
                // Item stats.
                weight += ItemDataBase.instance.chests[chestIndex].weight * value;                
                physical += ItemDataBase.instance.chests[chestIndex].physical * value;
                mental += ItemDataBase.instance.chests[chestIndex].mental * value;
                heat += ItemDataBase.instance.chests[chestIndex].heat * value;
                cold += ItemDataBase.instance.chests[chestIndex].cold * value;
                toxin += ItemDataBase.instance.chests[chestIndex].toxin * value;
                electricity += ItemDataBase.instance.chests[chestIndex].electricity * value;
                balance += ItemDataBase.instance.chests[chestIndex].balance * value;
                maxBalance += ItemDataBase.instance.chests[chestIndex].balance * value;
                // Mod stats.
                weight += sign ? (-1 * ItemDataBase.instance.chests[chestIndex].weight * chestMod.weight) : (ItemDataBase.instance.chests[chestIndex].weight * chestMod.weight);
                physical += ItemDataBase.instance.chests[chestIndex].physical * chestMod.physical * value;
                mental += ItemDataBase.instance.chests[chestIndex].mental * chestMod.mental * value;
                heat += ItemDataBase.instance.chests[chestIndex].heat * chestMod.heat * value;
                cold += ItemDataBase.instance.chests[chestIndex].cold * chestMod.cold * value;
                toxin += ItemDataBase.instance.chests[chestIndex].toxin * chestMod.toxic * value;
                electricity += ItemDataBase.instance.chests[chestIndex].electricity* chestMod.electricity * value;
                balance += ItemDataBase.instance.chests[chestIndex].balance * chestMod.balance * value;
                maxBalance += ItemDataBase.instance.chests[chestIndex].balance * chestMod.balance * value;
                break;
            case Slots.Hands:
                // Item stats.
                weight += ItemDataBase.instance.hands[handsIndex].weight * value;
                physical += ItemDataBase.instance.hands[handsIndex].physical * value;
                mental += ItemDataBase.instance.hands[handsIndex].mental * value;
                heat += ItemDataBase.instance.hands[handsIndex].heat * value;
                cold += ItemDataBase.instance.hands[handsIndex].cold * value;
                toxin += ItemDataBase.instance.hands[handsIndex].toxin * value;
                electricity += ItemDataBase.instance.hands[handsIndex].electricity * value;
                balance += ItemDataBase.instance.hands[handsIndex].balance * value;
                maxBalance += ItemDataBase.instance.hands[handsIndex].balance * value;
                // Mod stats.
                weight += sign ? (-1 * ItemDataBase.instance.hands[handsIndex].weight * handsMod.weight) : (ItemDataBase.instance.hands[handsIndex].weight * handsMod.weight);
                physical += ItemDataBase.instance.hands[handsIndex].physical * handsMod.physical * value;
                mental += ItemDataBase.instance.hands[handsIndex].mental * handsMod.mental * value;
                heat += ItemDataBase.instance.hands[handsIndex].heat * handsMod.heat * value;
                cold += ItemDataBase.instance.hands[handsIndex].cold * handsMod.cold * value;
                toxin += ItemDataBase.instance.hands[handsIndex].toxin * handsMod.toxic * value;
                electricity += ItemDataBase.instance.hands[handsIndex].electricity * handsMod.electricity * value;
                balance += ItemDataBase.instance.hands[handsIndex].balance * handsMod.balance * value;
                maxBalance += ItemDataBase.instance.hands[handsIndex].balance * handsMod.balance * value;
                break;
            case Slots.Legs:
                // Item stats.
                weight += ItemDataBase.instance.legs[legsIndex].weight * value;
                physical += ItemDataBase.instance.legs[legsIndex].physical * value;
                mental += ItemDataBase.instance.legs[legsIndex].mental * value;
                heat += ItemDataBase.instance.legs[legsIndex].heat * value;
                cold += ItemDataBase.instance.legs[legsIndex].cold * value;
                toxin += ItemDataBase.instance.legs[legsIndex].toxin * value;
                electricity += ItemDataBase.instance.legs[legsIndex].electricity * value;
                balance += ItemDataBase.instance.legs[legsIndex].balance * value;
                maxBalance += ItemDataBase.instance.legs[legsIndex].balance * value;
                // Mod stats.
                weight += sign ? (-1 * ItemDataBase.instance.legs[legsIndex].weight * legsMod.weight) : (ItemDataBase.instance.legs[legsIndex].weight * legsMod.weight);
                physical += ItemDataBase.instance.legs[legsIndex].physical * legsMod.physical * value;
                mental += ItemDataBase.instance.legs[legsIndex].mental * legsMod.mental * value;
                heat += ItemDataBase.instance.legs[legsIndex].heat * legsMod.heat * value;
                cold += ItemDataBase.instance.legs[legsIndex].cold * legsMod.cold * value;
                toxin += ItemDataBase.instance.legs[legsIndex].toxin * legsMod.toxic * value;
                electricity += ItemDataBase.instance.legs[legsIndex].electricity * legsMod.electricity * value;
                balance += ItemDataBase.instance.legs[legsIndex].balance * legsMod.balance * value;
                maxBalance += ItemDataBase.instance.legs[legsIndex].balance * legsMod.balance * value;
                break;
            case Slots.MainHand:
                // Use the appropiate weapon type.
                switch(sign ? wType : mainHandType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        // Item stats.
                        weight += ItemDataBase.instance.swords[mainHandIndex].weight * value;
                        weightMH += ItemDataBase.instance.swords[mainHandIndex].weight * value;
                        physicalMH += ItemDataBase.instance.swords[mainHandIndex].physical * value;
                        mentalMH += ItemDataBase.instance.swords[mainHandIndex].mental * value;
                        heatMH += ItemDataBase.instance.swords[mainHandIndex].heat * value;
                        coldMH += ItemDataBase.instance.swords[mainHandIndex].cold * value;
                        toxinMH += ItemDataBase.instance.swords[mainHandIndex].toxin * value;
                        electricityMH += ItemDataBase.instance.swords[mainHandIndex].electricity * value;
                        balanceMH += ItemDataBase.instance.swords[mainHandIndex].balance * value;
                        // Mod stats.
                        weight += sign ? (-1 * ItemDataBase.instance.swords[mainHandIndex].weight * mainHandMod.weight) : (ItemDataBase.instance.swords[mainHandIndex].weight * mainHandMod.weight);
                        weightMH += sign ? (-1 * ItemDataBase.instance.swords[mainHandIndex].weight * mainHandMod.weight) : (ItemDataBase.instance.swords[mainHandIndex].weight * mainHandMod.weight);
                        physicalMH += ItemDataBase.instance.swords[mainHandIndex].physical * mainHandMod.physical * value;
                        mentalMH += ItemDataBase.instance.swords[mainHandIndex].mental * mainHandMod.mental * value;
                        heatMH += ItemDataBase.instance.swords[mainHandIndex].heat * mainHandMod.heat * value;
                        coldMH += ItemDataBase.instance.swords[mainHandIndex].cold * mainHandMod.cold * value;
                        toxinMH += ItemDataBase.instance.swords[mainHandIndex].toxin * mainHandMod.toxic * value;
                        electricityMH += ItemDataBase.instance.swords[mainHandIndex].electricity * mainHandMod.electricity * value;
                        balanceMH += ItemDataBase.instance.swords[mainHandIndex].balance * mainHandMod.balance * value;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        // Item stats.
                        weight += ItemDataBase.instance.daggers[mainHandIndex].weight * value;
                        weightMH += ItemDataBase.instance.daggers[mainHandIndex].weight * value;
                        physicalMH += ItemDataBase.instance.daggers[mainHandIndex].physical * value;
                        mentalMH += ItemDataBase.instance.daggers[mainHandIndex].mental * value;
                        heatMH += ItemDataBase.instance.daggers[mainHandIndex].heat * value;
                        coldMH += ItemDataBase.instance.daggers[mainHandIndex].cold * value;
                        toxinMH += ItemDataBase.instance.daggers[mainHandIndex].toxin * value;
                        electricityMH += ItemDataBase.instance.daggers[mainHandIndex].electricity * value;
                        balanceMH += ItemDataBase.instance.daggers[mainHandIndex].balance * value;
                        // Mod stats.
                        weight += sign ? (-1 * ItemDataBase.instance.daggers[mainHandIndex].weight * mainHandMod.weight) : (ItemDataBase.instance.daggers[mainHandIndex].weight * mainHandMod.weight);
                        weightMH += sign ? (-1 * ItemDataBase.instance.daggers[mainHandIndex].weight * mainHandMod.weight) : (ItemDataBase.instance.daggers[mainHandIndex].weight * mainHandMod.weight);
                        physicalMH += ItemDataBase.instance.daggers[mainHandIndex].physical * mainHandMod.physical * value;
                        mentalMH += ItemDataBase.instance.daggers[mainHandIndex].mental * mainHandMod.mental * value;
                        heatMH += ItemDataBase.instance.daggers[mainHandIndex].heat * mainHandMod.heat * value;
                        coldMH += ItemDataBase.instance.daggers[mainHandIndex].cold * mainHandMod.cold * value;
                        toxinMH += ItemDataBase.instance.daggers[mainHandIndex].toxin * mainHandMod.toxic * value;
                        electricityMH += ItemDataBase.instance.daggers[mainHandIndex].electricity * mainHandMod.electricity * value;
                        balanceMH += ItemDataBase.instance.daggers[mainHandIndex].balance * mainHandMod.balance * value;
                        break;
                }                
                break;
            case Slots.OffHand:
                // Use the appropiate weapon type.
                switch (sign ? wType : offHandType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        // Item stats.
                        weight += ItemDataBase.instance.shields[offHandIndex].weight * value;
                        weightOH += ItemDataBase.instance.shields[offHandIndex].weight * value;
                        physicalOH += ItemDataBase.instance.shields[offHandIndex].physical * value;
                        mentalOH += ItemDataBase.instance.shields[offHandIndex].mental * value;
                        heatOH += ItemDataBase.instance.shields[offHandIndex].heat * value;
                        coldOH += ItemDataBase.instance.shields[offHandIndex].cold * value;
                        toxinOH += ItemDataBase.instance.shields[offHandIndex].toxin * value;
                        electricityOH += ItemDataBase.instance.shields[offHandIndex].electricity * value;
                        balanceOH += ItemDataBase.instance.shields[offHandIndex].balance * value;
                        // Mod stats.
                        weight += sign ? (-1 * ItemDataBase.instance.shields[offHandIndex].weight * offHandMod.weight) : (ItemDataBase.instance.shields[offHandIndex].weight * offHandMod.weight);
                        weightOH += sign ? (-1 * ItemDataBase.instance.shields[offHandIndex].weight * offHandMod.weight) : (ItemDataBase.instance.shields[offHandIndex].weight * offHandMod.weight);
                        physicalOH += ItemDataBase.instance.shields[offHandIndex].physical * offHandMod.physical * value;
                        mentalOH += ItemDataBase.instance.shields[offHandIndex].mental * offHandMod.mental * value;
                        heatOH += ItemDataBase.instance.shields[offHandIndex].heat * offHandMod.heat * value;
                        coldOH += ItemDataBase.instance.shields[offHandIndex].cold * offHandMod.cold * value;
                        toxinOH += ItemDataBase.instance.shields[offHandIndex].toxin * offHandMod.toxic * value;
                        electricityOH += ItemDataBase.instance.shields[offHandIndex].electricity * offHandMod.electricity * value;
                        balanceOH += ItemDataBase.instance.shields[offHandIndex].balance * offHandMod.balance * value;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        // Item stats.
                        weight += ItemDataBase.instance.pistols[offHandIndex].weight * value;
                        weightOH += ItemDataBase.instance.pistols[offHandIndex].weight * value;
                        physicalOH += ItemDataBase.instance.pistols[offHandIndex].physical * value;
                        mentalOH += ItemDataBase.instance.pistols[offHandIndex].mental * value;
                        heatOH += ItemDataBase.instance.pistols[offHandIndex].heat * value;
                        coldOH += ItemDataBase.instance.pistols[offHandIndex].cold * value;
                        toxinOH += ItemDataBase.instance.pistols[offHandIndex].toxin * value;
                        electricityOH += ItemDataBase.instance.pistols[offHandIndex].electricity * value;
                        balanceOH += ItemDataBase.instance.pistols[offHandIndex].balance * value;
                        // Mod stats.
                        weight += sign ? (-1 * ItemDataBase.instance.pistols[offHandIndex].weight * offHandMod.weight) : (ItemDataBase.instance.pistols[offHandIndex].weight * offHandMod.weight);
                        weightOH += sign ? (-1 * ItemDataBase.instance.pistols[offHandIndex].weight * offHandMod.weight) : (ItemDataBase.instance.pistols[offHandIndex].weight * offHandMod.weight);
                        physicalOH += ItemDataBase.instance.pistols[offHandIndex].physical * offHandMod.physical * value;
                        mentalOH += ItemDataBase.instance.pistols[offHandIndex].mental * offHandMod.mental * value;
                        heatOH += ItemDataBase.instance.pistols[offHandIndex].heat * offHandMod.heat * value;
                        coldOH += ItemDataBase.instance.pistols[offHandIndex].cold * offHandMod.cold * value;
                        toxinOH += ItemDataBase.instance.pistols[offHandIndex].toxin * offHandMod.toxic * value;
                        electricityOH += ItemDataBase.instance.pistols[offHandIndex].electricity * offHandMod.electricity * value;
                        balanceOH += ItemDataBase.instance.pistols[offHandIndex].balance * offHandMod.balance * value;
                        break;
                }
                break;
        }
    }

    private void RefreshMenuSlotStats(Slots slot, ItemDataBase.Weapon.WeaponType wType)
    {
        switch (slot)
        {
            case Slots.Head:
                // Item stats.
                headColor.color = GetItemColor(ItemDataBase.instance.heads[headIndex].itemColor);
                headSprite.sprite = ItemDataBase.instance.heads[headIndex].sprite;
                headName.text = ItemDataBase.instance.heads[headIndex].name;
                GetQuality(slot, 0, headRarity);
                headModification.text = headMod.name;
                headWeight.text = ItemDataBase.instance.heads[headIndex].weight.ToString("0.0");
                headPhysical.text = ItemDataBase.instance.heads[headIndex].physical.ToString("0");
                headMental.text = ItemDataBase.instance.heads[headIndex].mental.ToString("0");
                headHeat.text = ItemDataBase.instance.heads[headIndex].heat.ToString("0");
                headCold.text = ItemDataBase.instance.heads[headIndex].cold.ToString("0");
                headToxin.text = ItemDataBase.instance.heads[headIndex].toxin.ToString("0");
                headElectricity.text = ItemDataBase.instance.heads[headIndex].electricity.ToString("0");
                headBalance.text = ItemDataBase.instance.heads[headIndex].balance.ToString("0");
                // Mod stats.
                ClearModStats(slot);
                SetModStat(ItemDataBase.instance.heads[headIndex].weight, headMod.weight, headWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].physical, headMod.physical, headPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].mental, headMod.mental, headMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].heat, headMod.heat, headHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].cold, headMod.cold, headColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].toxin, headMod.toxic, headToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].electricity, headMod.electricity, headElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[headIndex].balance, headMod.balance, headBalanceMod, false, false, false);
                ColourModStat(ItemDataBase.instance.heads[headIndex].weight, headMod.weight, headWeightMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].physical, headMod.physical, headPhysicalMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].mental, headMod.mental, headMentalMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].heat, headMod.heat, headHeatMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].cold, headMod.cold, headColdMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].toxin, headMod.toxic, headToxinMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].electricity, headMod.electricity, headElectricityMod);
                ColourModStat(ItemDataBase.instance.heads[headIndex].balance, headMod.balance, headBalanceMod);
                // Info.
                headInfoText.text = ItemDataBase.instance.heads[headIndex].info;
                // Show stats and hide info.
                headStats.SetActive(true);
                headInfo.SetActive(false);
                break;
            case Slots.Chest:
                // Item stats.
                chestColor.color = GetItemColor(ItemDataBase.instance.chests[chestIndex].itemColor);
                chestSprite.sprite = ItemDataBase.instance.chests[chestIndex].sprite;
                chestName.text = ItemDataBase.instance.chests[chestIndex].name;
                GetQuality(slot, 0, chestRarity);
                chestModification.text = chestMod.name;
                chestWeight.text = ItemDataBase.instance.chests[chestIndex].weight.ToString("0.0");
                chestPhysical.text = ItemDataBase.instance.chests[chestIndex].physical.ToString("0");
                chestMental.text = ItemDataBase.instance.chests[chestIndex].mental.ToString("0");
                chestHeat.text = ItemDataBase.instance.chests[chestIndex].heat.ToString("0");
                chestCold.text = ItemDataBase.instance.chests[chestIndex].cold.ToString("0");
                chestToxin.text = ItemDataBase.instance.chests[chestIndex].toxin.ToString("0");
                chestElectricity.text = ItemDataBase.instance.chests[chestIndex].electricity.ToString("0");
                chestBalance.text = ItemDataBase.instance.chests[chestIndex].balance.ToString("0");
                // Mod stats.
                ClearModStats(slot);
                SetModStat(ItemDataBase.instance.chests[chestIndex].weight, chestMod.weight, chestWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].physical, chestMod.physical, chestPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].mental, chestMod.mental, chestMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].heat, chestMod.heat, chestHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].cold, chestMod.cold, chestColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].toxin, chestMod.toxic, chestToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].electricity, chestMod.electricity, chestElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[chestIndex].balance, chestMod.balance, chestBalanceMod, false, false, false);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].weight, chestMod.weight, chestWeightMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].physical, chestMod.physical, chestPhysicalMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].mental, chestMod.mental, chestMentalMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].heat, chestMod.heat, chestHeatMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].cold, chestMod.cold, chestColdMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].toxin, chestMod.toxic, chestToxinMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].electricity, chestMod.electricity, chestElectricityMod);
                ColourModStat(ItemDataBase.instance.chests[chestIndex].balance, chestMod.balance, chestBalanceMod);
                // Info.
                chestInfoText.text = ItemDataBase.instance.chests[chestIndex].info;
                // Show stats and hide info.
                chestStats.SetActive(true);
                chestInfo.SetActive(false);
                break;
            case Slots.Hands:
                // Item stats.
                handsColor.color = GetItemColor(ItemDataBase.instance.hands[handsIndex].itemColor);
                handsSprite.sprite = ItemDataBase.instance.hands[handsIndex].sprite;
                handsName.text = ItemDataBase.instance.hands[handsIndex].name;
                GetQuality(slot, 0, handsRarity);
                handsModification.text = handsMod.name;
                handsWeight.text = ItemDataBase.instance.hands[handsIndex].weight.ToString("0.0");
                handsPhysical.text = ItemDataBase.instance.hands[handsIndex].physical.ToString("0");
                handsMental.text = ItemDataBase.instance.hands[handsIndex].mental.ToString("0");
                handsHeat.text = ItemDataBase.instance.hands[handsIndex].heat.ToString("0");
                handsCold.text = ItemDataBase.instance.hands[handsIndex].cold.ToString("0");
                handsToxin.text = ItemDataBase.instance.hands[handsIndex].toxin.ToString("0");
                handsElectricity.text = ItemDataBase.instance.hands[handsIndex].electricity.ToString("0");
                handsBalance.text = ItemDataBase.instance.hands[handsIndex].balance.ToString("0");
                // Mod stats.
                ClearModStats(slot);
                SetModStat(ItemDataBase.instance.hands[handsIndex].weight, handsMod.weight, handsWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].physical, handsMod.physical, handsPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].mental, handsMod.mental, handsMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].heat, handsMod.heat, handsHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].cold, handsMod.cold, handsColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].toxin, handsMod.toxic, handsToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].electricity, handsMod.electricity, handsElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[handsIndex].balance, handsMod.balance, handsBalanceMod, false, false, false);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].weight, handsMod.weight, handsWeightMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].physical, handsMod.physical, handsPhysicalMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].mental, handsMod.mental, handsMentalMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].heat, handsMod.heat, handsHeatMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].cold, handsMod.cold, handsColdMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].toxin, handsMod.toxic, handsToxinMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].electricity, handsMod.electricity, handsElectricityMod);
                ColourModStat(ItemDataBase.instance.hands[handsIndex].balance, handsMod.balance, handsBalanceMod);
                // Info.
                handsInfoText.text = ItemDataBase.instance.hands[handsIndex].info;
                // Show stats and hide info.
                handsStats.SetActive(true);
                handsInfo.SetActive(false);
                break;
            case Slots.Legs:
                // Item stats.
                legsColor.color = GetItemColor(ItemDataBase.instance.legs[legsIndex].itemColor);
                legsSprite.sprite = ItemDataBase.instance.legs[legsIndex].sprite;
                legsName.text = ItemDataBase.instance.legs[legsIndex].name;
                GetQuality(slot, 0, legsRarity);
                legsModification.text = legsMod.name;
                legsWeight.text = ItemDataBase.instance.legs[legsIndex].weight.ToString("0.0");
                legsPhysical.text = ItemDataBase.instance.legs[legsIndex].physical.ToString("0");
                legsMental.text = ItemDataBase.instance.legs[legsIndex].mental.ToString("0");
                legsHeat.text = ItemDataBase.instance.legs[legsIndex].heat.ToString("0");
                legsCold.text = ItemDataBase.instance.legs[legsIndex].cold.ToString("0");
                legsToxin.text = ItemDataBase.instance.legs[legsIndex].toxin.ToString("0");
                legsElectricity.text = ItemDataBase.instance.legs[legsIndex].electricity.ToString("0");
                legsBalance.text = ItemDataBase.instance.legs[legsIndex].balance.ToString("0");
                // Mod stats.
                ClearModStats(slot);
                SetModStat(ItemDataBase.instance.legs[legsIndex].weight, legsMod.weight, legsWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].physical, legsMod.physical, legsPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].mental, legsMod.mental, legsMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].heat, legsMod.heat, legsHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].cold, legsMod.cold, legsColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].toxin, legsMod.toxic, legsToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].electricity, legsMod.electricity, legsElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[legsIndex].balance, legsMod.balance, legsBalanceMod, false, false, false);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].weight, legsMod.weight, legsWeightMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].physical, legsMod.physical, legsPhysicalMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].mental, legsMod.mental, legsMentalMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].heat, legsMod.heat, legsHeatMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].cold, legsMod.cold, legsColdMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].toxin, legsMod.toxic, legsToxinMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].electricity, legsMod.electricity, legsElectricityMod);
                ColourModStat(ItemDataBase.instance.legs[legsIndex].balance, legsMod.balance, legsBalanceMod);
                // Info.
                legsInfoText.text = ItemDataBase.instance.legs[legsIndex].info;
                // Show stats and hide info.
                legsStats.SetActive(true);
                legsInfo.SetActive(false);
                break;
            case Slots.MainHand:
                // Use the appropiate weapon type.
                switch(wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        // Item stats.
                        mainHandColor.color = GetItemColor(ItemDataBase.instance.swords[mainHandIndex].itemColor);
                        mainHandSprite.sprite = ItemDataBase.instance.swords[mainHandIndex].sprite;
                        mainHandName.text = ItemDataBase.instance.swords[mainHandIndex].name;
                        GetQuality(slot, wType, mainHandRarity);
                        mainHandModification.text = mainHandMod.name;
                        mainHandWeight.text = ItemDataBase.instance.swords[mainHandIndex].weight.ToString("0.0");
                        mainHandPhysical.text = ItemDataBase.instance.swords[mainHandIndex].physical.ToString("0");
                        mainHandMental.text = ItemDataBase.instance.swords[mainHandIndex].mental.ToString("0") + "%";
                        mainHandHeat.text = ItemDataBase.instance.swords[mainHandIndex].heat.ToString("0");
                        mainHandCold.text = ItemDataBase.instance.swords[mainHandIndex].cold.ToString("0");
                        mainHandToxin.text = ItemDataBase.instance.swords[mainHandIndex].toxin.ToString("0");
                        mainHandElectricity.text = ItemDataBase.instance.swords[mainHandIndex].electricity.ToString("0");
                        mainHandBalance.text = ItemDataBase.instance.swords[mainHandIndex].balance.ToString("0");
                        // Mod stats.
                        ClearModStats(slot);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].weight, mainHandMod.weight, mainHandWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].physical, mainHandMod.physical, mainHandPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].mental, mainHandMod.mental, mainHandMentalMod, false, false, true);                        
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].heat, mainHandMod.heat, mainHandHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].cold, mainHandMod.cold, mainHandColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].toxin, mainHandMod.toxic, mainHandToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].electricity, mainHandMod.electricity, mainHandElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[mainHandIndex].balance, mainHandMod.balance, mainHandBalanceMod, false, false, false);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].weight, mainHandMod.weight, mainHandWeightMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].physical, mainHandMod.physical, mainHandPhysicalMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].mental, mainHandMod.mental, mainHandMentalMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].heat, mainHandMod.heat, mainHandHeatMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].cold, mainHandMod.cold, mainHandColdMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].toxin, mainHandMod.toxic, mainHandToxinMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].electricity, mainHandMod.electricity, mainHandElectricityMod);
                        ColourModStat(ItemDataBase.instance.swords[mainHandIndex].balance, mainHandMod.balance, mainHandBalanceMod);
                        // Info.
                        mainHandInfoText.text = ItemDataBase.instance.swords[mainHandIndex].info;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        // Item stats.
                        mainHandColor.color = GetItemColor(ItemDataBase.instance.daggers[mainHandIndex].itemColor);
                        mainHandSprite.sprite = ItemDataBase.instance.daggers[mainHandIndex].sprite;
                        mainHandName.text = ItemDataBase.instance.daggers[mainHandIndex].name;
                        GetQuality(slot, wType, mainHandRarity);
                        mainHandModification.text = mainHandMod.name;
                        mainHandWeight.text = ItemDataBase.instance.daggers[mainHandIndex].weight.ToString("0.0");
                        mainHandPhysical.text = ItemDataBase.instance.daggers[mainHandIndex].physical.ToString("0");
                        mainHandMental.text = ItemDataBase.instance.daggers[mainHandIndex].mental.ToString("0") + "%";
                        mainHandHeat.text = ItemDataBase.instance.daggers[mainHandIndex].heat.ToString("0");
                        mainHandCold.text = ItemDataBase.instance.daggers[mainHandIndex].cold.ToString("0");
                        mainHandToxin.text = ItemDataBase.instance.daggers[mainHandIndex].toxin.ToString("0");
                        mainHandElectricity.text = ItemDataBase.instance.daggers[mainHandIndex].electricity.ToString("0");
                        mainHandBalance.text = ItemDataBase.instance.daggers[mainHandIndex].balance.ToString("0");
                        // Mod stats.
                        ClearModStats(slot);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].weight, mainHandMod.weight, mainHandWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].physical, mainHandMod.physical, mainHandPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].mental, mainHandMod.mental, mainHandMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].heat, mainHandMod.heat, mainHandHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].cold, mainHandMod.cold, mainHandColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].toxin, mainHandMod.toxic, mainHandToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].electricity, mainHandMod.electricity, mainHandElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[mainHandIndex].balance, mainHandMod.balance, mainHandBalanceMod, false, false, false);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].weight, mainHandMod.weight, mainHandWeightMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].physical, mainHandMod.physical, mainHandPhysicalMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].mental, mainHandMod.mental, mainHandMentalMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].heat, mainHandMod.heat, mainHandHeatMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].cold, mainHandMod.cold, mainHandColdMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].toxin, mainHandMod.toxic, mainHandToxinMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].electricity, mainHandMod.electricity, mainHandElectricityMod);
                        ColourModStat(ItemDataBase.instance.daggers[mainHandIndex].balance, mainHandMod.balance, mainHandBalanceMod);
                        // Info.
                        mainHandInfoText.text = ItemDataBase.instance.daggers[mainHandIndex].info;
                        break;
                }                
                // Show stats and hide info.
                mainHandStats.SetActive(true);
                mainHandInfo.SetActive(false);
                break;
            case Slots.OffHand:
                // Use the appropiate weapon type.
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        // Item stats.
                        offHandColor.color = GetItemColor(ItemDataBase.instance.shields[offHandIndex].itemColor);
                        offHandSprite.sprite = ItemDataBase.instance.shields[offHandIndex].sprite;
                        offHandName.text = ItemDataBase.instance.shields[offHandIndex].name;
                        GetQuality(slot, wType, offHandRarity);
                        offHandModification.text = offHandMod.name;
                        offHandWeight.text = ItemDataBase.instance.shields[offHandIndex].weight.ToString("0.0");
                        offHandPhysical.text = ItemDataBase.instance.shields[offHandIndex].physical.ToString("0");
                        offHandMental.text = ItemDataBase.instance.shields[offHandIndex].mental.ToString("0") + "%";
                        offHandHeat.text = ItemDataBase.instance.shields[offHandIndex].heat.ToString("0");
                        offHandCold.text = ItemDataBase.instance.shields[offHandIndex].cold.ToString("0");
                        offHandToxin.text = ItemDataBase.instance.shields[offHandIndex].toxin.ToString("0");
                        offHandElectricity.text = ItemDataBase.instance.shields[offHandIndex].electricity.ToString("0");
                        offHandBalance.text = ItemDataBase.instance.shields[offHandIndex].balance.ToString("0");
                        // Mod stats.
                        ClearModStats(slot);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].weight, offHandMod.weight, offHandWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].physical, offHandMod.physical, offHandPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].mental, offHandMod.mental, offHandMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].heat, offHandMod.heat, offHandHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].cold, offHandMod.cold, offHandColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].toxin, offHandMod.toxic, offHandToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].electricity, offHandMod.electricity, offHandElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[offHandIndex].balance, offHandMod.balance, offHandBalanceMod, false, false, false);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].weight, offHandMod.weight, offHandWeightMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].physical, offHandMod.physical, offHandPhysicalMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].mental, offHandMod.mental, offHandMentalMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].heat, offHandMod.heat, offHandHeatMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].cold, offHandMod.cold, offHandColdMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].toxin, offHandMod.toxic, offHandToxinMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].electricity, offHandMod.electricity, offHandElectricityMod);
                        ColourModStat(ItemDataBase.instance.shields[offHandIndex].balance, offHandMod.balance, offHandBalanceMod);
                        // Info.
                        offHandInfoText.text = ItemDataBase.instance.shields[offHandIndex].info;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        // Item stats.
                        offHandColor.color = GetItemColor(ItemDataBase.instance.pistols[offHandIndex].itemColor);
                        offHandSprite.sprite = ItemDataBase.instance.pistols[offHandIndex].sprite;
                        offHandName.text = ItemDataBase.instance.pistols[offHandIndex].name;
                        GetQuality(slot, wType, offHandRarity);
                        offHandModification.text = offHandMod.name;
                        offHandWeight.text = ItemDataBase.instance.pistols[offHandIndex].weight.ToString("0.0");
                        offHandPhysical.text = ItemDataBase.instance.pistols[offHandIndex].physical.ToString("0");
                        offHandMental.text = ItemDataBase.instance.pistols[offHandIndex].mental.ToString("0") + "%";
                        offHandHeat.text = ItemDataBase.instance.pistols[offHandIndex].heat.ToString("0");
                        offHandCold.text = ItemDataBase.instance.pistols[offHandIndex].cold.ToString("0");
                        offHandToxin.text = ItemDataBase.instance.pistols[offHandIndex].toxin.ToString("0");
                        offHandElectricity.text = ItemDataBase.instance.pistols[offHandIndex].electricity.ToString("0");
                        offHandBalance.text = ItemDataBase.instance.pistols[offHandIndex].balance.ToString("0");
                        // Mod stats.
                        ClearModStats(slot);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].weight, offHandMod.weight, offHandWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].physical, offHandMod.physical, offHandPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].mental, offHandMod.mental, offHandMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].heat, offHandMod.heat, offHandHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].cold, offHandMod.cold, offHandColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].toxin, offHandMod.toxic, offHandToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].electricity, offHandMod.electricity, offHandElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[offHandIndex].balance, offHandMod.balance, offHandBalanceMod, false, false, false);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].weight, offHandMod.weight, offHandWeightMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].physical, offHandMod.physical, offHandPhysicalMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].mental, offHandMod.mental, offHandMentalMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].heat, offHandMod.heat, offHandHeatMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].cold, offHandMod.cold, offHandColdMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].toxin, offHandMod.toxic, offHandToxinMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].electricity, offHandMod.electricity, offHandElectricityMod);
                        ColourModStat(ItemDataBase.instance.pistols[offHandIndex].balance, offHandMod.balance, offHandBalanceMod);
                        // Info.
                        offHandInfoText.text = ItemDataBase.instance.pistols[offHandIndex].info;
                        break;
                }
                // Show stats and hide info.
                offHandStats.SetActive(true);
                offHandInfo.SetActive(false);
                break;
        }
    }

    private Color GetItemColor(ItemDataBase.Item.ItemColors itemColor)
    {
        Color ret = Color.black;
        // Get the color of the item.
        switch (itemColor)
        {
            case ItemDataBase.Item.ItemColors.White:
                ret = ItemDataBase.instance.whiteItem;
                break;
            case ItemDataBase.Item.ItemColors.Red:
                ret = ItemDataBase.instance.redItem;
                break;
            case ItemDataBase.Item.ItemColors.Green:
                ret = ItemDataBase.instance.greenItem;
                break;
            case ItemDataBase.Item.ItemColors.Blue:
                ret = ItemDataBase.instance.blueItem;
                break;
            case ItemDataBase.Item.ItemColors.Yellow:
                ret = ItemDataBase.instance.yellowItem;
                break;
            case ItemDataBase.Item.ItemColors.Purple:
                ret = ItemDataBase.instance.purpleItem;
                break;
            case ItemDataBase.Item.ItemColors.Cyan:
                ret = ItemDataBase.instance.cyanItem;
                break;
        }
        return ret;
    }

    private void GetQuality(Slots slot, ItemDataBase.Weapon.WeaponType wType, ItemDataBase.Rarity rarity)
    {
        // Set tier.
        string ret = "Tier ";
        switch(slot)
        {
            case Slots.Head:
                ret += ItemDataBase.instance.heads[headIndex].tier;
                break;
            case Slots.Chest:
                ret += ItemDataBase.instance.chests[chestIndex].tier;
                break;
            case Slots.Hands:
                ret += ItemDataBase.instance.hands[handsIndex].tier;
                break;
            case Slots.Legs:
                ret += ItemDataBase.instance.legs[legsIndex].tier;
                break;
            case Slots.MainHand:
                switch(wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        ret += ItemDataBase.instance.swords[mainHandIndex].tier;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        ret += ItemDataBase.instance.daggers[mainHandIndex].tier;
                        break;
                }
                break;
            case Slots.OffHand:
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        ret += ItemDataBase.instance.shields[offHandIndex].tier;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        ret += ItemDataBase.instance.pistols[offHandIndex].tier;
                        break;
                }
                break;
        }

        // Set rarity
        ret += " " + rarity.name;
        switch (slot)
        {
            case Slots.Head:
                headQuality.text = ret;
                break;
            case Slots.Chest:
                chestQuality.text = ret;
                break;
            case Slots.Hands:
                handsQuality.text = ret;
                break;
            case Slots.Legs:
                legsQuality.text = ret;
                break;
            case Slots.MainHand:
                mainHandQuality.text = ret;
                break;
            case Slots.OffHand:
                offHandQuality.text = ret;
                break;
        }

        // Set colors.
        switch (slot)
        {
            case Slots.Head:
                headName.color = rarity.color;
                headQuality.color = rarity.color;
                headModification.color = rarity.color;
                break;
            case Slots.Chest:
                chestName.color = rarity.color;
                chestQuality.color = rarity.color;
                chestModification.color = rarity.color;
                break;
            case Slots.Hands:
                handsName.color = rarity.color;
                handsQuality.color = rarity.color;
                handsModification.color = rarity.color;
                break;
            case Slots.Legs:
                legsName.color = rarity.color;
                legsQuality.color = rarity.color;
                legsModification.color = rarity.color;
                break;
            case Slots.MainHand:
                mainHandName.color = rarity.color;
                mainHandQuality.color = rarity.color;
                mainHandModification.color = rarity.color;
                break;
            case Slots.OffHand:
                offHandName.color = rarity.color;
                offHandQuality.color = rarity.color;
                offHandModification.color = rarity.color;
                break;
        }
    }

    private void ClearModStats(Slots slot)
    {
        switch (slot)
        {
            case Slots.Head:
                headWeightMod.text = "";
                headPhysicalMod.text = "";
                headMentalMod.text = "";
                headHeatMod.text = "";
                headColdMod.text = "";
                headToxinMod.text = "";
                headElectricityMod.text = "";
                headBalanceMod.text = "";
                break;
            case Slots.Chest:
                chestWeightMod.text = "";
                chestPhysicalMod.text = "";
                chestMentalMod.text = "";
                chestHeatMod.text = "";
                chestColdMod.text = "";
                chestToxinMod.text = "";
                chestElectricityMod.text = "";
                chestBalanceMod.text = "";
                break;
            case Slots.Hands:
                handsWeightMod.text = "";
                handsPhysicalMod.text = "";
                handsMentalMod.text = "";
                handsHeatMod.text = "";
                handsColdMod.text = "";
                handsToxinMod.text = "";
                handsElectricityMod.text = "";
                handsBalanceMod.text = "";
                break;
            case Slots.Legs:
                legsWeightMod.text = "";
                legsPhysicalMod.text = "";
                legsMentalMod.text = "";
                legsHeatMod.text = "";
                legsColdMod.text = "";
                legsToxinMod.text = "";
                legsElectricityMod.text = "";
                legsBalanceMod.text = "";
                break;
            case Slots.MainHand:
                mainHandWeightMod.text = "";
                mainHandPhysicalMod.text = "";
                mainHandMentalMod.text = "";
                mainHandHeatMod.text = "";
                mainHandColdMod.text = "";
                mainHandToxinMod.text = "";
                mainHandElectricityMod.text = "";
                mainHandBalanceMod.text = "";
                break;
            case Slots.OffHand:
                offHandWeightMod.text = "";
                offHandPhysicalMod.text = "";
                offHandMentalMod.text = "";
                offHandHeatMod.text = "";
                offHandColdMod.text = "";
                offHandToxinMod.text = "";
                offHandElectricityMod.text = "";
                offHandBalanceMod.text = "";
                break;
        }
    }

    public void SetModStat(float stat, float modStat, Text text, bool inverted, bool decimals, bool percentage)
    {
        if (modStat != 0)
        {
            string format = !decimals ? "0" : "0.0";

            if (!inverted)
                    text.text = "+" + (stat * modStat).ToString(format);
            else if (inverted)
                    text.text = "-" + (stat * modStat).ToString(format);

            if (percentage)
                text.text += "%";
        }
    }

    public void ColourModStat(float stat, float modStat, Text text)
    {
        if ((stat * modStat) > 0)
            text.color = ItemDataBase.instance.better;
    }

    private void RefreshMenuTotalStats()
    {
        // Player stats.
        totalWeight.text = weight.ToString("0.0") + "/" + maxWeight.ToString("0.0");
        totalWeightPercent.text = (weight / maxWeight).ToString("0%");
        // Defense.
        totalPhysical.text = physical.ToString("0");
        totalPhysicalPercent.text = (physical / maxPhysical).ToString("0%");
        totalMental.text = mental.ToString("0");
        totalMentalPercent.text = (mental / maxMental).ToString("0%");
        totalHeat.text = heat.ToString("0");
        totalHeatPercent.text = (heat / maxHeat).ToString("0%");
        totalCold.text = cold.ToString("0");
        totalColdPercent.text = (cold / maxCold).ToString("0%");
        totalToxin.text = toxin.ToString("0");
        totalToxinPercent.text = (toxin / maxToxin).ToString("0%");
        totalElectricity.text = electricity.ToString("0");
        totalElectricityPercent.text = (electricity / maxElectricity).ToString("0%");
        totalBalance.text = balance.ToString("0") + "/" + maxBalance.ToString("0");
        totalBalancePercent.text = (balance / maxBalance).ToString("0%");
        if (totalBalancePercent.text.Equals("NaN"))
            totalBalancePercent.text = "0%";
        // Damage.
        totalMHAttack.text = (physicalMH + heatMH + coldMH + toxinMH + electricityMH).ToString("0");
        totalMHEfficiency.text = mentalMH.ToString("0") + "%";
        totalMHBalance.text = balanceMH.ToString("0");
        totalOHAttack.text = (physicalOH + heatOH + coldOH + toxinOH + electricityOH).ToString("0");
        totalOHEfficiency.text = mentalOH.ToString("0") + "%";
        totalOHBalance.text = balanceOH.ToString("0");
    }

    private void RefreshWillColor()
    {
        // Reset the amount of items of each color.
        equippedColors.ResetColors();

        // Go through each slot and add them to the amount of their item color.
        equippedColors.AddColor(ItemDataBase.instance.heads[headIndex].itemColor);
        equippedColors.AddColor(ItemDataBase.instance.chests[chestIndex].itemColor);
        equippedColors.AddColor(ItemDataBase.instance.hands[handsIndex].itemColor);
        equippedColors.AddColor(ItemDataBase.instance.legs[legsIndex].itemColor);

        // Set the previous will colors.
        previousWillColor = willColor;

        // Mix the equipped colors to get the new will color for the light.
        willColor = equippedColors.GetColorMix();

        // Reset the color lerp.
        colorLerp = 0f;
    }

    private void SpawnActual(Slots slot, ItemDataBase.Weapon.WeaponType wType, int slotIndex, ItemDataBase.Rarity rarity, ItemDataBase.Modification modification)
    {
        // Instantiate the actual item in the world.
        ItemDrop spawned = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        // Set its attributes to match the instance.
        spawned.playerDropped = true;
        spawned.slot = slot;
        spawned.wType = wType;
        spawned.slotIndex = slotIndex;
        spawned.rarity = rarity;
        spawned.modification = modification;
    }

    public void EquipPreview()
    {
        if (itemPreview != null)
        {
            // Get the preview item.
            ItemDrop item = itemPreview.GetComponent<ItemDrop>();
            // Equip it.
            EquipItem(item.slot, item.wType, item.slotIndex, item.rarity, item.modification);
            // Refresh player will color.
            RefreshWillColor();
            // Destroy the item being previewed.
            Destroy(itemPreview.gameObject);
        }
    }

    private void Update()
    {
        // Set the color of will based things to match the will color.
        MatchWillColor();

        // Apply a glowing effect to the players light.
        GlowEffect();

        // Selects default when nothing is selected.
        SelectionAux();

        // Check what icons to show.
        CheckIcons();
    }

    private void MatchWillColor()
    {
        if (!playerLight.Color.Equals(willColor))
        {
            // Light.
            playerLight.Color = Color.Lerp(previousWillColor, willColor, colorLerp);
            // UI.
            willHUDCounter.color = Color.Lerp(previousWillColor, willColor, colorLerp);
            willHUDBar.color = Color.Lerp(previousWillColor, willColor, colorLerp);
            willPoints.color = Color.Lerp(previousWillColor, willColor, colorLerp);
            willIcon.color = Color.Lerp(previousWillColor, willColor, colorLerp);
        }

        colorLerp += Time.deltaTime * 1f;
    }

    private void GlowEffect()
    {
        // If the light should glow up...
        if (glowUp)
        {
            // ... aument the light scale.
            playerLight.transform.localScale += Vector3.one * Time.deltaTime * glowSpeed;

            // Check for a change in the glow direction.
            if (playerLight.transform.localScale.x >= maxGlow)
                glowUp = false;
        }
        // If the light should glow down...
        else if (!glowUp)
        {
            // ... decrement the light scale.
            playerLight.transform.localScale -= Vector3.one * Time.deltaTime * glowSpeed;

            // Check for a change in the glow direction.
            if (playerLight.transform.localScale.x <= minGlow)
                glowUp = true;
        }
    }

    private void SelectionAux()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(stats);
    }

    private void CheckIcons()
    {        
        if (InputHandler.instance.controller && !controllerButtons)
        {
            // Inventory.
            controllerButtons = true;
            equipmentMK.SetActive(false);
            equipmentCtrl.SetActive(true);
            mainHandMK.SetActive(false);
            mainHandCtrl.SetActive(true);
            offHandMK.SetActive(false);
            offHandCtrl.SetActive(true);
            headMK.SetActive(false);
            headCtrl.SetActive(true);
            chestMK.SetActive(false);
            chestCtrl.SetActive(true);
            handsMK.SetActive(false);
            handsCtrl.SetActive(true);
            legsMK.SetActive(false);
            legsCtrl.SetActive(true);
            // Preview.
            itemMK.SetActive(false);
            itemCtrl.SetActive(true);
        }
        else if (!InputHandler.instance.controller && controllerButtons)
        {
            // Inventory.
            controllerButtons = false;
            equipmentMK.SetActive(true);
            equipmentCtrl.SetActive(false);
            mainHandMK.SetActive(true);
            mainHandCtrl.SetActive(false);
            offHandMK.SetActive(true);
            offHandCtrl.SetActive(false);
            headMK.SetActive(true);
            headCtrl.SetActive(false);
            chestMK.SetActive(true);
            chestCtrl.SetActive(false);
            handsMK.SetActive(true);
            handsCtrl.SetActive(false);
            legsMK.SetActive(true);
            legsCtrl.SetActive(false);
            // Preview.
            itemMK.SetActive(true);
            itemCtrl.SetActive(false);
        }
    }

    // Methods called by collisions.

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Item touched.
        if (other.CompareTag("Item"))
        {            
            // Get the item component.
            ItemDrop item = other.GetComponent<ItemDrop>();
            if (item.kinematic)
            {
                itemPreview = item;
                ClearPreview();
                SetPreviewInfo(itemPreview);

                // Show preview if the inventory is closed.
                if (!inventory.enabled)
                    preview.enabled = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Item touched.
        if (other.CompareTag("Item"))
        {
            // Set the preview if its empty.
            if (itemPreview == null)
            {
                // Get the item component.
                ItemDrop item = other.GetComponent<ItemDrop>();
                if (item.kinematic)
                {
                    itemPreview = item;
                    ClearPreview();
                    SetPreviewInfo(itemPreview);

                    // Show preview if the inventory is closed.
                    if (!inventory.enabled)
                        preview.enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Item left.
        if (other.CompareTag("Item"))
        {
            // Hide and clear the item preview.
            itemPreview = null;
            ClearPreview();
            preview.enabled = false;                            
        }
    }

    private void ClearPreview()
    {
        // Item color.
        itemColor.color = Color.white;
        // Item sprite.
        itemSprite.sprite = null;
        // Name.
        itemName.text = "Name";
        // Quality.
        itemQuality.text = "Quality";
        itemName.color = Color.white;
        itemQuality.color = Color.white;
        itemModName.color = Color.white;
        // Mod name.
        itemModName.text = "Modification";
        // Stats.
        itemWeight.text = "0";
        itemWeight.color = Color.white;
        itemPhysical.text = "0";
        itemPhysical.color = Color.white;
        itemMental.text = "0";
        itemMental.color = Color.white;
        itemHeat.text = "0";
        itemHeat.color = Color.white;
        itemCold.text = "0";
        itemCold.color = Color.white;
        itemToxin.text = "0";
        itemToxin.color = Color.white;
        itemElectricity.text = "0";
        itemElectricity.color = Color.white;
        itemBalance.text = "0";
        itemBalance.color = Color.white;
        // Mod stats.
        itemWeightMod.text = "";
        itemWeightMod.color = Color.white;
        itemPhysicalMod.text = "";
        itemPhysicalMod.color = Color.white;
        itemMentalMod.text = "";
        itemMentalMod.color = Color.white;
        itemHeatMod.text = "";
        itemHeatMod.color = Color.white;
        itemColdMod.text = "";
        itemColdMod.color = Color.white;
        itemToxinMod.text = "";
        itemToxinMod.color = Color.white;
        itemElectricityMod.text = "";
        itemElectricityMod.color = Color.white;
        itemBalanceMod.text = "";
        itemBalanceMod.color = Color.white;
    }

    private void SetPreviewInfo(ItemDrop item)
    {
        switch (item.slot)
        {
            case Slots.Head:
                // Item color.
                itemColor.color = GetItemColor(ItemDataBase.instance.heads[item.slotIndex].itemColor);
                // Item sprite.
                itemSprite.sprite = ItemDataBase.instance.heads[item.slotIndex].sprite;
                // Name.
                itemName.text = ItemDataBase.instance.heads[item.slotIndex].name;
                // Quality.
                itemQuality.text = "Tier " + ItemDataBase.instance.heads[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                itemName.color = item.rarity.color;
                itemQuality.color = item.rarity.color;
                itemModName.color = item.rarity.color;
                // Mod name.
                itemModName.text = item.modification.name;
                // Stats.
                itemWeight.text = ItemDataBase.instance.heads[item.slotIndex].weight.ToString("0.0");
                itemPhysical.text = ItemDataBase.instance.heads[item.slotIndex].physical.ToString("0");
                itemMental.text = ItemDataBase.instance.heads[item.slotIndex].mental.ToString("0");
                itemHeat.text = ItemDataBase.instance.heads[item.slotIndex].heat.ToString("0");
                itemCold.text = ItemDataBase.instance.heads[item.slotIndex].cold.ToString("0");
                itemToxin.text = ItemDataBase.instance.heads[item.slotIndex].toxin.ToString("0");
                itemElectricity.text = ItemDataBase.instance.heads[item.slotIndex].electricity.ToString("0");
                itemBalance.text = ItemDataBase.instance.heads[item.slotIndex].balance.ToString("0");
                // Mod stats.
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.heads[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                // Preview colors.
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].weight, item.modification.weight,
                               ItemDataBase.instance.heads[headIndex].weight, headMod.weight,
                               itemWeight, itemWeightMod, true);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].physical, item.modification.physical,
                               ItemDataBase.instance.heads[headIndex].physical, headMod.physical,
                               itemPhysical, itemPhysicalMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].mental, item.modification.mental,
                               ItemDataBase.instance.heads[headIndex].mental, headMod.mental,
                               itemMental, itemMentalMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].heat, item.modification.heat,
                               ItemDataBase.instance.heads[headIndex].heat, headMod.heat,
                               itemHeat, itemHeatMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].cold, item.modification.cold,
                               ItemDataBase.instance.heads[headIndex].cold, headMod.cold,
                               itemCold, itemColdMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].toxin, item.modification.toxic,
                               ItemDataBase.instance.heads[headIndex].toxin, headMod.toxic,
                               itemToxin, itemToxinMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].electricity, item.modification.electricity,
                               ItemDataBase.instance.heads[headIndex].electricity, headMod.electricity,
                               itemElectricity, itemElectricityMod, false);
                SetPreviewStat(ItemDataBase.instance.heads[item.slotIndex].balance, item.modification.balance,
                               ItemDataBase.instance.heads[headIndex].balance, headMod.balance,
                               itemBalance, itemBalanceMod, false);
                break;
            case Slots.Chest:
                // Item color.
                itemColor.color = GetItemColor(ItemDataBase.instance.chests[item.slotIndex].itemColor);
                // Item sprite.
                itemSprite.sprite = ItemDataBase.instance.chests[item.slotIndex].sprite;
                // Name.
                itemName.text = ItemDataBase.instance.chests[item.slotIndex].name;
                // Quality.
                itemQuality.text = "Tier " + ItemDataBase.instance.chests[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                itemName.color = item.rarity.color;
                itemQuality.color = item.rarity.color;
                itemModName.color = item.rarity.color;
                // Mod name.
                itemModName.text = item.modification.name;
                // Stats.
                itemWeight.text = ItemDataBase.instance.chests[item.slotIndex].weight.ToString("0.0");
                itemPhysical.text = ItemDataBase.instance.chests[item.slotIndex].physical.ToString("0");
                itemMental.text = ItemDataBase.instance.chests[item.slotIndex].mental.ToString("0");
                itemHeat.text = ItemDataBase.instance.chests[item.slotIndex].heat.ToString("0");
                itemCold.text = ItemDataBase.instance.chests[item.slotIndex].cold.ToString("0");
                itemToxin.text = ItemDataBase.instance.chests[item.slotIndex].toxin.ToString("0");
                itemElectricity.text = ItemDataBase.instance.chests[item.slotIndex].electricity.ToString("0");
                itemBalance.text = ItemDataBase.instance.chests[item.slotIndex].balance.ToString("0");
                // Mod stats.
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.chests[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                // Preview colors.
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].weight, item.modification.weight,
                               ItemDataBase.instance.chests[chestIndex].weight, chestMod.weight,
                               itemWeight, itemWeightMod, true);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].physical, item.modification.physical,
                               ItemDataBase.instance.chests[chestIndex].physical, chestMod.physical,
                               itemPhysical, itemPhysicalMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].mental, item.modification.mental,
                               ItemDataBase.instance.chests[chestIndex].mental, chestMod.mental,
                               itemMental, itemMentalMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].heat, item.modification.heat,
                               ItemDataBase.instance.chests[chestIndex].heat, chestMod.heat,
                               itemHeat, itemHeatMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].cold, item.modification.cold,
                               ItemDataBase.instance.chests[chestIndex].cold, chestMod.cold,
                               itemCold, itemColdMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].toxin, item.modification.toxic,
                               ItemDataBase.instance.chests[chestIndex].toxin, chestMod.toxic,
                               itemToxin, itemToxinMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].electricity, item.modification.electricity,
                               ItemDataBase.instance.chests[chestIndex].electricity, chestMod.electricity,
                               itemElectricity, itemElectricityMod, false);
                SetPreviewStat(ItemDataBase.instance.chests[item.slotIndex].balance, item.modification.balance,
                               ItemDataBase.instance.chests[chestIndex].balance, chestMod.balance,
                               itemBalance, itemBalanceMod, false);
                break;
            case Slots.Hands:
                // Item color.
                itemColor.color = GetItemColor(ItemDataBase.instance.hands[item.slotIndex].itemColor);
                // Item sprite.
                itemSprite.sprite = ItemDataBase.instance.hands[item.slotIndex].sprite;
                // Name.
                itemName.text = ItemDataBase.instance.hands[item.slotIndex].name;
                // Quality.
                itemQuality.text = "Tier " + ItemDataBase.instance.hands[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                itemName.color = item.rarity.color;
                itemQuality.color = item.rarity.color;
                itemModName.color = item.rarity.color;
                // Mod name.
                itemModName.text = item.modification.name;
                // Stats.
                itemWeight.text = ItemDataBase.instance.hands[item.slotIndex].weight.ToString("0.0");
                itemPhysical.text = ItemDataBase.instance.hands[item.slotIndex].physical.ToString("0");
                itemMental.text = ItemDataBase.instance.hands[item.slotIndex].mental.ToString("0");
                itemHeat.text = ItemDataBase.instance.hands[item.slotIndex].heat.ToString("0");
                itemCold.text = ItemDataBase.instance.hands[item.slotIndex].cold.ToString("0");
                itemToxin.text = ItemDataBase.instance.hands[item.slotIndex].toxin.ToString("0");
                itemElectricity.text = ItemDataBase.instance.hands[item.slotIndex].electricity.ToString("0");
                itemBalance.text = ItemDataBase.instance.hands[item.slotIndex].balance.ToString("0");
                // Mod stats.
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.hands[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                // Preview colors.
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].weight, item.modification.weight,
                               ItemDataBase.instance.hands[handsIndex].weight, handsMod.weight,
                               itemWeight, itemWeightMod, true);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].physical, item.modification.physical,
                               ItemDataBase.instance.hands[handsIndex].physical, handsMod.physical,
                               itemPhysical, itemPhysicalMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].mental, item.modification.mental,
                               ItemDataBase.instance.hands[handsIndex].mental, handsMod.mental,
                               itemMental, itemMentalMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].heat, item.modification.heat,
                               ItemDataBase.instance.hands[handsIndex].heat, handsMod.heat,
                               itemHeat, itemHeatMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].cold, item.modification.cold,
                               ItemDataBase.instance.hands[handsIndex].cold, handsMod.cold,
                               itemCold, itemColdMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].toxin, item.modification.toxic,
                               ItemDataBase.instance.hands[handsIndex].toxin, handsMod.toxic,
                               itemToxin, itemToxinMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].electricity, item.modification.electricity,
                               ItemDataBase.instance.hands[handsIndex].electricity, handsMod.electricity,
                               itemElectricity, itemElectricityMod, false);
                SetPreviewStat(ItemDataBase.instance.hands[item.slotIndex].balance, item.modification.balance,
                               ItemDataBase.instance.hands[handsIndex].balance, handsMod.balance,
                               itemBalance, itemBalanceMod, false);
                break;
            case Slots.Legs:
                // Item color.
                itemColor.color = GetItemColor(ItemDataBase.instance.legs[item.slotIndex].itemColor);
                // Item sprite.
                itemSprite.sprite = ItemDataBase.instance.legs[item.slotIndex].sprite;
                // Name.
                itemName.text = ItemDataBase.instance.legs[item.slotIndex].name;
                // Quality.
                itemQuality.text = "Tier " + ItemDataBase.instance.legs[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                itemName.color = item.rarity.color;
                itemQuality.color = item.rarity.color;
                itemModName.color = item.rarity.color;
                // Mod name.
                itemModName.text = item.modification.name;
                // Stats.
                itemWeight.text = ItemDataBase.instance.legs[item.slotIndex].weight.ToString("0.0");
                itemPhysical.text = ItemDataBase.instance.legs[item.slotIndex].physical.ToString("0");
                itemMental.text = ItemDataBase.instance.legs[item.slotIndex].mental.ToString("0");
                itemHeat.text = ItemDataBase.instance.legs[item.slotIndex].heat.ToString("0");
                itemCold.text = ItemDataBase.instance.legs[item.slotIndex].cold.ToString("0");
                itemToxin.text = ItemDataBase.instance.legs[item.slotIndex].toxin.ToString("0");
                itemElectricity.text = ItemDataBase.instance.legs[item.slotIndex].electricity.ToString("0");
                itemBalance.text = ItemDataBase.instance.legs[item.slotIndex].balance.ToString("0");
                // Mod stats.
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                SetModStat(ItemDataBase.instance.legs[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                // Preview colors.
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].weight, item.modification.weight,
                               ItemDataBase.instance.legs[legsIndex].weight, legsMod.weight,
                               itemWeight, itemWeightMod, true);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].physical, item.modification.physical,
                               ItemDataBase.instance.legs[legsIndex].physical, legsMod.physical,
                               itemPhysical, itemPhysicalMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].mental, item.modification.mental,
                               ItemDataBase.instance.legs[legsIndex].mental, legsMod.mental,
                               itemMental, itemMentalMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].heat, item.modification.heat,
                               ItemDataBase.instance.legs[legsIndex].heat, legsMod.heat,
                               itemHeat, itemHeatMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].cold, item.modification.cold,
                               ItemDataBase.instance.legs[legsIndex].cold, legsMod.cold,
                               itemCold, itemColdMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].toxin, item.modification.toxic,
                               ItemDataBase.instance.legs[legsIndex].toxin, legsMod.toxic,
                               itemToxin, itemToxinMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].electricity, item.modification.electricity,
                               ItemDataBase.instance.legs[legsIndex].electricity, legsMod.electricity,
                               itemElectricity, itemElectricityMod, false);
                SetPreviewStat(ItemDataBase.instance.legs[item.slotIndex].balance, item.modification.balance,
                               ItemDataBase.instance.legs[legsIndex].balance, legsMod.balance,
                               itemBalance, itemBalanceMod, false);
                break;
            case Slots.MainHand:
                // Use the appropiate weapon type.
                switch(item.wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        // Item color.
                        itemColor.color = GetItemColor(ItemDataBase.instance.swords[item.slotIndex].itemColor);
                        // Item sprite.
                        itemSprite.sprite = ItemDataBase.instance.swords[item.slotIndex].sprite;
                        // Name.
                        itemName.text = ItemDataBase.instance.swords[item.slotIndex].name;
                        // Quality.
                        itemQuality.text = "Tier " + ItemDataBase.instance.swords[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                        itemName.color = item.rarity.color;
                        itemQuality.color = item.rarity.color;
                        itemModName.color = item.rarity.color;
                        // Mod name.
                        itemModName.text = item.modification.name;
                        // Stats.
                        itemWeight.text = ItemDataBase.instance.swords[item.slotIndex].weight.ToString("0.0");
                        itemPhysical.text = ItemDataBase.instance.swords[item.slotIndex].physical.ToString("0");
                        itemMental.text = ItemDataBase.instance.swords[item.slotIndex].mental.ToString("0") + "%";
                        itemHeat.text = ItemDataBase.instance.swords[item.slotIndex].heat.ToString("0");
                        itemCold.text = ItemDataBase.instance.swords[item.slotIndex].cold.ToString("0");
                        itemToxin.text = ItemDataBase.instance.swords[item.slotIndex].toxin.ToString("0");
                        itemElectricity.text = ItemDataBase.instance.swords[item.slotIndex].electricity.ToString("0");
                        itemBalance.text = ItemDataBase.instance.swords[item.slotIndex].balance.ToString("0");
                        // Mod stats.
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.swords[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                        // Preview colors.
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].weight, item.modification.weight,
                                       weightMH, 0, 
                                       itemWeight, itemWeightMod, true);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].physical, item.modification.physical,
                                       physicalMH, 0, 
                                       itemPhysical, itemPhysicalMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].mental, item.modification.mental,
                                       mentalMH, 0,
                                       itemMental, itemMentalMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].heat, item.modification.heat,
                                       heatMH, 0,
                                       itemHeat, itemHeatMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].cold, item.modification.cold,
                                       coldMH, 0,
                                       itemCold, itemColdMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].toxin, item.modification.toxic,
                                       toxinMH, 0,
                                       itemToxin, itemToxinMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].electricity, item.modification.electricity,
                                       electricityMH, 0,
                                       itemElectricity, itemElectricityMod, false);
                        SetPreviewStat(ItemDataBase.instance.swords[item.slotIndex].balance, item.modification.balance,
                                       balanceMH, 0,
                                       itemBalance, itemBalanceMod, false);
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        // Item color.
                        itemColor.color = GetItemColor(ItemDataBase.instance.daggers[item.slotIndex].itemColor);
                        // Item sprite.
                        itemSprite.sprite = ItemDataBase.instance.daggers[item.slotIndex].sprite;
                        // Name.
                        itemName.text = ItemDataBase.instance.daggers[item.slotIndex].name;
                        // Quality.
                        itemQuality.text = "Tier " + ItemDataBase.instance.daggers[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                        itemName.color = item.rarity.color;
                        itemQuality.color = item.rarity.color;
                        itemModName.color = item.rarity.color;
                        // Mod name.
                        itemModName.text = item.modification.name;
                        // Stats.
                        itemWeight.text = ItemDataBase.instance.daggers[item.slotIndex].weight.ToString("0.0");
                        itemPhysical.text = ItemDataBase.instance.daggers[item.slotIndex].physical.ToString("0");
                        itemMental.text = ItemDataBase.instance.daggers[item.slotIndex].mental.ToString("0") + "%";
                        itemHeat.text = ItemDataBase.instance.daggers[item.slotIndex].heat.ToString("0");
                        itemCold.text = ItemDataBase.instance.daggers[item.slotIndex].cold.ToString("0");
                        itemToxin.text = ItemDataBase.instance.daggers[item.slotIndex].toxin.ToString("0");
                        itemElectricity.text = ItemDataBase.instance.daggers[item.slotIndex].electricity.ToString("0");
                        itemBalance.text = ItemDataBase.instance.daggers[item.slotIndex].balance.ToString("0");
                        // Mod stats.
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.daggers[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                        // Preview colors.
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].weight, item.modification.weight,
                                       weightMH, 0,
                                       itemWeight, itemWeightMod, true);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].physical, item.modification.physical,
                                       physicalMH, 0,
                                       itemPhysical, itemPhysicalMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].mental, item.modification.mental,
                                       mentalMH, 0,
                                       itemMental, itemMentalMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].heat, item.modification.heat,
                                       heatMH, 0,
                                       itemHeat, itemHeatMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].cold, item.modification.cold,
                                       coldMH, 0,
                                       itemCold, itemColdMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].toxin, item.modification.toxic,
                                       toxinMH, 0,
                                       itemToxin, itemToxinMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].electricity, item.modification.electricity,
                                       electricityMH, 0,
                                       itemElectricity, itemElectricityMod, false);
                        SetPreviewStat(ItemDataBase.instance.daggers[item.slotIndex].balance, item.modification.balance,
                                       balanceMH, 0,
                                       itemBalance, itemBalanceMod, false);
                        break;
                    default:
                        break;
                }                                                
                break;
            case Slots.OffHand:
                // Use the appropiate weapon type.
                switch (item.wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        // Item color.
                        itemColor.color = GetItemColor(ItemDataBase.instance.shields[item.slotIndex].itemColor);
                        // Item sprite.
                        itemSprite.sprite = ItemDataBase.instance.shields[item.slotIndex].sprite;
                        // Name.
                        itemName.text = ItemDataBase.instance.shields[item.slotIndex].name;
                        // Quality.
                        itemQuality.text = "Tier " + ItemDataBase.instance.shields[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                        itemName.color = item.rarity.color;
                        itemQuality.color = item.rarity.color;
                        itemModName.color = item.rarity.color;
                        // Mod name.
                        itemModName.text = item.modification.name;
                        // Stats.
                        itemWeight.text = ItemDataBase.instance.shields[item.slotIndex].weight.ToString("0.0");
                        itemPhysical.text = ItemDataBase.instance.shields[item.slotIndex].physical.ToString("0");
                        itemMental.text = ItemDataBase.instance.shields[item.slotIndex].mental.ToString("0") + "%";
                        itemHeat.text = ItemDataBase.instance.shields[item.slotIndex].heat.ToString("0");
                        itemCold.text = ItemDataBase.instance.shields[item.slotIndex].cold.ToString("0");
                        itemToxin.text = ItemDataBase.instance.shields[item.slotIndex].toxin.ToString("0");
                        itemElectricity.text = ItemDataBase.instance.shields[item.slotIndex].electricity.ToString("0");
                        itemBalance.text = ItemDataBase.instance.shields[item.slotIndex].balance.ToString("0");
                        // Mod stats.
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.shields[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                        // Preview colors.
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].weight, item.modification.weight,
                                       weightOH, 0,
                                       itemWeight, itemWeightMod, true);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].physical, item.modification.physical,
                                       physicalOH, 0,
                                       itemPhysical, itemPhysicalMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].mental, item.modification.mental,
                                       mentalOH, 0,
                                       itemMental, itemMentalMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].heat, item.modification.heat,
                                       heatOH, 0,
                                       itemHeat, itemHeatMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].cold, item.modification.cold,
                                       coldOH, 0,
                                       itemCold, itemColdMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].toxin, item.modification.toxic,
                                       toxinOH, 0,
                                       itemToxin, itemToxinMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].electricity, item.modification.electricity,
                                       electricityOH, 0,
                                       itemElectricity, itemElectricityMod, false);
                        SetPreviewStat(ItemDataBase.instance.shields[item.slotIndex].balance, item.modification.balance,
                                       balanceOH, 0,
                                       itemBalance, itemBalanceMod, false);
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        // Item color.
                        itemColor.color = GetItemColor(ItemDataBase.instance.pistols[item.slotIndex].itemColor);
                        // Item sprite.
                        itemSprite.sprite = ItemDataBase.instance.pistols[item.slotIndex].sprite;
                        // Name.
                        itemName.text = ItemDataBase.instance.pistols[item.slotIndex].name;
                        // Quality.
                        itemQuality.text = "Tier " + ItemDataBase.instance.pistols[item.slotIndex].tier.ToString() + " " + item.rarity.name;
                        itemName.color = item.rarity.color;
                        itemQuality.color = item.rarity.color;
                        itemModName.color = item.rarity.color;
                        // Mod name.
                        itemModName.text = item.modification.name;
                        // Stats.
                        itemWeight.text = ItemDataBase.instance.pistols[item.slotIndex].weight.ToString("0.0");
                        itemPhysical.text = ItemDataBase.instance.pistols[item.slotIndex].physical.ToString("0");
                        itemMental.text = ItemDataBase.instance.pistols[item.slotIndex].mental.ToString("0") + "%";
                        itemHeat.text = ItemDataBase.instance.pistols[item.slotIndex].heat.ToString("0");
                        itemCold.text = ItemDataBase.instance.pistols[item.slotIndex].cold.ToString("0");
                        itemToxin.text = ItemDataBase.instance.pistols[item.slotIndex].toxin.ToString("0");
                        itemElectricity.text = ItemDataBase.instance.pistols[item.slotIndex].electricity.ToString("0");
                        itemBalance.text = ItemDataBase.instance.pistols[item.slotIndex].balance.ToString("0");
                        // Mod stats.
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].weight, item.modification.weight, itemWeightMod, true, true, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].physical, item.modification.physical, itemPhysicalMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].mental, item.modification.mental, itemMentalMod, false, false, true);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].heat, item.modification.heat, itemHeatMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].cold, item.modification.cold, itemColdMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].toxin, item.modification.toxic, itemToxinMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].electricity, item.modification.electricity, itemElectricityMod, false, false, false);
                        SetModStat(ItemDataBase.instance.pistols[item.slotIndex].balance, item.modification.balance, itemBalanceMod, false, false, false);
                        // Preview colors.
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].weight, item.modification.weight,
                                       weightOH, 0,
                                       itemWeight, itemWeightMod, true);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].physical, item.modification.physical,
                                       physicalOH, 0,
                                       itemPhysical, itemPhysicalMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].mental, item.modification.mental,
                                       mentalOH, 0,
                                       itemMental, itemMentalMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].heat, item.modification.heat,
                                       heatOH, 0,
                                       itemHeat, itemHeatMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].cold, item.modification.cold,
                                       coldOH, 0,
                                       itemCold, itemColdMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].toxin, item.modification.toxic,
                                       toxinOH, 0,
                                       itemToxin, itemToxinMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].electricity, item.modification.electricity,
                                       electricityOH, 0,
                                       itemElectricity, itemElectricityMod, false);
                        SetPreviewStat(ItemDataBase.instance.pistols[item.slotIndex].balance, item.modification.balance,
                                       balanceOH, 0,
                                       itemBalance, itemBalanceMod, false);
                        break;
                    default:
                        break;
                }
                break;
        }
    }    
    
    private void SetPreviewStat(float previewItemStat, float previewModStat, float actualItemStat, float actualModStat, Text previewStatText, Text previewModText, bool inverted)
    {
        if (!inverted)
        {
            if ((previewItemStat + (previewItemStat * previewModStat)) > (actualItemStat + (actualItemStat * actualModStat)))
            {
                previewStatText.color = ItemDataBase.instance.better;
                previewModText.color = ItemDataBase.instance.better;
            }
            else if ((previewItemStat + (previewItemStat * previewModStat)) < (actualItemStat + (actualItemStat * actualModStat)))
            {
                previewStatText.color = ItemDataBase.instance.worse;
                previewModText.color = ItemDataBase.instance.worse;
            }
        }
        else if(inverted)
        {
            if ((previewItemStat - (previewItemStat * previewModStat)) < (actualItemStat - (actualItemStat * actualModStat)))
            {
                previewStatText.color = ItemDataBase.instance.better;
                previewModText.color = ItemDataBase.instance.better;
            }
            else if ((previewItemStat - (previewItemStat * previewModStat)) > (actualItemStat - (actualItemStat * actualModStat)))
            {
                previewStatText.color = ItemDataBase.instance.worse;
                previewModText.color = ItemDataBase.instance.worse;
            }
        }
    }
}
