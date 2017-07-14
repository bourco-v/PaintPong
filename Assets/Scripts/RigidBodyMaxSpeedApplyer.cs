using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyMaxSpeedApplyer : MonoBehaviour {

	public Rigidbody RigidBody { get; protected set; }
	[SerializeField]
	private float MaxSpeed;

	bool Freeze = true;

	void OnEnable()
	{
		Events.Instance.AddListener<OnStateChangedEvent>(HandleOnStateChanged);
		RigidBody = GetComponent<Rigidbody>();
	}

	void HandleOnStateChanged(OnStateChangedEvent e)
	{
		switch (e.State)
		{
			case EGameState.Game:
				Freeze = false;
				break;
			default:
				Freeze = true;
				break;
		}
	}

	void FixedUpdate()
	{
		if (Freeze == false)
			RigidBody.velocity = RigidBody.velocity.normalized * MaxSpeed;
		else
			RigidBody.velocity = Vector3.zero;
	}
}
