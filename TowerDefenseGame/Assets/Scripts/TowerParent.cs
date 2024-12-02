using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyTowerEnums;

public abstract class TowerParent : MonoBehaviour
{
    public HealthBar healthBar;
    public DefenseObject placable;
    public PlacableType placableType;

    public float maxHealth;
    public float health;
    public float fireRate;
    protected float damage;
    public float range;
    public SphereCollider sphereCollider;
    public GameObject target;
    protected bool isTower = false;
    private protected Coroutine attackCoroutine;
    protected EnemyScript currentTarget;
    public bool isAttacking = false;
    public float targetCheckTime = 2;
    public float targetCheckTimer = 0;
    public GameObject model;
    public Transform appearenceTransform;

    public GameObject[] upgradeButton;
    public TMP_Text[] upgradeName;
    public int[] upgradeCost;
    public TMP_Text[] upgradeCostText;
    private Camera mainCamera; // Reference to the main camera
    private bool isMouseOver = false; // Track if the mouse is over the object
    public bool isUpgraded = false;

    public virtual void OnTowerPlaced(DefenseObject placable)
    {
        this.placable = placable;
        maxHealth = placable.health;
        health = placable.health;
        fireRate = placable.FireRate;
        range = placable.range;
        damage = placable.damage;
        placableType = placable.type;
        healthBar.SetHealth(health, maxHealth);
        
        for(int i = 0; i < upgradeName.Length; i++)
        {
            if (placable.upgrades[i] != null)
            {
                upgradeName[i].text = placable.upgrades[i].name;
                upgradeCost[i] = placable.upgrades[i].cost;
                upgradeCostText[i].text = upgradeCost[i].ToString();
            }
        }
        if (placable.type == PlacableType.Tower)
        {
            isTower = true;
            return;
        }
        ChangeAppearence(placable.appearence);
    }

    private void Start()
    {
        mainCamera = Camera.main;

        // Ensure buttons are hidden at the start
        if (upgradeButton != null) 
        {
            foreach (GameObject button in upgradeButton)
            {
                if (button != null) button.SetActive(false);
            }
        }
    }

    //attacks based on attack rate
    public virtual IEnumerator AttackCoroutine()
    {
        while (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= range)
            {
                Attack(currentTarget);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    public virtual void Attack(EnemyScript target)
    {
        if (target == null)
        {
            return;
        }
        target.OnDamageTaken(damage);
        Debug.Log("Attack" + gameObject.name);
    }
    //sets target and starts the attack
    public virtual void SetTarget(GameObject targetToAttack)
    {
        target = targetToAttack;
        if (target == null) return;
        currentTarget = target.GetComponent<EnemyScript>();
        if (!isAttacking)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
            isAttacking = true;
        }
    }

    public virtual void StopAttacking()
    {
        if (isAttacking)
        {
            //StopCoroutine(attackCoroutine);
            isAttacking = false;
            target = null;
            currentTarget = null;
        }
    }
    public virtual void OnDamageTaken(float takenDamage)
    {
        //Debug.Log("damage taken");
        health -= takenDamage;
        healthBar.SetHealth(health, maxHealth);
        if (health <= 0)
        {
            OnKilled();
        }
    }
    public void OnKilled()
    {
        ResourceManager.instance.spawnedDefenses.Remove(gameObject);
        if (isTower)    // checks whether game is lost or not
        {
            ResourceManager.instance.lost = true;
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        //stops the attack coroutine
        StopAttacking();
    }
    public void ChangeAppearence(GameObject newModel)
    {
        if(model != null)
        {
            Destroy(model);
        }
        model = Instantiate(newModel, appearenceTransform);
    }
    public void HandleMouseOver()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (!isMouseOver)
                {
                    if (isUpgraded == false)
                    {
                        isMouseOver = true;
                        SetButtonsActive(true);
                    }
                }
                return;
            }
        }

        if (isMouseOver)
        {
            isMouseOver = false;
            SetButtonsActive(false);
        }
    }

    private void SetButtonsActive(bool isActive)
    {
        if (upgradeButton != null)
        {
            foreach (GameObject button in upgradeButton)
            {
                if (button != null) button.SetActive(isActive);
            }
        }
    }
    private void Update()
    {
        HandleMouseOver();
    }
    public void UpgradeUnit(int index)
    {
        Debug.Log("upgrade clicked");
        if (ResourceManager.instance.money < placable.upgrades[index].cost) return;
        ResourceManager.instance.money -= placable.upgrades[index].cost;
        maxHealth = placable.upgrades[index].health;
        health = placable.upgrades[index].health;
        fireRate = placable.upgrades[index].FireRate;
        range = placable.upgrades[index].range;
        damage = placable.upgrades[index].damage;
        healthBar.SetHealth(health, maxHealth);
        ChangeAppearence(placable.upgrades[index].appearence);
        SetButtonsActive(false);
        isUpgraded = true;
    }
}
