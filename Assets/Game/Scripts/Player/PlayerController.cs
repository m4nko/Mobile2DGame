using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer2D.Character;

[RequireComponent(typeof(CharacterMovement2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterFacing2D))]
[RequireComponent(typeof(IDamageable))]
public class PlayerController : MonoBehaviour
{
    CharacterMovement2D playerMovement;
    CharacterFacing2D playerFacing;
    PlayerInput playerInput;
    IDamageable damageable;
    public IWeapon weapon { get; private set; }

    [SerializeField] GameObject weaponObject;

    [Header("Camera")]
    [SerializeField] Transform cameraTarget;
    
    [Range(0.0f, 5.0f)]
    [SerializeField] float cameraTargetOffsetX = 2.0f;

    [Range(0.5f, 50.0f)]
    [SerializeField] float cameraTargetFlipSpeed = 2.0f;
    
    [Range(0.0f, 5.0f)]
    [SerializeField] float characterSpeedInfluence = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<CharacterMovement2D>();
        playerInput = GetComponent<PlayerInput>();
        playerFacing = GetComponent<CharacterFacing2D>();
        damageable = GetComponent<IDamageable>();

        damageable.DeathEvent += OnDeath; // Quando o evento for invocado, a função OnDamage será chamada

        if (weaponObject != null)
        {
            weapon = weaponObject.GetComponent<IWeapon>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Movimentacao
        Vector2 movementInput = playerInput.GetMovementInput();
        playerMovement.ProcessMovementInput(movementInput);

        playerFacing.UpdateFacing(movementInput);

        //Pulo
        if (playerInput.IsJumpButtonDown())
        {
            playerMovement.Jump();
        }
        if (playerInput.IsJumpButtonHeld() == false)
        {
            playerMovement.AbortJump();
        }

        //Agachar
        if (playerInput.IsCrouchButtonDown())
        {
            playerMovement.Crouch();
        }
        else if (playerInput.IsCrouchButtonUp())
        {
            playerMovement.UnCrouch();
        }

        //Atacar
        if (playerInput.IsAttackButtonDown() && weapon != null)
        {
            weapon.Attack();
        }
    }

    private void FixedUpdate()
    {
        // Controle do target da camera dependendo da direcao do sprite e da velocidade do jogador
        bool isFacingRight = playerFacing.IsFacingRight();
        float targetOffsetX = isFacingRight ? cameraTargetOffsetX : -cameraTargetOffsetX;

        float currentOffsetX = Mathf.Lerp(cameraTarget.localPosition.x, targetOffsetX, Time.fixedDeltaTime * cameraTargetFlipSpeed);

        currentOffsetX += playerMovement.CurrentVelocity.x * Time.fixedDeltaTime * characterSpeedInfluence;

        cameraTarget.localPosition = new Vector3(currentOffsetX, cameraTarget.localPosition.y, cameraTarget.localPosition.z);
        //
    }

    private void OnDestroy()
    {
        if (damageable != null)
        {
            damageable.DeathEvent -= OnDeath;
        }
    }

    private void OnDeath()
    {
        playerMovement.StopImmediately();
        enabled = false;
    }
}
