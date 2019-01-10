using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {

    public TowerButton towerBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;

    private List<Tower> TowerList = new List<Tower>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 mapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mapPoint, Vector2.zero);

            // place towers only on special ground areas
            // tagged as "buildGround"
            if (hit.collider.tag == "buildGround")
            {
                // to avoid double tower placing on single ground
                buildTile = hit.collider;
                buildTile.tag = "groundWithTowerOn";
                RegisterBuildSite(buildTile);
                placeTower(hit);
            }
        }
        if (spriteRenderer.enabled)
        {
            followMouse();
        }
    }

    public void RegisterBuildSite(Collider2D buildTag)
    {
        BuildList.Add(buildTag);
    }

    public void RegisterTower(Tower tower)
    {
        TowerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach(Collider2D buildTag in BuildList)
        {
            buildTag.tag = "buildGround";
        }
        BuildList.Clear();
    }

    public void DestroyAllTower()
    {
        foreach(Tower tower in TowerList)
        {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // make sure dragged tower is above everything on map
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void enableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
    }

    public void placeTower(RaycastHit2D hit)
    {
        // problem fix
        // when we press on tower button it tries to build tower there
        if (! EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
        {
            Tower newTower = Instantiate(towerBtnPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
            buyTower(towerBtnPressed.TowerPrice);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            disableDragSprite();
            towerBtnPressed = null;
        }
    }

    public void buyTower(int price)
    {
        GameManager.Instance.subtractMoney(price);
    }

    public void selectedTower(TowerButton towerSelected)
    {
        if(towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
        {
            towerBtnPressed = towerSelected;
            enableDragSprite(towerBtnPressed.DragSprite);
        }
        //Debug.Log("Pressed! :" + towerBtnPressed.gameObject);
    }
}
