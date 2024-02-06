using UnityEngine;
using UnityEngine.AI;

public class EnemyProperties : GamiManagers
{
    [Header("Enemy Healh and Damage")]
    public float enemyHealth = 120f;
    public float presentHealth;
    public float giveDamage = 5f;
    public float enemySpeed;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform LookPoint;
    public GameObject shootingRaycastArea;
    public Transform playerBody;
    public LayerMask playerLayer;
    public GameObject spawnPoints;
    public Transform spawn;
    public Transform enemyCharacter;

    [Header("Enemy Animation and Spark effect")]
    public Animator anim;
    public ParticleSystem muzzuleSpark;

    [Header("Enemy States")]
    public float visionRadius = 50f;
    public float shootingRadius;
    public float checkPlayerVision;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;
    public bool isPlayer = false;

    public ScoreManager scoreManager;

    [Header("Sound effects")]
    public AudioSource audioSource;
    public AudioClip shootingSound;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEnemyAgent();
        this.LoadShootingRaycastArea();
        this.LoadSpawnPoints();
        this.LoadSpawn();
        this.LoadEnemyCharacter();
        this.LoadPlayerBody();
        this.LoadLookPoint();
    }

    protected virtual void LoadEnemyAgent()
    {
        if (this.enemyAgent != null) return;

        this.enemyAgent = GetComponent<NavMeshAgent>();
    }
    protected virtual void LoadShootingRaycastArea()
    {
        if (this.shootingRaycastArea != null) return;

        Transform shootingAreaTransform = transform.Find("ShootingAreaRaycast");

        if (shootingAreaTransform != null)
        {
            this.shootingRaycastArea = shootingAreaTransform.gameObject;
        }
        else
        {
            Debug.LogError("ShootingAreaRaycast object not found as a child of EnemyProperties.");
        }
    }
    protected virtual void LoadSpawnPoints()
    {
        if (this.spawnPoints != null) return;

        this.spawnPoints = GameObject.Find("SpawnPoints");
    }
    protected virtual void LoadSpawn()
    {

        if (this.spawnPoints != null)
        {
            Transform[] spawnPointChildren = spawnPoints.GetComponentsInChildren<Transform>();

            if (spawnPointChildren.Length > 1)
            {
                this.spawn = spawnPointChildren[Random.Range(1, spawnPointChildren.Length)];
            }
            else
            {
                Debug.LogError("No spawn points found under SpawnPoints.");
            }
        }
        else
        {
            Debug.LogError("SpawnPoints not found. Cannot load spawn point.");
        }
    }
    protected virtual void LoadEnemyCharacter()
    {
        if (this.enemyCharacter != null) return;

        this.enemyCharacter = transform;

        if (this.enemyCharacter == null)
        {
            Debug.LogError("Transform component not found on the object.");
        }
    }
    protected virtual void LoadPlayerBody()
    {
        GameObject playersObject = GameObject.Find("Players");

        if (playersObject != null)
        {
            if (isPlayer)
            {
                playerBody = GameObject.Find("Player").transform;
            }
            else
            {
                Transform[] playerChildren = playersObject.transform.GetComponentsInChildren<Transform>(true);

                float closestDistance = Mathf.Infinity;

                foreach (Transform child in playerChildren)
                {
                    if (child != playersObject.transform && child.parent == playersObject.transform)
                    {
                        float distance = Vector3.Distance(child.position, this.enemyCharacter.position);

                        if (distance <= visionRadius && distance < closestDistance)
                        {
                            this.playerBody = child;
                            closestDistance = distance;
                        }
                    }
                }


            }
        }
        else
        {
            //Debug.LogError("Players object not found in the hierarchy.");
        }
    }


    protected virtual void LoadLookPoint()
    {

        if (this.playerBody != null)
        {
            Transform lookPointTransform = this.playerBody.Find("LookPoint");
            this.LookPoint = lookPointTransform;
        }
    }

}