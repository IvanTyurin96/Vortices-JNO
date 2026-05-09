namespace Assets.Scripts.Craft.Parts.Modifiers
{
	using Assets.Scripts.Design;
	using ModApi.Craft.Parts;
    using ModApi.Craft.Parts.Attributes;
	using ModApi.Design.PartProperties;
	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using UnityEngine;

    [Serializable]
    [DesignerPartModifier("LerxVortex")]
    [PartModifierTypeId("Vortices.LerxVortex")]
    public class LerxVortexData : PartModifierData<LerxVortexScript>
    {
		[DesignerPropertyToggleButton(Label = "Visible in Designer")]
		public bool VisibleInDesigner = true;

		[DesignerPropertySlider(0.1f, 5f, 50, Label = "Size")]
		public float Size = 1f;

		[DesignerPropertySlider(0f, 2f, 21, Label = "Random size")]
		public float RandomSize = 0.25f;

		[DesignerPropertySlider(0.1f, 10f, 100, Label = "Length")]
		public float Length = 1f;

		[DesignerPropertySlider(0.1f, 2f, 20, Label = "Speed")]
		public float Speed = 1f;

		[DesignerPropertySlider(0.1f, 5f, 50, Label = "Emission")]
		public float Emission = 1f;

		[DesignerPropertySlider(0.1f, 4f, 40, Label = "Random angle")]
		public float RandomAngleMultiplier = 1f;

		[DesignerPropertySlider(0.1f, 2f, 20, Label = "AoA sensitivity")]
		public float AngleOfAttackSensitivity = 0.5f;

		[DesignerPropertySlider(0.1f, 2f, 20, Label = "AoS sensitivity")]
		public float AngleOfSlipSensitivity = 1f;

		[DesignerPropertySlider(0f, 60f, 61, Label = "Maximal AoA")]
		public float MaxAngleOfAttack = 60f;

		[DesignerPropertySlider(0f, 60f, 61, Label = "Maximal AoS")]
		public float MaxAngleOfSlip = 60f;

		[DesignerPropertySlider(3000f, 30000f, 10, Label = "Max particles count")]
		public int MaxParticles = 3000;

		[DesignerPropertySlider(0.1f, 1f, 10, Label = "Opacity")]
		public float Opacity = 1f;

		[DesignerPropertySlider(0f, 90f, 91, Label = "Grow start AoA")]
		public float GrowStartVisibilityAOA = 5f;

		[DesignerPropertySlider(0f, 90f, 91, Label = "Grow end AoA")]
		public float GrowEndVisibilityAOA = 10f;

		[DesignerPropertySlider(0f, 90f, 91, Label = "Fade start AoA")]
		public float FadeStartVisibilityAOA = 25f;

		[DesignerPropertySlider(0f, 90f, 91, Label = "Fade end AoA")]
		public float FadeEndVisibilityAOA = 60f;

		[DesignerPropertySlider(50f, 300f, 251, Label = "Min visibility speed, km/h")]
		public float MinVisibilitySpeed = 100f;

		[DesignerPropertySlider(50f, 300f, 251, Label = "Max visibility speed, km/h")]
		public float MaxVisibilitySpeed = 150f;

		[DesignerPropertyToggleButton(Label = "Visible for -AoA")]
		public bool VisibleForNegativeAngleOfAttack = true;

		protected override void OnDesignerInitialization(IDesignerPartPropertiesModifierInterface d)
		{
			base.OnDesignerInitialization(d);

			d.OnAnyPropertyChanged(delegate
			{
				Symmetry.SynchronizePartModifiers(base.Script.PartScript);
			});
		}
	}
}