using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class ItemDrop : MonoBehaviour
{
    [Header("Random Generation")]
    public bool randomize;                              // Whether or not to randomize the item at start.

    [Header("Manual Generation")]
    public PlayerInventory.Slots slot;                  // The actual item type.  
    public ItemDataBase.Weapon.WeaponType wType;        // The type of weapon if it is one.
    public int slotIndex;                               // The index of an array of items in the items data base.     
    public int rarityIndex;                             // the index of rarity of the item.  

    [Header("Attributes")]
    public ItemDataBase.Rarity rarity;                  // The rarity of item to spawn.    
    public ItemDataBase.Modification modification;      // The modification of the item to spawn.
    private int tierIndex;                              // The index of tier of the item.    
    private int modNum;                                 // Number of current mods.
    private List<ItemDataBase.Modification.Stats> modStats; // A list to store the possible mods for the item to spawn.

    [Header("Behaviour")]
    public bool playerDropped;                          // Whether or not the player dropped this item.
    private Rigidbody2D itemRb;                         // A reference to the rigidbody2D component.
    public LayerMask floor;                             // A mask determining what should make the item kinematic. 
    public bool kinematic;                              // Whether or not to be kinematic.    

    [Header("Visuals")]
    public SpriteRenderer itemSprite;                   // The sprite that represents the item.
    private Color itemColor;                            // The color of the item.
    private float colorLerp;                            // Aux for color transition.
    public LightSprite itemLight;                       // The light of the item.
    public float minGlow = 4f;                          // The minimum scale to glow down.
    public float maxGlow = 5f;                          // The maximum scale to glow up.
    public float glowSpeed = 1f;                        // The speed of the glow transition.
    private bool glowUp;                                // The glow direction.
    

    private void Awake()
    {
        // Get references.
        itemRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Generate the item if the player didnt dropped it.
        if (!playerDropped)
        {
            if (randomize)
            {
                // Get random item attributes.
                GetItem();
                SetTier();
                GetRarity();
            }
            // Set the item attributes.
            SetTier();
            SetRarity();
            SetModification();
        }

        // Set the item world visuals.
        SetItemVisuals();

        // Set the movement.
        SetMovement();
    }

    private void GetItem()
    {
        // Get the item slot.
        slot = (PlayerInventory.Slots)Random.Range(0, System.Enum.GetValues(typeof(PlayerInventory.Slots)).Length);
        // Get the item index based on the slot.
        switch (slot)
        {
            case PlayerInventory.Slots.Head:
                slotIndex = Random.Range(1, ItemDataBase.instance.heads.Length);
                break;
            case PlayerInventory.Slots.Chest:
                slotIndex = Random.Range(1, ItemDataBase.instance.chests.Length);
                break;
            case PlayerInventory.Slots.Hands:
                slotIndex = Random.Range(1, ItemDataBase.instance.hands.Length);
                break;
            case PlayerInventory.Slots.Legs:
                slotIndex = Random.Range(1, ItemDataBase.instance.legs.Length);
                break;
            case PlayerInventory.Slots.MainHand:
                wType = (ItemDataBase.Weapon.WeaponType)Random.Range(1, 3);
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        slotIndex = Random.Range(1, ItemDataBase.instance.swords.Length);
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        slotIndex = Random.Range(1, ItemDataBase.instance.daggers.Length);
                        break;
                    default:
                        break;
                }                
                break;
            case PlayerInventory.Slots.OffHand:
                wType = (ItemDataBase.Weapon.WeaponType)Random.Range(3, 5);
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        slotIndex = Random.Range(1, ItemDataBase.instance.shields.Length);
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        slotIndex = Random.Range(1, ItemDataBase.instance.pistols.Length);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }        
    }

    private void GetRarity()
    {
        if (tierIndex != 0)
        {
            // Cycle through the rarities to see wich one gets the chance.
            float chance = Random.value;
            for (int i = 0; i < ItemDataBase.instance.rarities.Length; i++)
            {
                if (chance <= ItemDataBase.instance.rarities[i].chance)
                {
                    rarityIndex = i;
                    break;
                }
            }
        }        
    }

    private void SetTier()
    {
        // Use the slot and slot index to set the item tier.
        switch (slot)
        {
            case PlayerInventory.Slots.Head:
                tierIndex = ItemDataBase.instance.heads[slotIndex].tier;
                break;
            case PlayerInventory.Slots.Chest:
                tierIndex = ItemDataBase.instance.chests[slotIndex].tier;
                break;
            case PlayerInventory.Slots.Hands:
                tierIndex = ItemDataBase.instance.hands[slotIndex].tier;
                break;
            case PlayerInventory.Slots.Legs:
                tierIndex = ItemDataBase.instance.legs[slotIndex].tier;
                break;
            case PlayerInventory.Slots.MainHand:                
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        tierIndex = ItemDataBase.instance.swords[slotIndex].tier;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        tierIndex = ItemDataBase.instance.daggers[slotIndex].tier;
                        break;
                    default:
                        break;
                }
                break;
            case PlayerInventory.Slots.OffHand:
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        tierIndex = ItemDataBase.instance.shields[slotIndex].tier;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        tierIndex = ItemDataBase.instance.pistols[slotIndex].tier;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void SetRarity()
    {
        rarity = ItemDataBase.instance.rarities[rarityIndex];
    }

    private void SetModification()
    {
        // Set the possible stats to modify in a list.
        modStats = new List<ItemDataBase.Modification.Stats>();
        // Default stats.
        modStats.Add(ItemDataBase.Modification.Stats.Weight);
        modStats.Add(ItemDataBase.Modification.Stats.Physical);
        modStats.Add(ItemDataBase.Modification.Stats.Mental);
        modStats.Add(ItemDataBase.Modification.Stats.Heat);
        modStats.Add(ItemDataBase.Modification.Stats.Cold);
        modStats.Add(ItemDataBase.Modification.Stats.Toxic);
        modStats.Add(ItemDataBase.Modification.Stats.Electricity);
        modStats.Add(ItemDataBase.Modification.Stats.Balance);

        // Set the potency of the mod based on the tier of the item and the number of stats based on the rarity of the item.
        switch (tierIndex)
        {
            case 1:
                switch(rarityIndex)
                {
                    case 0:
                        while (modNum < ItemDataBase.instance.minR0Mods)
                            SetModStat(tierIndex);
                        while(modNum < ItemDataBase.instance.maxR0Mods && Random.value <= ItemDataBase.instance.extraModChance)                                                
                            SetModStat(tierIndex);                        
                        break;
                    case 1:
                        while (modNum < ItemDataBase.instance.minR1Mods)
                            SetModStat(tierIndex);
                        while (modNum < ItemDataBase.instance.maxR1Mods && Random.value <= ItemDataBase.instance.extraModChance)
                            SetModStat(tierIndex);
                        break;
                    case 2:
                        while (modNum < ItemDataBase.instance.minR2Mods)
                            SetModStat(tierIndex);
                        while (modNum < ItemDataBase.instance.maxR2Mods && Random.value <= ItemDataBase.instance.extraModChance)
                            SetModStat(tierIndex);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void SetModStat(int tier)
    {
        // Get a possible stat.
        ItemDataBase.Modification.Stats stat = modStats[Random.Range(0, modStats.Count)];

        // Set the mod value based on the tier.
        switch(tier)
        {
            case 1:
                switch(stat)
                {
                    case ItemDataBase.Modification.Stats.Weight:
                        modification.weight = Random.Range(ItemDataBase.instance.t1MinWeightMult, ItemDataBase.instance.t1MaxWeightMult);
                        break;
                    case ItemDataBase.Modification.Stats.Physical:
                        modification.physical = Random.Range(ItemDataBase.instance.t1MinPhysicalMult, ItemDataBase.instance.t1MaxPhysicalMult);
                        break;
                    case ItemDataBase.Modification.Stats.Mental:
                        modification.mental = Random.Range(ItemDataBase.instance.t1MinMentalMult, ItemDataBase.instance.t1MaxMentalMult);
                        break;
                    case ItemDataBase.Modification.Stats.Heat:
                        modification.heat = Random.Range(ItemDataBase.instance.t1MinHeatMult, ItemDataBase.instance.t1MaxHeatMult);
                        break;
                    case ItemDataBase.Modification.Stats.Cold:
                        modification.cold = Random.Range(ItemDataBase.instance.t1MinColdMult, ItemDataBase.instance.t1MaxColdMult);
                        break;
                    case ItemDataBase.Modification.Stats.Toxic:
                        modification.toxic = Random.Range(ItemDataBase.instance.t1MinToxicMult, ItemDataBase.instance.t1MaxToxicMult);
                        break;
                    case ItemDataBase.Modification.Stats.Electricity:
                        modification.electricity = Random.Range(ItemDataBase.instance.t1MinElectricalMult, ItemDataBase.instance.t1MaxElectricalMult);
                        break;
                    case ItemDataBase.Modification.Stats.Balance:
                        modification.balance = Random.Range(ItemDataBase.instance.t1MinBalanceMult, ItemDataBase.instance.t1MaxBalanceMult);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        // Increment the mod count for in and max checks.
        modNum++;

        // Delete the stat from the possible stat list.
        modStats.Remove(stat);
    }

    private void SetItemVisuals()
    {
        switch (slot)
        {
            case PlayerInventory.Slots.Head:
                // Light Color.
                itemColor = GetItemColor(ItemDataBase.instance.heads[slotIndex].itemColor);
                // Sprite.
                itemSprite.sprite = ItemDataBase.instance.heads[slotIndex].sprite;
                break;
            case PlayerInventory.Slots.Chest:
                // Light Color.
                itemColor = GetItemColor(ItemDataBase.instance.chests[slotIndex].itemColor);
                // Sprite.
                itemSprite.sprite = ItemDataBase.instance.chests[slotIndex].sprite;
                break;
            case PlayerInventory.Slots.Hands:
                // Light Color.
                itemColor = GetItemColor(ItemDataBase.instance.hands[slotIndex].itemColor);
                // Sprite.
                itemSprite.sprite = ItemDataBase.instance.hands[slotIndex].sprite;
                break;
            case PlayerInventory.Slots.Legs:
                // Light Color.
                itemColor = GetItemColor(ItemDataBase.instance.legs[slotIndex].itemColor);
                // Sprite.
                itemSprite.sprite = ItemDataBase.instance.legs[slotIndex].sprite;
                break;
            case PlayerInventory.Slots.MainHand:
                // Use the appropiate weapon type.
                switch(wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Sword:
                        // Light Color.
                        itemColor = GetItemColor(ItemDataBase.instance.swords[slotIndex].itemColor);
                        // Sprite.
                        itemSprite.sprite = ItemDataBase.instance.swords[slotIndex].sprite;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Dagger:
                        // Light Color.
                        itemColor = GetItemColor(ItemDataBase.instance.daggers[slotIndex].itemColor);
                        // Sprite.
                        itemSprite.sprite = ItemDataBase.instance.daggers[slotIndex].sprite;
                        break;
                    default:
                        break;
                }
                break;
            case PlayerInventory.Slots.OffHand:
                // Use the appropiate weapon type.
                switch (wType)
                {
                    case ItemDataBase.Weapon.WeaponType.Shield:
                        // Light Color.
                        itemColor = GetItemColor(ItemDataBase.instance.shields[slotIndex].itemColor);
                        // Sprite.
                        itemSprite.sprite = ItemDataBase.instance.shields[slotIndex].sprite;
                        break;
                    case ItemDataBase.Weapon.WeaponType.Pistol:
                        // Light Color.
                        itemColor = GetItemColor(ItemDataBase.instance.pistols[slotIndex].itemColor);
                        // Sprite.
                        itemSprite.sprite = ItemDataBase.instance.pistols[slotIndex].sprite;
                        break;
                    default:
                        break;
                }
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

    private void SetMovement()
    {
        // Set the force.
        Vector2 force = Vector2.zero;

        force.y = 1800f;

        force.x = Random.Range(-750f, 750f);
        if (force.x >= 0)
            force.x = Mathf.Clamp(force.x, 250, 500);
        else
            force.x = Mathf.Clamp(force.x, -500, -250);

        // Add the force.
        itemRb.AddForce(force);
    }

    private void Update()
    {
        // Manage the state state of the rigidbody based on the floor.
        FloorCheck();

        // Apply a glowing effect to the item light.
        GlowEffect();
    }

    private void FloorCheck()
    {
        // Check for floor if the item can move and its going down.
        if (!kinematic && itemRb.velocity.y <= 0f)
        {
            // The item shouldnt move if a ray cast from its position hits anything designated as floor.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.25f, floor);
            if (hit.collider != null)
            {
                kinematic = true;
                itemRb.velocity = Vector2.zero;
                itemRb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void GlowEffect()
    {
        // Light color.
        if (!itemLight.Color.Equals(itemColor))
        {
            itemLight.Color = Color.Lerp(Color.black, itemColor, colorLerp);
            colorLerp += Time.deltaTime * 1f;
        }

        // If the light should glow up...
        if (glowUp)
        {
            // ... aument the light scale.
            itemLight.transform.localScale += Vector3.one * Time.deltaTime * glowSpeed;

            // Check for a change in the glow direction.
            if (itemLight.transform.localScale.x >= maxGlow)
                glowUp = false;
        }
        // If the light should glow down...
        else if (!glowUp)
        {
            // ... decrement the light scale.
            itemLight.transform.localScale -= Vector3.one * Time.deltaTime * glowSpeed;

            // Check for a change in the glow direction.
            if (itemLight.transform.localScale.x <= minGlow)
                glowUp = true;
        }
    }
}
