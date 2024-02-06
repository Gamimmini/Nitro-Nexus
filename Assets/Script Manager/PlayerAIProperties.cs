using UnityEngine;
using UnityEngine.AI;

public class PlayerAIProperties : GamiManagers
{
    [Header("Player Healh and Damage")]
    public float PlayerHealth = 120f;
    public float presentHealth;
    public float giveDamage = 5f;
    public float PlayerSpeed;

    [Header("Player Things")]
    public NavMeshAgent PlayerAgent;
    public Transform LookPoint;
    public GameObject shootingRaycastArea;
    public Transform enemyBody;
    public LayerMask enemyLayer;
    public GameObject spawnPointsForPlayer;
    public Transform spawn;
    public Transform PlayerCharacter;

    [Header("Player Animation and Spark effect")]
    public Animator anim;
    public ParticleSystem muzzuleSpark;

    [Header("Player States")]
    public float visionRadius = 50000f;
    public float shootingRadius;
    public bool enemyInvisionRadius;
    public bool enemyInshootingRadius;
    //public bool isPlayer = false;

    [Header("Sound effects")]
    public AudioSource audioSource;
    public AudioClip shootingSound;

    public ScoreManager scoreManager;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerAgent();
        this.LoadShootingRaycastArea();
        this.LoadSpawnPointsForPlayer();
        this.LoadSpawn();
        this.LoadPlayerCharacter();
        this.LoadEnemyBody();
        this.LoadLookPoint();
    }

    protected virtual void LoadPlayerAgent()
    {
        if (this.PlayerAgent != null) return;

        this.PlayerAgent = GetComponent<NavMeshAgent>();
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
            Debug.LogError("ShootingAreaRaycast object not found as a child of PlayerAIProperties.");
        }
    }
    protected virtual void LoadSpawnPointsForPlayer()
    {
        if (this.spawnPointsForPlayer != null) return;

        this.spawnPointsForPlayer = GameObject.Find("SpawnPointsForPlayer");

        if (this.spawnPointsForPlayer == null)
        {
            Debug.LogError("SpawnPointsForPlayer object not found in the hierarchy.");
        }
    }
    protected virtual void LoadSpawn()
    {

        if (this.spawnPointsForPlayer != null)
        {
            Transform[] spawnPointChildren = spawnPointsForPlayer.GetComponentsInChildren<Transform>();

            if (spawnPointChildren.Length > 1)
            {
                this.spawn = spawnPointChildren[Random.Range(1, spawnPointChildren.Length)];
            }
            else
            {
                Debug.LogError("No spawn points found under SpawnPointsForPlayer.");
            }
        }
        else
        {
            Debug.LogError("SpawnPointsForPlayer not found. Cannot load spawn point.");
        }
    }
    protected virtual void LoadPlayerCharacter()
    {
        if (this.PlayerCharacter != null) return;

        this.PlayerCharacter = transform;

        if (this.PlayerCharacter == null)
        {
            Debug.LogError("Transform component not found on the object.");
        }
    }
    protected virtual void LoadEnemyBody()
    {
        GameObject enemiesObject = GameObject.Find("Enemies");

        if (enemiesObject != null)
        {
            Transform[] enemyChildren = enemiesObject.GetComponentsInChildren<Transform>(true);

            float closestDistance = Mathf.Infinity;

            foreach (Transform child in enemyChildren)
            {
                if (child != enemiesObject.transform && child.parent == enemiesObject.transform && !child.name.Equals("Death"))
                {
                    float distance = Vector3.Distance(child.position, this.PlayerCharacter.position);

                    if (distance <= visionRadius && distance < closestDistance)
                    {
                        this.enemyBody = child;
                        closestDistance = distance;
                    }
                }
            }

            
        }
        else
        {
            //Debug.LogError("Enemies object not found in the hierarchy.");
        }
    }

    protected virtual void LoadLookPoint()
    {
       
        if (this.enemyBody != null)
        {
            Transform lookPointTransform = this.enemyBody.Find("LookPointEnemy");
            this.LookPoint = lookPointTransform;
        }
    }
}