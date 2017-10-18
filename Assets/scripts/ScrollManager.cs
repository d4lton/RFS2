using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour {

	class PoolObject {
		public Transform transform;
		public bool inUse = false;
		public PoolObject(Transform t) {transform = t;}
		public void acquire() {inUse = true;}
		public void release() {inUse = false;}
	}

	[System.Serializable]
	public struct SpawnRange {
		public float min;
		public float max;
	}

	public GameObject prefab;
	public int poolSize;
	public float shiftSpeed;
	public float spawnRate;

	public SpawnRange xSpawnRange;
	public SpawnRange ySpawnRange;
	public Vector3 defaultSpawnPos;
	public bool immediate;
	public Vector3 immediateSpawnPos;

	float spawnTimer = 0;
	PoolObject[] poolObjects;

	void Awake() {
		configure();
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
			GameObject gameObject = Instantiate(prefab) as GameObject;
			Transform t = gameObject.transform;
			t.SetParent(transform);
			t.position = Vector3.one * -1000;
			poolObjects[i] = new PoolObject(t);
		}
		if (immediate) {
			spawnImmediate();
		}
		spawn(); // spawn a prefab in the default position, since it'll be "spawnRate" seconds before this is done
	}

	void spawn() {
		PoolObject poolObject = getPoolObject();
		if (poolObject != null) {
			poolObject.transform.localPosition = new Vector3(defaultSpawnPos.x + Random.Range(xSpawnRange.min, xSpawnRange.max), Random.Range(ySpawnRange.min, ySpawnRange.max), 0);
		}
	}

	void spawnImmediate() {
		PoolObject poolObject = getPoolObject();
		if (poolObject != null) {
			poolObject.transform.localPosition = new Vector3(immediateSpawnPos.x, Random.Range(ySpawnRange.min, ySpawnRange.max), 0);
		}
	}

	void shift() {
		if (poolObjects != null) {
			for (int i = 0; i < poolObjects.Length; i++) {
				if (poolObjects[i].inUse) {
					poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
					checkDisposeObject(poolObjects[i]);
				}
			}
		}
	}

	void checkDisposeObject(PoolObject poolObject) {
		if (poolObject.transform.localPosition.x < -defaultSpawnPos.x) {
			poolObject.release();
			poolObject.transform.position = Vector3.one * -1000;
		}
	}

	PoolObject getPoolObject() {
		if (poolObjects != null) {
			for (int i = 0; i < poolObjects.Length; i++) {
				if (!poolObjects[i].inUse) {
					poolObjects[i].acquire();
					return poolObjects[i];
				}
			}
		}
		return null;
	}

}
