using System;
using UnityEngine;
using View;

public class DuckBehaviour
{
	public event Action OnCaught;
	public event Action OnScared;
    private DuckView _view;

	private float _time;
	private float _timeTillIdleChange;

	private float _distance = 0;
	private Vector3 _lastPosition = Vector3.zero;

	public DuckBehaviour(DuckView view)
	{
		_view = view;

		_view.OnCaught += DuckCaught;
		_view.OnScared += DuckScared;

		GetRandomTime();
	}

	private void DuckScared(object sender, EventArgs e)
	{
		OnScared?.Invoke();
	}

	private void DuckCaught(object sender, EventArgs e)
	{
		OnCaught?.Invoke();
	}

	public void Update()
	{
		if(_distance <= _view.MaxDistance)
			CheckIdleChange();

		CheckSpeed();
	}

	private void CheckSpeed()
	{
		_view.Animator.SetFloat("DuckDistance", _distance);
	}

	public void FixedUpdate()
	{
		if (_view.FollowTarget != null)
		{
			FollowTarget();
			LookAtTarget();
		}
	}

	private void CheckIdleChange()
	{
		if(_time >= _timeTillIdleChange)
		{
			_view.Animator.SetTrigger("OnIdle2");
			GetRandomTime();
			_time = 0;
		}

		_time += Time.deltaTime;
	}

	private void GetRandomTime()
	{
		_timeTillIdleChange = UnityEngine.Random.Range(_view.TimeUntilIdleChange.x, _view.TimeUntilIdleChange.y);
	}

	private void LookAtTarget()
	{
		_view.Transform.LookAt(_view.FollowTarget.position);
	}

	private void FollowTarget()
	{
		_distance = Vector3.Distance(_view.transform.position, _view.FollowTarget.position);
		if (_distance >= _view.MaxDistance)
		{
			_view.transform.position = Vector3.Lerp(_view.Transform.position, _view.FollowTarget.position - (_view.FollowTarget.forward * _view.TargetOffset), _view.FollowSpeed);
		}
		else
		{
			_lastPosition = _view.transform.position;
			_distance = 0;
		}
	}
}