using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : PlayerAIProperties
{
    [Header("Player Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;

    [Header("Random Point")]
    private bool isMovingRandom = false;
    private Vector3 randomDestination;
    protected override void Awake()
    {
        base.Awake();
        PlayerAgent = GetComponent<NavMeshAgent>();
        presentHealth = PlayerHealth;
    }

    private void Update()
    {
        if (!enemyInvisionRadius) CheckAndMoveRandom();

        LoadEnemyBody();
        LoadLookPoint();

        enemyInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, enemyLayer);
        enemyInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, enemyLayer);

        if (enemyBody == null || LookPoint == null)
        {
            enemyInvisionRadius = false;
            enemyInshootingRadius = false;
        }

        if (enemyInvisionRadius && !enemyInshootingRadius) PurpsueEnemy();
        if (enemyInvisionRadius && enemyInshootingRadius) ShootEnemy();
    }
    private void PurpsueEnemy()
    {
        if (PlayerAgent != null && PlayerAgent.isActiveAndEnabled && PlayerAgent.isOnNavMesh)
        {
            if (enemyBody != null)
            {
                if (PlayerAgent.SetDestination(enemyBody.position))
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


    private void ShootEnemy()
    {
        if (PlayerAgent != null && PlayerAgent.isActiveAndEnabled && PlayerAgent.isOnNavMesh)
        {
            PlayerAgent.SetDestination(transform.position);
        }


        transform.LookAt(LookPoint);

        if (!previouslyShoot)
        {
            muzzuleSpark.Play();
            audioSource.PlayOneShot(shootingSound);
            RaycastHit hit;
            //Debug.DrawRay(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward * shootingRadius, Color.green, 1.0f);
            if (Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enviroment") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Vector3 preferredShootingPosition = FindPreferredShootingPosition();
                    if (preferredShootingPosition != Vector3.zero)
                    {
                        PlayerAgent.SetDestination(preferredShootingPosition);
                        return;
                    }
                    anim.SetBool("Running", true);
                    anim.SetBool("Shooting", false);
                }
                else
                {
                    Enemy enemy = hit.transform.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.enemyHitDamage(giveDamage);
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
            if (RandomPoint(transform.position, 70f, out randomPos))
            {
                randomDestination = randomPos;
                isMovingRandom = true;

                PlayerAgent.SetDestination(randomDestination);
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
        for (int i = 0; i < 1; i++)
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
    public void PlayerAIHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if (presentHealth <= 0 && !GameManagers.instance.survivalMode)
        {
            StartCoroutine(Respawn());
        }
        else if (presentHealth <= 0 && GameManagers.instance.survivalMode)
        {
            gameObject.SetActive(false);
            //gameObject.layer = 12;
            PlayerAIDeath();
            Destroy(gameObject);
        }
    }
    IEnumerator Respawn()
    {

        PlayerAIDeath();
        yield return new WaitForSeconds(3f);

        //Debug.Log("spawn");

        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        presentHealth = Mathf.RoundToInt(Random.Range(110f, 120f));
        PlayerSpeed = 3f;
        shootingRadius = Mathf.RoundToInt(Random.Range(10f, 20f));
        visionRadius = Mathf.RoundToInt(Random.Range(20f, 25f));
        enemyInvisionRadius = true;
        enemyInshootingRadius = false;

        anim.SetBool("Die", false);
        anim.SetBool("Running", true);
        LoadSpawn();
        PlayerCharacter.transform.position = spawn.transform.position;
        PurpsueEnemy();
    }
    public void PlayerAIDeath()
    {
        if (PlayerAgent != null && PlayerAgent.isActiveAndEnabled && PlayerAgent.isOnNavMesh)
        {
            PlayerAgent.SetDestination(transform.position);
        }

        PlayerSpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        enemyInvisionRadius = false;
        enemyInshootingRadius = false;
        anim.SetBool("Die", true);
        anim.SetBool("Running", false);
        anim.SetBool("Shooting", false);
        //Debug.Log("Dead");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        scoreManager.enemyKills += 1;
    }
}