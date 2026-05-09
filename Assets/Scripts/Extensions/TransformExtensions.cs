using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
	/// <summary>
	/// Extensions for Transform component.
	/// </summary>
	public static class TransformExtensions
	{
		/// <summary>
		/// Find the child by contains name.
		/// </summary>
		/// <param name="transform">Transform that contains the child.</param>
		/// <param name="name">The part of name of the child.</param>
		/// <returns>Child`s transform if found, otherwise null.</returns>
		public static Transform FindChildByContainsName(this Transform transform, string name)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if (child.gameObject.name.Contains(name))
				{
					return child;
				}

				child = FindChildByContainsName(child, name);
				if (child != null)
				{
					return child;
				}
			}

			return null;
		}

		/// <summary>
		/// Find the child by name.
		/// </summary>
		/// <param name="transform">Transform that contains the child.</param>
		/// <param name="name">Name of the child.</param>
		/// <returns>Child`s transform if found, otherwise null.</returns>
		public static Transform FindChildByName(this Transform transform, string name)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if (child.gameObject.name == name)
				{
					return child;
				}

				child = FindChildByName(child, name);
				if (child != null)
				{
					return child;
				}
			}

			return null;
		}

		/// <summary>
		/// Transoform the point around pivot.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="rotationAngle">Rotation angles.</param>
		/// <param name="pivot">Pivot.</param>
		/// <returns>Transformed point.</returns>
		public static Vector3 TransformPointAroundPivot(Vector3 point, Vector3 rotationAngle, Vector3 pivot)
		{
			return Quaternion.Euler(rotationAngle.x, rotationAngle.y, rotationAngle.z) * (point - pivot) + pivot;
		}
	}
}
