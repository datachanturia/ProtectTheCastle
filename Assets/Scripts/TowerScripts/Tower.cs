using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    [SerializeField]
    private float timeBetweenAttacks;
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private Projectile projectile;
    private Enemy targetEnemy = null;

    private float attackCounter;
    private bool isAttacking = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        attackCounter -= Time.deltaTime;
        if(targetEnemy == null || targetEnemy.IsDead)
        {
            Enemy nearestEnemy = GetNearestEnemyInRange();
            if(nearestEnemy != null)
            {
                targetEnemy = nearestEnemy;
            }
        } else
        {
            if(attackCounter <= 0)
            {
                isAttacking = true;
                attackCounter = timeBetweenAttacks;
            } else
            {
                isAttacking = false;
            }
            if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
            {
                targetEnemy = null;
            }
        }
	}

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            Attack();
        }
    }

    public void Attack()
    {
        isAttacking = false;
        Projectile newProjectile = Instantiate(projectile) as Projectile;
        newProjectile.transform.localPosition = transform.localPosition;
        //sounds of projectiles
        if(newProjectile.ProjectileType == projectileType.rock)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        } else if(newProjectile.ProjectileType == projectileType.arrow)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        } else if(newProjectile.ProjectileType == projectileType.fireball)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
        }
        if(targetEnemy == null)
        {
            Destroy(newProjectile);
        }
        else
        {
            // hit to enemy projectile
            StartCoroutine(MoveProjectile(newProjectile));
        }
    }

    IEnumerator MoveProjectile(Projectile projectile)
    {
        while (getTargetDistance(targetEnemy) > 0.2f && projectile != null && targetEnemy != null)
        {
            var dir = targetEnemy.transform.localPosition - transform.localPosition;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
            yield return null;
        }
        if(projectile != null || targetEnemy == null)
        {
            Destroy(projectile);
        }
    }

    private float getTargetDistance(Enemy thisEnemy)
    {
        if(thisEnemy == null)
        {
            thisEnemy = GetNearestEnemyInRange();
            if(thisEnemy == null)
            {
                return 0f;
            }
        }
        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
    }

    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        foreach(Enemy enemy in GameManager.Instance.EnemyList)
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }

    private Enemy GetNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach(Enemy enemy in GetEnemiesInRange())
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}
