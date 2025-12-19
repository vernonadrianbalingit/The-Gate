using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Grabber functions for spawning turrets and placing them
- Transforms turrets to cursor position when grabbed
- checks valid placement on stand
- refunds money based on cancellation or destruction
*/

public class Grabber : MonoBehaviour
{   
    public GameObject GameCurrencyObject;
    private GameObject grabbed = null;
    private GameCurrency gameCurrency;
    private MonoBehaviour currentTurretScript;

    private int price = -1; // null price
    void Start()
    {
        if (GameCurrencyObject != null)
        {
            gameCurrency = GameCurrencyObject.GetComponent<GameCurrency>();
        }
    }

    public Camera GetCurrentCamera()
    {
        return Camera.current != null ? Camera.current : Camera.main;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbed != null)
            {
                GameObject closestStand = GetClosestStand(grabbed);

                if (CheckValidPlacement(closestStand))
                {
                    grabbed.transform.position = closestStand.transform.position;

                    // sets postion equal to the closest stand and sets predetermined y position
                    grabbed.transform.position = new Vector3(grabbed.transform.position.x, 2.88f, grabbed.transform.position.z);
                    
                    // enables turret functions when placed
                    if (currentTurretScript != null)
                    {
                        currentTurretScript.enabled = true;
                        currentTurretScript = null;
                    }
                    
                    grabbed = null;
                    Cursor.visible = true;
                }
            }
        }
        
        // refunds money and destroys grabbed turret when menu is toggled while still placing turret
        if (Input.GetKeyDown(KeyCode.Tab) && grabbed != null)
        {
            if (gameCurrency != null)
            {
                gameCurrency.AddMoney(price);
            }
            currentTurretScript = null;
            Destroy(grabbed);
            grabbed = null;
            Cursor.visible = true;
        }

        //right click to cancel placement or destroy turret
        if (Input.GetMouseButtonDown(1))
        {
            if (grabbed != null)
            {
                currentTurretScript = null;
                gameCurrency.AddMoney(price);
                Destroy(grabbed);
                price = -1;
                grabbed = null;
                Cursor.visible = true;
            }
            else
            {
                RaycastHit hit = CastRay();
                
                if (hit.collider != null)
                {
                    
                    if (hit.collider.gameObject.CompareTag("Turret"))
                    {
                        GameObject turretToDestroy = hit.collider.gameObject;
                        GameObject standParent = GetGrandestParent(GetClosestStand(turretToDestroy));
                        GameObject cameraParent = GetGrandestParent(GetCurrentCamera().gameObject);
                    
                        if (standParent == cameraParent)
                        {
                            // Free up the stand before destroying
                            GameObject closestStand = GetClosestStand(turretToDestroy);
                            if (closestStand != null)
                            {
                                StandInUse standInUse = closestStand.GetComponent<StandInUse>();
                                if (standInUse != null)
                                {
                                    standInUse.SetInUse(false);
                                }
                            }
                            
                            if (gameCurrency != null)
                            {
                                gameCurrency.AddMoney(1);
                            }
                            Destroy(turretToDestroy);
                        }
                    }
                }
            }
        }
        
        // moves grabbed turret to cursor position
        if (grabbed != null)
        {   
            GameObject closestStand = GetClosestStand(grabbed);
            Vector3 ScreenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GetCurrentCamera().WorldToScreenPoint(grabbed.transform.position).z);   
            Vector3 worldMousePos = GetCurrentCamera().ScreenToWorldPoint(ScreenMousePos);
            grabbed.transform.position = new Vector3(worldMousePos.x, 3.88f, worldMousePos.z);
        }
    }

    private RaycastHit CastRay()
    {
        Camera cam = GetCurrentCamera();
        Vector3 ScreenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.farClipPlane);
        Vector3 ScreenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
        Vector3 worldMousePosFar = cam.ScreenToWorldPoint(ScreenMousePosFar);
        Vector3 worldMousePosNear = cam.ScreenToWorldPoint(ScreenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
        return hit;
    }

    private GameObject GetClosestStand(GameObject turret)
    {
        GameObject[] stands = GameObject.FindGameObjectsWithTag("Stand");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 turretPos = turret.transform.position;

        foreach (GameObject stand in stands)
        {
            float distance = Vector3.Distance(stand.transform.position, turretPos);
            if (distance < minDistance)
            {
                closest = stand;
                minDistance = distance;
            }
        }

        return closest;
    }

    private bool CheckValidPlacement(GameObject stand)
    {
        // find sections of stand and camera for valid placement
        GameObject StandSection = GetGrandestParent(stand);
        GameObject CameraSection = GetGrandestParent(GetCurrentCamera().gameObject);

        StandInUse standInUse = stand.GetComponent<StandInUse>();
        
        if (standInUse != null && standInUse.IsInUse())
        {
            return false;
        }
        else
        {
            if (standInUse != null)
            {
                standInUse.SetInUse(true);
            }
        }

        return StandSection == CameraSection;

    }

    private GameObject GetGrandestParent(GameObject obj)
    {
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current.gameObject;
    }

    public void SpawnPrefabAtCursor(GameObject prefab, int turretPrice)
    {
        // Get the mouse position in world space
        Camera cam = GetCurrentCamera();
        Vector3 ScreenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 10f);
        Vector3 worldMousePos = cam.ScreenToWorldPoint(ScreenMousePos);

        GameObject spawnedObject = Instantiate(prefab, new Vector3(worldMousePos.x, 3.88f, worldMousePos.z), Quaternion.identity);

        grabbed = spawnedObject;
        Cursor.visible = false;

        price = turretPrice;

        currentTurretScript = grabbed.GetComponent<MonoBehaviour>();
        

        var rapidFire = grabbed.GetComponent("RapidFireTurretController") as MonoBehaviour;
        var normal = grabbed.GetComponent("TurretController") as MonoBehaviour;
        var sniper = grabbed.GetComponent("SniperTurretController") as MonoBehaviour;
        
        if (rapidFire != null)
            currentTurretScript = rapidFire;
        else if (normal != null)
            currentTurretScript = normal;
        else if (sniper != null)
            currentTurretScript = sniper;

        // disable turret functions while placing
        if (currentTurretScript != null)
        {
            currentTurretScript.enabled = false;
        }

        currentTurretScript.enabled = false;
    }
}
