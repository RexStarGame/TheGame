using UnityEngine;
using System.Collections;

public class JulemandensBomber : MonoBehaviour
{
    [Header("Bomberens Indstillinger")]
    public int skade = 20;
    public float eksplosionsRadius = 3f;
    public LayerMask skadeLag;

    [Header("Animation og Timing")]
    public Animator Animator;
    public float skadeForsinkelse = 0.5f; // Tid før skade påføres under eksplosionen
    public float animationTid = 1.5f; // Total varighed af eksplosionens animation

    [Header("Visuel Advarsel")]
    public GameObject radiusVisningPrefab;
    public float visningOffset = 0.5f;
    private GameObject radiusVisning;

    private bool harEksploderet = false;

    private void Start()
    {
        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }

        if (radiusVisningPrefab != null)
        {
            radiusVisning = Instantiate(radiusVisningPrefab, transform.position, Quaternion.identity);
            float diameter = (eksplosionsRadius + visningOffset) * 2;
            radiusVisning.transform.localScale = new Vector3(diameter, diameter, 1f);
            radiusVisning.transform.parent = transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!harEksploderet)
        {
            StartEksplosion();
        }
    }

    private void StartEksplosion()
    {
        if (harEksploderet) return;
        harEksploderet = true;

        if (Animator != null)
        {
            Animator.SetTrigger("IsExploded"); // Start eksplosionens animation
        }

        // Forsinket skade baseret på `skadeForsinkelse`
        StartCoroutine(PåførSkadeMedForsinkelse());

        // Vent på animationens afslutning før destruktion
        StartCoroutine(AfvantAnimationOgDestruer());
    }

    private IEnumerator PåførSkadeMedForsinkelse()
    {
        yield return new WaitForSeconds(skadeForsinkelse);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, eksplosionsRadius, skadeLag);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(skade);
                }
            }
            else if (collider.CompareTag("Enemies"))
            {
                EnemyHealthBar enemyHealth = collider.GetComponent<EnemyHealthBar>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(skade);
                }
            }
        }
    }

    private IEnumerator AfvantAnimationOgDestruer()
    {
        yield return new WaitForSeconds(animationTid);

        if (radiusVisning != null)
        {
            Destroy(radiusVisning);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eksplosionsRadius);
    }
}
