using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour 
{
	CharacterController _controller;
	Transform _player;
	
	[SerializeField]
	float _moveSpeed = 5.0f;
	
	[SerializeField]
	float _gravity = 2.0f;
	
	float _yVelocity = 0.0f;
		
	
	void Start()
	{
		GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
		_player = playerGameObject.transform;
		
		_controller = GetComponent < CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 direction = _player.position - transform.position;
		direction.Normalize ();
		
		Vector3 velocity = direction * _moveSpeed;
		
		if(!_controller.isGrounded)
		{
			_yVelocity -= _gravity;
		}
		
		velocity.y = _yVelocity;
		
		// AudioVelocityUpdateMode.y = _yVelocity;
		
		direction.y = 0;		
		transform.rotation = Quaternion.LookRotation(direction);
		
		_controller.Move(velocity * Time.deltaTime);
	}
}
