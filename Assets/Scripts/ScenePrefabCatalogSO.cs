using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Prefab Catalog", menuName = "Scene Prefab Catalog")]
public class ScenePrefabCatalogSO : ScriptableObject
{
    public GameObject cratePrefab;
    public GameObject foodBagPrefab;
    public GameObject rubbishPrefab;
    public GameObject trashBagRegularPrefab;
    public GameObject trashBagOrganicPrefab;
    public GameObject trashBagPaperPrefab;
    public GameObject trashBagPlasticPrefab;
}
