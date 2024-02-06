using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : EnemyProperties
{

    [Header("Enemy Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;



    [Header("Random Point")]
    public bool isMovingRandom = false;
    private Vector3 randomDestination;
    protected override void Awake()
    {
        base.Awake();
        presentHealth = enemyHealth;
        checkPlayerVision = shootingRadius - 2f;
    }

    private void Update()
    {
        if (!playerInvisionRadius) CheckAndMoveRandom();

        LoadPlayerBody();
        LoadLookPoint();


        CheckBoolPlayer();

        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if (playerBody == null || LookPoint == null)
        {
            playerInshootingRadius = false;
            playerInvisionRadius = false;
        }



        if (playerInvisionRadius && !playerInshootingRadius) Purpsueplayer();
        if (playerInvisionRadius && playerInshootingRadius) ShootPlayer();
    }
    protected virtual void CheckBoolPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkPlayerVision, playerLayer);

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject.name == "Player")
            {
                isPlayer = true;
                return;
            }
        }
        isPlayer = false;
    }
    private void Purpsueplayer()
    {
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled && enemyAgent.isOnNavMesh)
        {
            if (playerBody != null)
            {
                if (enemyAgent.SetDestination(playerBody.position))
                {
                    anim.SetBool("Running", true);
                    anim.SetBool("Shooting", false);
                }
                else
                {
                    anim.SetBool("Running", false);
                    anim.SetBool("Shooting", false);
                }
            }

        }
    }


    private void ShootPlayer()
    {
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled && enemyAgent.isOnNavMesh)
        {
            enemyAgent.SetDestination(transform.position);
        }
        transform.LookAt(LookPoint);

        if (!previouslyShoot)
        {
            muzzuleSpark.Play();
            audioSource.PlayOneShot(shootingSound);

            RaycastHit hit;
            //Debug.DrawRay(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward * shootingRadius, Color.red, 0.5f);

            if (Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enviroment") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Vector3 preferredShootingPosition = FindPreferredShootingPosition();
                    if (preferredShootingPosition != Vector3.zero)
                    {
                        enemyAgent.SetDestination(preferredShootingPosition);
                        return;
                    }
                    anim.SetBool("Running", true);
                    anim.SetBool("Shooting", false);
                }
                else
                {
                    if (isPlayer == true)
                    {
                        PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>();
                        if (playerBody != null)
                        {
                            playerBody.playerHitDamage(giveDamage);
                        }
                    }
                    else
                    {
                        PlayerAI playerBody = hit.transform.GetComponent<PlayerAI>();
                        if (playerBody != null)
                        {
                            playerBody.PlayerAIHitDamage(giveDamage);
                        }
                    }
                    anim.SetBool("Running", false);
                    anim.SetBool("Shooting", true);
                }



            }

            previouslyShoot = true;
            Invoke(nameof(ActiveShooting), timebtwShoot);
        }
    }
    private Vector3 FindPreferredShootingPosition()
    {
        Vector3[] surroundingPoints = GetSurroundingPoints(transform.position, 360f, 10f);

        List<Vector3> validShootingPoints = FilterValidShootingPoints(surroundingPoints);

        if (validShootingPoints.Count > 0)
        {
            return validShootingPoints[Random.Range(0, validShootingPoints.Count)];
        }
        return Vector3.zero;
    }

    private Vector3[] GetSurroundingPoints(Vector3 center, float angle, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        int numPoints = 36;

        for (int i = 0; i < numPoints; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * (i * (360f / numPoints));
            Vector3 point = center + new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians)) * radius;
            points.Add(point);
        }

        return points.ToArray();
    }

    private List<Vector3> FilterValidShootingPoints(Vector3[] points)
    {
        List<Vector3> validPoints = new List<Vector3>();

        foreach (var point in points)
        {
            RaycastHit hit;
            if (!Physics.Raycast(point, transform.forward, out hit, shootingRadius))
            {
                validPoints.Add(point);
            }
        }

        return validPoints;
    }



    private void CheckAndMoveRandom()
    {
        if (!isMovingRandom)
        {
            Vector3 randomPos;
            if (RandomPoint(transform.position, 85f, out randomPos))
            {
                //Debug.Log("Random Destination: " + randomDestination);

                randomDestination = randomPos;
                isMovingRandom = true;

                enemyAgent.SetDestination(randomDestination);
                anim.SetBool("Running", true);
                anim.SetBool("Shooting", false);
            }

        }

        if (isMovingRandom && Vector3.Distance(transform.position, randomDestination) < 10f)
        {
            isMovingRandom = false;
            CheckAndMoveRandom();
        }
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(randomDestination, 1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, randomDestination);
    }
    */


    private bool RandomPoint(Vector3 startPoint, float range, out Vector3 result)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint = startPoint + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.5f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }


    private void ActiveShooting()
    {
        previouslyShoot = false;
    }
    public void enemyHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if (presentHealth <= 0 && !GameManagers.instance.survivalMode)
        {
            StartCoroutine(Respawn());
        }
        else if (presentHealth <= 0 && GameManagers.instance.survivalMode)
        {
            //gameObject.SetActive(false);
            //   transform.GetComponent<Transform>().gameObject.SetActive(false);
            //gameObject.layer = 12;

            EnemyDeath();
            Destroy(gameObject);
        }
    }
    IEnumerator Respawn()
    {

        EnemyDeath();
        yield return new WaitForSeconds(3f);

        //Debug.Log("spawn");

        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        presentHealth = Mathf.RoundToInt(Random.Range(105f, 150f));
        enemySpeed = 3f;
        shootingRadius = Mathf.RoundToInt(Random.Range(14f, 25f));
        visionRadius = Mathf.RoundToInt(Random.Range(25f, 35f));
        checkPlayerVision = shootingRadius - 2f;
        playerInvisionRadius = true;
        playerInshootingRadius = false;

        anim.SetBool("Die", false);
        anim.SetBool("Running", true);
        LoadSpawn();

        enemyCharacter.transform.position = spawn.transform.position;
        Purpsueplayer();
    }
    public void EnemyDeath()
    {
        if (enemyAgent != null && enemyAgent.isActiveAndEnabled && enemyAgent.isOnNavMesh)
        {
            enemyAgent.SetDestination(transform.position);
        }
        enemySpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        playerInvisionRadius = false;
        playerInshootingRadius = false;
        anim.SetBool("Die", true);
        //anim.SetBool("Running", false);
        //anim.SetBool("Shooting", false);
        //Debug.Log("Dead");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        scoreManager.kills += 1;
    }
}