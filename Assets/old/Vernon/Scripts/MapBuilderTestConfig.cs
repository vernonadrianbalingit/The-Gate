using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapBuilderTestConfig : MonoBehaviour
{
	//henry
	public GameObject s;  //start object
	public GameObject end;  //end object
	public GameObject w;  //waypoint object 
	public GameObject r;  //road object 
	public GameObject e;  //empty
	//end henry


	//vern
	public GameObject t;  //tower
	public GameObject enemySpawner; //enemy 

	public string[,] map; //declaring the double array map string
    private GameObject _mapParent; //empty game object to organize the hierarchy
	private GameObject[] roads;

	public DirectedAgent directedAgent;
	public SpawnEnemiesVern spawnEnemiesVern;
	public Vector3 endPoint;

	[SerializeField] private GameObject navMeshRoot = null;



    private void Awake()
    {
        if (navMeshRoot == null)
        {
			navMeshRoot = new GameObject("nav" +
				"MeshRoot");
        }
    }


    // Start is called before the first frame update
    void Start()
	{
		spawnEnemiesVern = GetComponent<SpawnEnemiesVern>();
		//an empty game objects where all instanciated objects will go under for organization. 

		_mapParent = GameObject.Find("MapParent"); 
		if (!_mapParent)
		{
			_mapParent = new GameObject("MapParent");
		}
		
		//setTestingLevel();
		spawnEnemiesVern.setSpawnPoint(s.transform);
		setLevel1();
		spawnEnemiesVern.setEndPoint(endPoint);



		buildMap();



		roads = GameObject.FindGameObjectsWithTag("Road");
		Debug.Log("Number of road objects found: " + roads.Length);

		for(int i= 0; i < roads.Length; i++)
        {
			roads[i].GetComponent<NavMeshSurface>().BuildNavMesh();
			Debug.Log("Nav mesh built: " + i);
        }

		spawnEnemiesVern.SpawnEnemy();
		directedAgent.MoveToLocation(endPoint);
		Debug.Log("agent point set");







	}


	//vern
	public void setTestingLevel() //small map to test code 
    {
		map = new string[5, 10]
		{
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "s", "r", "r", "e", "e", "r", "r", "r", "end"},
			{"e", "e", "e", "r", "t", "e", "r", "e", "e", "e"},
			{"e", "e", "e", "r", "r", "r", "r", "t", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e"}
		};
		Debug.Log("Testing level set");
    }
	public void setLevel1()
    {
		map = new string[30, 30]
		{
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "s", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "e", "e", "e", "t", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "t", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "t", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "end", "e"},
			{"e", "r", "e", "r", "r", "r", "r", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "t", "e", "e", "e", "t", "e", "e", "e", "t", "e", "e", "t", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "t", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "r", "e", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "r", "r", "r", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "t", "e", "e", "e", "t", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "r", "e", "e", "e", "e"},
			{"e", "e", "t", "e", "e", "e", "e", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "r", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"},
			{"e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e"}
		};

		Debug.Log("level 1 set");
    }

	public void buildMap() //goes through map double array to build the map by checking the string 
	{

		for (int row = 0; row < map.GetLength(0); row++)
		{
			for (int col = 0; col < map.GetLength(1); col++)
			{
				if (map[row, col] == "e")
				{
					Instantiate(e, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity).transform.parent
						= _mapParent.transform;
				}
				else if (map[row, col] == "s")
				{
					spawnEnemiesVern.setSpawnPoint(s.transform);
					spawnEnemiesVern.setEndPoint(endPoint);

					Instantiate(r, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
						.transform.parent = _mapParent.transform; //take out if doesnt work

					spawnEnemiesVern.SpawnEnemies();

					Instantiate(spawnEnemiesVern.enemyPrefab, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
						.transform.parent = _mapParent.transform;


					//Instantiate(spawnEnemiesVern, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
					//	.transform.parent = _mapParent.transform;

					//Instantiate(spawnEnemiesVern, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity);
					//s.GetComponent<WaveSpawner>().spawnPoint = s.transform;
					//s.GetComponent<WaveSpawner>().enemyPrefab = Instantiate(enemy, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity);


				}
				else if (map[row, col] == "r")
				{
					Instantiate(r, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
						.transform.parent = _mapParent.transform;
					

				}
				else if (map[row, col] == "t")
                {
					Instantiate(t, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
						.transform.parent = _mapParent.transform;
                }
				else if(map[row, col] == "end")
                {
					Instantiate(end, new Vector3(3.0f * col, 0.0f, 3.0f * row), Quaternion.identity)
						.transform.parent = _mapParent.transform;
					//endPoint = new Vector3(3.0f * col, 0.0f, 3.0f * row);
					setEndPoint(new Vector3(3.0f * col, 0.0f, 3.0f * row));
					Debug.Log("Spawn Point set");
					
					
				}
			}
		}
    

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void setEndPoint(Vector3 ep)
    {
		endPoint = ep;
    }

	public Vector3 getEndPoint()
    {
		return endPoint;
    }
}