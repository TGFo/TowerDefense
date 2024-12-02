using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlacable : ScriptableObject
{
    public float health;
    public new string name;
    public string description;
    public int cost;
    public GameObject model;
    public Sprite icon;
    public GameObject appearence;
}
