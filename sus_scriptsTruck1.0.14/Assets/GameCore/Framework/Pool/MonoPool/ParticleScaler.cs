/*
using Framework;

using UnityEngine;

[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour, IPooledMonoBehaviour
{
	public float particleScale = 1f;

	public bool alsoScaleGameobject = true;

	private float prevScale = 1f;

	[HideInInspector]
	public float initScale = 1f;

	[HideInInspector]
	public bool scriptGenerated;

	private bool m_gotten;

	public void OnCreate()
	{
	}

	public void OnGet()
	{
		if (!m_gotten)
		{
			m_gotten = true;
			prevScale = particleScale;
			initScale = particleScale;
			if (scriptGenerated && particleScale != 1f)
			{
				prevScale = 1f;
				CheckAndApplyScale();
			}
		}
	}

	public void OnRecycle()
	{
		m_gotten = false;
	}

	private void Start()
	{
		OnGet();
	}

	public void RevertApplyScale()
	{
		particleScale = initScale;
		CheckAndApplyScale();
	}

	public void CheckAndApplyScale()
	{
		if (prevScale != particleScale && particleScale > 0f)
		{
			if (alsoScaleGameobject)
			{
				base.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
			}
			float scaleFactor = particleScale / prevScale;
			ScaleLegacySystems(scaleFactor);
			ScaleShurikenSystems(scaleFactor);
			ScaleTrailRenderers(scaleFactor);
			prevScale = particleScale;
		}
	}

	private void Update()
	{
	}

	private void ScaleShurikenSystems(float scaleFactor)
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startSpeed *= scaleFactor;
			particleSystem.startSize *= scaleFactor;
			particleSystem.gravityModifier *= scaleFactor;
			particleSystem.Stop();
			particleSystem.Play();
		}
	}

	private void ScaleLegacySystems(float scaleFactor)
	{
		ParticleEmitter[] componentsInChildren = base.GetComponentsInChildren<ParticleEmitter>(true);
		ParticleAnimator[] componentsInChildren2 = base.GetComponentsInChildren<ParticleAnimator>(true);
		ParticleEmitter[] array = componentsInChildren;
		foreach (ParticleEmitter particleEmitter in array)
		{
			particleEmitter.minSize *= scaleFactor;
			particleEmitter.maxSize *= scaleFactor;
			ParticleEmitter particleEmitter2 = particleEmitter;
			particleEmitter2.worldVelocity *= scaleFactor;
			ParticleEmitter particleEmitter3 = particleEmitter;
			particleEmitter3.localVelocity *= scaleFactor;
			ParticleEmitter particleEmitter4 = particleEmitter;
			particleEmitter4.rndVelocity *= scaleFactor;
			particleEmitter.ClearParticles();
			particleEmitter.Emit();
		}
		ParticleAnimator[] array2 = componentsInChildren2;
		foreach (ParticleAnimator particleAnimator in array2)
		{
			ParticleAnimator particleAnimator2 = particleAnimator;
			particleAnimator2.force *= scaleFactor;
			ParticleAnimator particleAnimator3 = particleAnimator;
			particleAnimator3.rndForce *= scaleFactor;
		}
	}

	private void ScaleTrailRenderers(float scaleFactor)
	{
		TrailRenderer[] componentsInChildren = base.GetComponentsInChildren<TrailRenderer>(true);
		TrailRenderer[] array = componentsInChildren;
		foreach (TrailRenderer trailRenderer in array)
		{
			trailRenderer.startWidth *= scaleFactor;
			trailRenderer.endWidth *= scaleFactor;
		}
	}
}
*/