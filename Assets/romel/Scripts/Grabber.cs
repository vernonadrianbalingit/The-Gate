using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{   
    public GameObject GameCurrencyObject;
    private GameObject grabbed = null;
    private GameCurrency gameCurrency;
    private MonoBehaviour currentTurretScript;

    private int price = -1;
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
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbed != null)
            {
                // Place on the closest stand
                GameObject closestStand = GetClosestStand(grabbed);

                if (CheckValidPlacement(closestStand))
                {
                    grabbed.transform.position = closestStand.transform.position;
                    grabbed.transform.position = new Vector3(grabbed.transform.position.x, 2.88f, grabbed.transform.position.z);
                    
                    // Re-enable the turret script when placing
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
        
        // Tab key to destroy grabbed turret
        if (Input.GetKeyDown(KeyCode.Tab) && grabbed != null)
        {
            Debug.Log("Destroyed turret in hand (Tab): " + grabbed.name);
            if (gameCurrency != null)
            {
                gameCurrency.AddMoney(price);
                Debug.Log("Refunded 1 dollar");
            }
            currentTurretScript = null;
            Destroy(grabbed);
            grabbed = null;
            Cursor.visible = true;
        }
        
        // Right-click handler
        if (Input.GetMouseButtonDown(1))
        {
            // If holding a turret, destroy it
            if (grabbed != null)
            {
                Debug.Log("Destroyed turret in hand: " + grabbed.name);
                currentTurretScript = null;
                gameCurrency.AddMoney(price);
                Destroy(grabbed);
                price = -1;
                grabbed = null;
                Cursor.visible = true;
            }
            // Otherwise, try to destroy a placed turret
            else
            {
                Debug.Log("Right-click on placed turret - checking raycast...");
                RaycastHit hit = CastRay();
                
                if (hit.collider != null)
                {
                    Debug.Log($"Raycast hit: {hit.collider.gameObject.name}, Tag: {hit.collider.gameObject.tag}");
                    
                    if (hit.collider.gameObject.CompareTag("Turret"))
                    {
                        GameObject turretToDestroy = hit.collider.gameObject;
                        GameObject standParent = GetGrandestParent(GetClosestStand(turretToDestroy));
                        GameObject cameraParent = GetGrandestParent(GetCurrentCamera().gameObject);
                        
                        Debug.Log($"Turret parent: {standParent.name}, Camera parent: {cameraParent.name}, Same: {standParent == cameraParent}");
                        
                        // Check if turret is in the same section as the camera
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
                                    Debug.Log("Freed up stand: " + closestStand.name);
                                }
                            }
                            
                            Debug.Log("Destroyed turret: " + turretToDestroy.name);
                            if (gameCurrency != null)
                            {
                                gameCurrency.AddMoney(1);
                                Debug.Log("Refunded 1 dollar");
                            }
                            Destroy(turretToDestroy);
                        }
                        else
                        {
                            Debug.Log("Turret not in same section as camera - cannot destroy");
                        }
                    }
                    else
                    {
                        Debug.Log("Hit object is not tagged as Turret");
                    }
                }
                else
                {
                    Debug.Log("Raycast hit nothing");
                }
            }
        }
        
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
        if (prefab == null)
        {
            Debug.LogWarning("Grabber: Cannot spawn null prefab");
            return;
        }

        // Get the mouse position in world space
        Camera cam = GetCurrentCamera();
        Vector3 ScreenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 10f);
        Vector3 worldMousePos = cam.ScreenToWorldPoint(ScreenMousePos);

        // Spawn the prefab at the cursor position
        GameObject spawnedObject = Instantiate(prefab, new Vector3(worldMousePos.x, 3.88f, worldMousePos.z), Quaternion.identity);

        // Automatically grab the spawned object
        grabbed = spawnedObject;
        Cursor.visible = false;

        price = turretPrice;

        // Get connected script of turret - check all possible turret controller types
        currentTurretScript = grabbed.GetComponent<MonoBehaviour>();
        
        // Try to find the specific turret controller component
        var rapidFire = grabbed.GetComponent("RapidFireTurretController") as MonoBehaviour;
        var normal = grabbed.GetComponent("TurretController") as MonoBehaviour;
        var sniper = grabbed.GetComponent("SniperTurretController") as MonoBehaviour;
        
        if (rapidFire != null)
            currentTurretScript = rapidFire;
        else if (normal != null)
            currentTurretScript = normal;
        else if (sniper != null)
            currentTurretScript = sniper;

        if (currentTurretScript != null)
        {
            currentTurretScript.enabled = false;
            Debug.Log($"Disabled turret script: {currentTurretScript.GetType().Name}");
        }

        currentTurretScript.enabled = false;
    }
}
