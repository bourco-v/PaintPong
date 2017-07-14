using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		protected Brush brush = null;

		[SerializeField]
		private int wait = 3;

		private int waitCount;

		public void SetColor()
		{
			GetComponent<MeshRenderer>().material.color = brush.Color;
		}

		public void Awake()
		{
			SetColor();
		}

		public void FixedUpdate()
		{
			++waitCount;
		}

		protected bool IsPaintCollisionReady()
		{
			if (waitCount < wait)
				return false;
			waitCount = 0;
			return (true);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			if(waitCount < wait)
				return;
			waitCount = 0;

			foreach(var p in collision.contacts)
			{
				var canvas = p.otherCollider.GetComponent<InkCanvas>();
				if(canvas != null)
					canvas.Paint(brush, p.point);
			}
		}
	}
}