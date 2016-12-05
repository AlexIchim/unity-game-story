using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private bool _isFacingRight;
    private CharacterController2D _controller;
    private float _normalizedHorizontalSpeed;

    private float MaxSpeed = 8;
    private float SpeedAccelerationOnGround = 10f;
    private float SpeedAccelerationInAir = 5f;
    public bool IsDead { get; private set; }

    public void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        Debug.Log("STATE IS : " + _controller);
        _isFacingRight = transform.localScale.x > 0;
    }

    public void Update()
    {
        if (!IsDead)
            HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        // ReSharper disable ConvertIfStatementToConditionalTernaryExpression

        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
    }

    /* Level Management */
    public void Kill()
    {
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;

        _controller.SetForce(new Vector2(0, 20));
    }

    public void RespawnAt(Transform spawnPoint)
    {
        if (!_isFacingRight)
            Flip();

        IsDead = false;
        GetComponent<Collider2D>().enabled = true;
        _controller.HandleCollisions = true;

        transform.position = spawnPoint.position;
    }



    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }

        if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }

}
