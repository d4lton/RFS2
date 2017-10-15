using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour {

	class PoolObject {
		public Transform transform;
		public bool inUse = false;
		public PoolObject(Transform t) {transform = t;}
		public void Use() {inUse = true;}
		public void Dispose() {
			inUse = false;
			//transform.position = Vector3.one * 1000;
		}
	}

	[System.Serializable]
	public struct YSpawnRange {
		public float min;
		public float max;
	}

	public GameObject Prefab;
	public int poolSize;
	public float shiftSpeed;
	public float spawnRate;

	public YSpawnRange ySpawnRange;
	public Vector3 defaultSpawnPos;
	public bool immediate;
	public Vector3 immediateSpawnPos;

	float spawnTimer = 0;
	PoolObject[] poolObjects;

	GameManager gameManager;

	void Awake() {
		configure();
	}

	void Start() {
	}
	
	void Update() {
		shift();
		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnRate) {
			spawn();
			spawnTimer = 0;
		}
	}

	void configure() {
		poolObjects = new PoolObject[poolSize];
		for (int i = 0; i < poolObjects.Length; i++) {
			GameObject gameObject = Instantiate(Prefab) as GameObject;
			Transform t = gameObject.transform;
			t.SetParent(transform);
			t.position = Vector3.one * -1000;
			poolObjects[i] = new PoolObject(t);
		}
		if (immediate) {
			spawnImmediate();
		}
	}

	void spawn() {
		PoolObject poolObject = getPoolObject();
		if (poolObject != null) {
			poolObject.transform.localPosition = new Vector3(defaultSpawnPos.x, Random.Range(ySpawnRange.min, ySpawnRange.max), 0);
		}
	}

	void spawnImmediate() {
		PoolObject poolObject = getPoolObject();
		if (poolObject != null) {
			poolObject.transform.localPosition = new Vector3(immediateSpawnPos.x, Random.Range(ySpawnRange.min, ySpawnRange.max), 0);
		}
		spawn();
	}

	void shift() {
		for (int i = 0; i < poolObjects.Length; i++) {
			if (poolObjects[i].inUse) {
				poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
				checkDisposeObject(poolObjects[i]);
			}
		}
	}

	void checkDisposeObject(PoolObject poolObject) {
		if (poolObject.transform.localPosition.x < -defaultSpawnPos.x) {
			poolObject.Dispose();
			poolObject.transform.position = Vector3.one * -1000;
		}
	}

	PoolObject getPoolObject() {
		for (int i = 0; i < poolObjects.Length; i++) {
			if (!poolObjects[i].inUse) {
				poolObjects[i].Use();
				return poolObjects[i];
			}
		}
		return null;
	}

}
