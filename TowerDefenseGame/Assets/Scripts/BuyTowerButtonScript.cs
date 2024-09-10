using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyTowerButtonScript : MonoBehaviour
{
    public TMP_Text towerName;
    public TMP_Text description;
    public Image image;
    public TMP_Text cost;
    public BasePlacable tower;
    //sets the buttons text and icons to display correct properties
    public void RefreshButton(BasePlacable tower)
    {
        this.tower = tower;
        towerName.text = tower.name;
        description.text = tower.description;
        image.sprite = tower.icon;
        cost.text = tower.cost.ToString();
    }
    private void Start()
    {
        if(tower != null)
        {
            RefreshButton(tower);
        }
    }
}
