using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script est� dentro de un prefab que se llama "Bullet"
public class BulletBehaviour : MonoBehaviour
{
    [Header("General Bullet Stats")]
    [SerializeField] private LayerMask whatDestroysBullet;
    [SerializeField] private float destroyTime = 3f;

    [Header("Normal Bullet Stats")]
    [SerializeField] private float normalBulletSpeed = 15f;
    [SerializeField] private float normalBulletDamage = 25f;

    [Header("Physics Bullet Stats")]
    [SerializeField] public float physicsBulletSpeed;
    [SerializeField] private float physicsBulletGravity = 3f;
    [SerializeField] private float physicsBulletDamage = 50f;

    private Rigidbody2D rb;
    private float damage;

    public enum BulletType
    {
        Normal,
        Physics
    }
    public BulletType bulletType;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(EnableCollisionDelay());

        SetDestroyTime();

        // Cambia las propiedades del RigidBody en base al tipo de bala
        SetRBStats();

        // Cambia la velocidad en base al tipo de bala que se dispara
        InitializeBulletStats();
    }

    private void FixedUpdate()
    {
        if (bulletType == BulletType.Physics)
        {
            // Rotar la bala en la direcci�n de velocidad

            transform.right = rb.velocity;
        }
    }

    private void InitializeBulletStats()
    {
        if (bulletType == BulletType.Normal)
        {
            SetStraightVelocity();
            damage = normalBulletDamage;
        }
        else if (bulletType == BulletType.Physics)
        {
            SetPhysicsVelocity();
            damage = physicsBulletDamage;   
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checkea si la colisi�n esta dentro de la "whatDestroysBullet" layerMask

        if ((whatDestroysBullet.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Spawnear particulas

            // Reproducir sonido

            // Temblor de la pantalla

            // Hacer da�o
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damage);
            }

            // Destruir la bala
            Destroy(gameObject);
        }
    }

    // Que la bala vaya recta
    private void SetStraightVelocity()
    {
        rb.velocity = transform.right * normalBulletSpeed;
    }

    private void SetPhysicsVelocity()
    {
        rb.velocity = transform.right * physicsBulletSpeed;
    }

    // Despues de x segundos que la bala desaparezca si no colisiona con nada
    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }
    private void SetRBStats()
    {
        if (bulletType == BulletType.Normal)
        {
            rb.gravityScale = 0f;
        }

        else if (bulletType == BulletType.Physics)
        {
            rb.gravityScale = physicsBulletGravity;
        }
    }

    // Espera un tiempo muy corto para que la bala tenga collider y no colisione con el player que lo dispara.
    private IEnumerator EnableCollisionDelay()
    {
        Collider2D bulletCollider = GetComponent<Collider2D>();
        bulletCollider.enabled = false;
        yield return new WaitForSeconds(0.07f);
        bulletCollider.enabled = true;
    }
}