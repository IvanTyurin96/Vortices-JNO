namespace Assets.Scripts.Craft.Parts.Modifiers
{
	using Assets.Scripts.Extensions;
	using ModApi.Craft.Parts;
    using ModApi.GameLoop.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class TrailVortexScript : PartModifierScript<TrailVortexData>
    {
		private ParticleSystem ParticleSystem;
		private GameObject Pyramid;

		private bool _visibleInDesigner;
		private float _size;
		private float _length;
		private float _speed;
		private float _emission;
		private float _randomAngleMultiplier;
		private float _randomLengthMultiplier;
		private int _maxParticles;
		private float _opacity;
		private float _growStartVisibilityAngleOfAttack;
		private float _growEndVisibilityAngleOfAttack;
		private float _fadeStartVisibilityAngleOfAttack;
		private float _fadeEndVisibilityAngleOfAttack;
		private float _minVisibilitySpeed;
		private float _maxVisibilitySpeed;
		private bool _visibleForNegativeAngleOfAttack;

		private const float Ms2Kmh = 3.6f;

		private void Start()
		{
			FindObjects();
			GetAndClampModifiers();
			SetPyramidVisibility();
			TuneVortex();
		}

		private void Update()
		{
			if (Game.InDesignerScene)
			{
				GetAndClampModifiers();
				TuneVortex();
			}

			VortexControl();
		}

		private void FindObjects()
		{
			ParticleSystem = this.transform.FindChildByName("ParticleSystem").GetComponent<ParticleSystem>();
			Pyramid = this.transform.FindChildByName("Pyramid").gameObject;
		}

		private void GetAndClampModifiers()
		{
			_visibleInDesigner = this.Data.VisibleInDesigner;
			_size = Mathf.Clamp(this.Data.Size, 0f, Mathf.Infinity);
			_length = Mathf.Clamp(this.Data.Length, 0f, Mathf.Infinity);
			_speed = Mathf.Clamp(this.Data.Speed, 0f, Mathf.Infinity);
			_emission = Mathf.Clamp(this.Data.Emission, 0f, Mathf.Infinity);
			_randomAngleMultiplier = Mathf.Clamp(this.Data.RandomAngleMultiplier, 0f, Mathf.Infinity);
			_randomLengthMultiplier = Mathf.Clamp(this.Data.RandomLengthMultiplier, 0f, Mathf.Infinity);
			_maxParticles = Mathf.Clamp(this.Data.MaxParticles, 0, System.Int32.MaxValue);
			_opacity = Mathf.Clamp(this.Data.Opacity, 0f, 1f);
			_growStartVisibilityAngleOfAttack = Mathf.Clamp(this.Data.GrowStartVisibilityAOA, 0f, 90f);
			_growEndVisibilityAngleOfAttack = Mathf.Clamp(this.Data.GrowEndVisibilityAOA, 0f, 90f);
			_fadeStartVisibilityAngleOfAttack = Mathf.Clamp(this.Data.FadeStartVisibilityAOA, 0f, 90f);
			_fadeEndVisibilityAngleOfAttack = Mathf.Clamp(this.Data.FadeEndVisibilityAOA, 0f, 90f);
			_minVisibilitySpeed = Mathf.Clamp(this.Data.MinVisibilitySpeed, 0f, Mathf.Infinity);
			_maxVisibilitySpeed = Mathf.Clamp(this.Data.MaxVisibilitySpeed, 0f, Mathf.Infinity);
			_visibleForNegativeAngleOfAttack = this.Data.VisibleForNegativeAngleOfAttack;
		}

		private void SetPyramidVisibility()
		{
			if (Game.InFlightScene)
			{
				Pyramid.gameObject.SetActive(false);
			}
		}

		private void TuneVortex()
		{
			ParticleSystem.MainModule main = ParticleSystem.main;
			float defaultStartSize = 0.25f;
			main.startSize = defaultStartSize * _size;
			main.maxParticles = _maxParticles;

			ParticleSystem.ShapeModule shape = ParticleSystem.shape;
			float defaultRadius = 0.025f;
			shape.radius = defaultRadius * _size;

			ParticleSystem.EmissionModule emission = ParticleSystem.emission;
			float defaultRateOverTime = 1280f;
			emission.rateOverTime = defaultRateOverTime * _emission;

			if (Game.InDesignerScene)
			{
				if (_visibleInDesigner)
				{
					ParticleSystem.gameObject.SetActive(true);
				}
				else
				{
					ParticleSystem.gameObject.SetActive(false);
				}
			}

			if (Game.InFlightScene)
			{
				ParticleSystem.gameObject.SetActive(true);
			}
		}

		private void VortexControl()
		{
			const float MaximalAngleOfAttack = 60f;
			float angleOfAttack = Mathf.Clamp(GetAngleOfAttack(), -MaximalAngleOfAttack, MaximalAngleOfAttack);
			float angleOfSlip = GetAngleOfSlip();

			ParticleSystem.MainModule main = ParticleSystem.main;
			float startSpeed = 40f * _speed;
			float randomLifetimeMultiplier = 0.1f * UnityEngine.Random.Range(-1f, 1f) * _randomLengthMultiplier;
			float startLifetime = 10f / startSpeed * _length + randomLifetimeMultiplier;
			float multiplier = startSpeed / startLifetime;
			main.startLifetime = startLifetime;
			main.startSpeed = startSpeed;

			float speedVisibilityMultiplier = 1f;
			if (Game.InFlightScene)
			{
				Rigidbody rigidbody = this.PartScript.BodyScript.RigidBody;
				speedVisibilityMultiplier = Mathf.InverseLerp(_minVisibilitySpeed, _maxVisibilitySpeed, rigidbody.velocity.magnitude * Ms2Kmh);
			}
			float airDensityMultiplier = 1f;
			float visibilityMultiplier = airDensityMultiplier * speedVisibilityMultiplier * _opacity;
			if (!_visibleForNegativeAngleOfAttack && angleOfAttack < 0f)
			{
				visibilityMultiplier = 0f;
			}
			main.startColor = angleOfAttack >= 0f
				? new Color(1f, 1f, 1f, visibilityMultiplier * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(_growStartVisibilityAngleOfAttack, _growEndVisibilityAngleOfAttack, angleOfAttack)))
				: new Color(1f, 1f, 1f, visibilityMultiplier * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(-_growStartVisibilityAngleOfAttack, -_growEndVisibilityAngleOfAttack, angleOfAttack)));
			if (angleOfAttack > _fadeStartVisibilityAngleOfAttack || angleOfAttack < -_fadeStartVisibilityAngleOfAttack)
			{
				main.startColor = angleOfAttack >= 0f
					? new Color(1f, 1f, 1f, visibilityMultiplier * Mathf.Lerp(1f, 0f, Mathf.InverseLerp(_fadeStartVisibilityAngleOfAttack, _fadeEndVisibilityAngleOfAttack, angleOfAttack)))
					: new Color(1f, 1f, 1f, visibilityMultiplier * Mathf.Lerp(1f, 0f, Mathf.InverseLerp(-_fadeStartVisibilityAngleOfAttack, -_fadeEndVisibilityAngleOfAttack, angleOfAttack)));
			}

			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = ParticleSystem.forceOverLifetime;
			float randomAngle = 10f * _randomAngleMultiplier;
			float randomAngleY = UnityEngine.Random.Range(-randomAngle, randomAngle);
			float randomAngleX = UnityEngine.Random.Range(-randomAngle, randomAngle);
			float forceY = Mathf.Tan((angleOfAttack + randomAngleY) * Mathf.Deg2Rad) * multiplier;
			float forceX = -Mathf.Tan((angleOfSlip + randomAngleX) * Mathf.Deg2Rad) * multiplier;
			forceOverLifetime.y = new(forceY, forceY);
			forceOverLifetime.x = new(forceX, forceX);

			if (Game.InDesignerScene)
			{
				main.startColor = new Color(1f, 1f, 1f, _opacity);
			}
		}

		private float GetAngleOfAttack()
		{
			if (Game.InFlightScene)
			{
				Vector3 velocity = this.PartScript.BodyScript.RigidBody.GetPointVelocity(this.transform.position) + GetVelocityDifference();

				float forwardAngle = Vector3.Angle(this.transform.forward, velocity);
				float angleOfAttack = Vector3.Angle(this.transform.up, velocity) - 90f;
				if (forwardAngle > 90f)
				{
					angleOfAttack = 180f * Mathf.Sign(angleOfAttack) - angleOfAttack;
				}

				return angleOfAttack;
			}

			return 0f;
		}

		private float GetAngleOfSlip()
		{
			if (Game.InFlightScene)
			{
				Vector3 velocity = this.PartScript.BodyScript.RigidBody.GetPointVelocity(this.transform.position) + GetVelocityDifference();

				float forwardAngle = Vector3.Angle(this.transform.forward, velocity);
				float angleOfSlip = Vector3.Angle(this.transform.right, velocity) - 90f;
				if (forwardAngle > 90f)
				{
					angleOfSlip = 180f * Mathf.Sign(angleOfSlip) - angleOfSlip;
				}

				return angleOfSlip;
			}

			return 0f;
		}

		private Vector3 GetVelocityDifference()
		{
			if (Game.InFlightScene)
			{
				Vector3 surfaceVelocity = this.PartScript.BodyScript.SurfaceVelocity;
				Vector3 frameVelocity = this.PartScript.BodyScript.CraftScript.FrameVelocity;
				Vector3 difference = surfaceVelocity - frameVelocity;

				return difference;
			}

			return Vector3.zero;
		}
	}
}