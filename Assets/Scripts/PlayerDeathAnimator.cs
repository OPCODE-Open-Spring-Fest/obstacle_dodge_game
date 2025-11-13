using UnityEngine;
using System.Collections;

public enum DeathAnimationType
{
    ParticleBlast,
    Sparkle
}

public class PlayerDeathAnimator : MonoBehaviour
{
    [Header("Death Animation Settings")]
    [SerializeField] private DeathAnimationType deathType = DeathAnimationType.ParticleBlast;
    [SerializeField] private float animationDuration = 1.5f;
    [SerializeField] private float delayBeforeGameOver = 0.5f;

    [Header("Particle Blast Settings")]
    [SerializeField] private GameObject particleBlastPrefab;
    [SerializeField] private int particleCount = 50;
    [SerializeField] private float blastForce = 10f;
    [SerializeField] private Color particleColor = Color.white;

    [Header("Sparkle Settings")]
    [SerializeField] private GameObject sparklePrefab;
    [SerializeField] private int sparkleCount = 30;
    [SerializeField] private float sparkleDuration = 2f;
    [SerializeField] private Color sparkleColor = Color.yellow;
    [SerializeField] private float sparkleSpread = 5f;

    private GameObject playerMesh;
    private Renderer[] renderers;
    private Rigidbody rb;
    private Collider playerCollider;
    private bool isAnimating = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length > 0)
        {
            playerMesh = renderers[0].gameObject;
        }
    }

    public void PlayDeathAnimation()
    {
        if (isAnimating) return;
        
        isAnimating = true;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        switch (deathType)
        {
            case DeathAnimationType.ParticleBlast:
                StartCoroutine(ParticleBlastAnimation());
                break;
            case DeathAnimationType.Sparkle:
                StartCoroutine(SparkleAnimation());
                break;
        }
    }

    private IEnumerator ParticleBlastAnimation()
    {
        Vector3 deathPosition = transform.position;

        if (particleBlastPrefab != null)
        {
            GameObject particles = Instantiate(particleBlastPrefab, deathPosition, Quaternion.identity);
            ParticleSystem ps = particles.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = particleColor;
                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0.0f, particleCount)
                });
                var velocity = ps.velocityOverLifetime;
                velocity.enabled = true;
                velocity.space = ParticleSystemSimulationSpace.Local;
                velocity.x = new ParticleSystem.MinMaxCurve(-blastForce, blastForce);
                velocity.y = new ParticleSystem.MinMaxCurve(blastForce * 0.5f, blastForce);
                velocity.z = new ParticleSystem.MinMaxCurve(-blastForce, blastForce);
            }
        }
        else
        {
            CreateParticleBlast(deathPosition);
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        yield return new WaitForSeconds(animationDuration);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }

        yield return new WaitForSeconds(delayBeforeGameOver);

        LastLevelRecorder.SaveAndLoad("gameover3");
    }

    private void CreateParticleBlast(Vector3 position)
    {
        GameObject particleObj = new GameObject("DeathParticles");
        particleObj.transform.position = position;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = particleObj.GetComponent<ParticleSystemRenderer>();
        
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = blastForce;
        main.startColor = particleColor;
        main.maxParticles = particleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, particleCount)
        });
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-blastForce, blastForce);
        velocity.y = new ParticleSystem.MinMaxCurve(blastForce * 0.5f, blastForce);
        velocity.z = new ParticleSystem.MinMaxCurve(-blastForce, blastForce);
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(particleColor, 0.0f), new GradientColorKey(particleColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        ps.Play();
        Destroy(particleObj, animationDuration + 1f);
    }

    private IEnumerator SparkleAnimation()
    {
        Vector3 deathPosition = transform.position;

        if (sparklePrefab != null)
        {
            GameObject sparkles = Instantiate(sparklePrefab, deathPosition, Quaternion.identity);
            ParticleSystem ps = sparkles.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = sparkleColor;
                main.startLifetime = sparkleDuration;
                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0.0f, sparkleCount)
                });
                var velocity = ps.velocityOverLifetime;
                velocity.enabled = true;
                velocity.space = ParticleSystemSimulationSpace.Local;
                velocity.x = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
                velocity.y = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
                velocity.z = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
            }
        }
        else
        {
            CreateSparkleEffect(deathPosition);
        }

        float fadeTime = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    Color color = renderer.material.color;
                    color.a = Mathf.Lerp(1f, 0f, t);
                    renderer.material.color = color;
                }
            }
            
            yield return null;
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        yield return new WaitForSeconds(sparkleDuration - fadeTime);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }

        yield return new WaitForSeconds(delayBeforeGameOver);

        LastLevelRecorder.SaveAndLoad("GameOver");
    }

    private void CreateSparkleEffect(Vector3 position)
    {
        GameObject sparkleObj = new GameObject("SparkleParticles");
        sparkleObj.transform.position = position;
        
        ParticleSystem ps = sparkleObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = sparkleObj.GetComponent<ParticleSystemRenderer>();
        
        var main = ps.main;
        main.startLifetime = sparkleDuration;
        main.startSpeed = sparkleSpread * 0.5f;
        main.startColor = sparkleColor;
        main.maxParticles = sparkleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, sparkleCount)
        });
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;
        
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
        velocity.y = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
        velocity.z = new ParticleSystem.MinMaxCurve(-sparkleSpread, sparkleSpread);
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(sparkleColor, 0.0f), new GradientColorKey(sparkleColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(0.5f, 1.5f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        psr.renderMode = ParticleSystemRenderMode.Billboard;
        psr.material = new Material(Shader.Find("Sprites/Default"));
        
        ps.Play();
        Destroy(sparkleObj, sparkleDuration + 1f);
    }
}

