﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
	public Grid _grid;
    public float _speed = 5.0f;

	Vector3Int _cellPos = Vector3Int.zero;    
	bool _isMoving = false;
	Animator _animator;

	MoveDir _dir = MoveDir.Down;
	public MoveDir Dir
	{
		get { return _dir; }
		set 
		{
			if (_dir == value)
				return;

			switch (value)
			{
				case MoveDir.Up:
					_animator.Play("WALK_BACK");
					transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					break;
				case MoveDir.Down:
					_animator.Play("WALK_FRONT");
					transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					break;
				case MoveDir.Left:
					_animator.Play("WALK_RIGHT");
					transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
					break;
				case MoveDir.Right:
					_animator.Play("WALK_RIGHT");
					transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					break;
				case MoveDir.None:
					if (_dir == MoveDir.Up)
					{
						_animator.Play("IDLE_BACK");
						transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					}
					else if (_dir == MoveDir.Down)
					{
						_animator.Play("IDLE_FRONT");
						transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					}
					else if (_dir == MoveDir.Left)
					{
						_animator.Play("IDLE_RIGHT");
						transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
					}
					else
					{
						_animator.Play("IDLE_RIGHT");
						transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					}
					break;
			}

			_dir = value;
		}
	}

	void Start()
    {
		_animator = GetComponent<Animator>();
		Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
		transform.position = pos;
	}

    void Update()
    {
		GetDirInput();
		UpdatePosition();
		UpdateIsMoving();		
	}

	// 키보드 입력
    void GetDirInput()
	{
		if (Input.GetKey(KeyCode.W))
		{
			Dir = MoveDir.Up;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Dir = MoveDir.Down;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			Dir = MoveDir.Left;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Dir = MoveDir.Right;
		}
		else
		{
			Dir = MoveDir.None;
		}
	}

	// 스르륵 이동하는 것을 처리
	void UpdatePosition()
	{
		if (_isMoving == false)
			return;

		Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
		Vector3 moveDir = destPos - transform.position;

		// 도착 여부 체크
		float dist = moveDir.magnitude;
		if (dist < _speed * Time.deltaTime)
		{
			transform.position = destPos;
			_isMoving = false;
		}
		else
		{
			transform.position += moveDir.normalized * _speed * Time.deltaTime;
			_isMoving = true;
		}
	}

	// 이동 가능한 상태일 때, 실제 좌표를 이동한다
	void UpdateIsMoving()
	{
		if (_isMoving == false)
		{
			switch (_dir)
			{
				case MoveDir.Up:
					_cellPos += Vector3Int.up;
					_isMoving = true;
					break;
				case MoveDir.Down:
					_cellPos += Vector3Int.down;
					_isMoving = true;
					break;
				case MoveDir.Left:
					_cellPos += Vector3Int.left;
					_isMoving = true;
					break;
				case MoveDir.Right:
					_cellPos += Vector3Int.right;
					_isMoving = true;
					break;
			}
		}
	}
}
