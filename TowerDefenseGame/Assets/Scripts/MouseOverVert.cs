using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnemyTowerEnums;

public class MouseOverVert : MonoBehaviour
{
    public Vector3 mousePos;
    public Vector3 vertPosition;
    public MeshGenerator meshGenerator;
    public Camera cam;
    public int vertIndex;
    public GameObject currentSelected;
    public DefenseObject currentPlacable;
    public Button buttonPrefab;
    public GameObject selectedImage;
    public Material material;
    Material defaultmat;
    public GameObject verticalLayoutGroup;
    public Button towerBuy;
    public bool towerPlaced = false;
    public TMP_Text currentSelectedTxt;
    // Start is called before the first frame update
    void Start()
    {
        //defaultmat = selectedImage.GetComponent<Material>();
        //selectedImage.GetComponent<Renderer>().material = material;
        //RefreshBuyList();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        mousePos.z = 10;
        //Highlights closest vert to mouse position
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore))
        {
            vertIndex = meshGenerator.GetVertexIndex(hit.point);
            vertPosition = meshGenerator.GetClosestVertPosition(hit.point);
            meshGenerator.HighlightVert(vertPosition);
            //selectedImage.SetActive(true);
            //selectedImage.transform.position = vertPosition;
            if (Input.GetMouseButtonDown(0))
            {
                //places tower or defense
                if (currentSelected != null)
                {
                    PlaceSelected(vertPosition);
                }
            }
            if(currentSelected == null)
            {
                currentSelectedTxt.text = "";
            }else
            currentSelectedTxt.text = currentSelected.name;
        }
    }
    public void SetSelected(DefenseObject selected)
    {
        //sets the selected object for placement
        currentPlacable = selected;
        currentSelected = selected.model;
    }
    //places the selected tower
    public void PlaceSelected(Vector3 pos)
    {
        if (towerPlaced)
        {
            //checks if object can be placed
            if (meshGenerator.IsPlacable(pos) == false)
            {
                return;
            }
        }
        if(ResourceManager.instance.money < currentPlacable.cost)   //checks if object can be purchased
        {
            return;
        }
        ResourceManager.instance.PlaceDefense(currentPlacable);
        GameObject defense = Instantiate(currentSelected);
        defense.transform.position = pos;
        TowerParent towerScript = defense.GetComponent<TowerParent>();
        towerScript.OnTowerPlaced(currentPlacable);
        ResourceManager.instance.spawnedDefenses.Add(defense);
        if(currentPlacable.type == PlacableType.Tower)
        {
            pos.y = 0;
            defense.transform.position = pos;
            TowerSelected(pos, defense);
            ResourceManager.instance.mainTower = towerScript;
        }
        meshGenerator.HidePlacementPoint(pos);
    }
    public void TowerSelected(Vector3 pos, GameObject defense)
    {
        Debug.Log("tower placed");
        ResourceManager.instance.towerObj = defense;
        towerBuy.gameObject.SetActive(false);
        currentPlacable = null;
        currentSelected = null;
        meshGenerator.AddTower(pos);
        towerPlaced = true;
        ResourceManager.instance.OnTowerPlaced();
        meshGenerator.HighLightPlacePoints();
        RefreshBuyList();
    }
    public void RefreshBuyList()
    {
        //adds buttons to purchaseable list based on available defenses
        foreach(DefenseObject placable in ResourceManager.instance.GetPlacables())
        {
            Button buyButton = Instantiate(buttonPrefab, verticalLayoutGroup.transform);
            BuyTowerButtonScript buyTowerButtonScript = buyButton.GetComponent<BuyTowerButtonScript>();
            buyTowerButtonScript.RefreshButton(placable);
            buyButton.onClick.AddListener(() => { SetSelected(placable); });
        }
    }
}
