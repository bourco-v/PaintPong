using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InitialForceApplyer : MonoBehaviour {

	public Vector3 Force;
	public ForceMode ForceMode;

	Rigidbody _rigidBody;

	void OnEnable()
	{
		_rigidBody = GetComponent<Rigidbody>();
		Events.Instance.AddListener<OnGameEndEvent>(HandleOnGameEnd);
		Events.Instance.AddListener<OnGameStartEvent>(HandleOnGameStart);
		_rigidBody.AddForce(Force, ForceMode);
	}

	void HandleOnGameStart(OnGameStartEvent e)
	{
		_rigidBody.AddForce(Force, ForceMode);
	}

	void HandleOnGameEnd(OnGameEndEvent e)
	{
		_rigidBody.velocity = Vector3.zero;
	}
}
