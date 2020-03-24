using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static InputHelpers;
/// <summary>
/// it may be very easy in the future to make this even more generic as something like heInteract. not sure.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class HeAttac : MonoBehaviour
{
    public float AttackRate = 1f;
    private AudioSource audio;
    
    [HideInInspector]
    private HeParticipate _heParticipate;
    [HideInInspector]
    private bool _alreadyAttacking;
    public bool IsPlayer = false;


    public InputActionAsset asset;

    private InputAction inputAction;
    private ButtonControl buttonControl;


    public Action AttackAction;
    public Predicate<object> AttackWhile;
    public Func<YieldInstruction> AttackRateInstruction;
    // Start is called before the first frame update
    void Awake()
    {
        audio = GetComponent<AudioSource>();
        _heParticipate = GetComponent<HeParticipate>();
        
        if (IsPlayer)
        {
            inputAction = asset.FindAction("Player/Fire");
            // Getting the first binding of the input action using index of 0. If we had more bindings, we would use different indices.
            buttonControl = (ButtonControl)inputAction.controls[0];
            inputAction.Enable();

            if (_heParticipate != null)
                _heParticipate.TeamName = Teams.Player.ToString();
            //InputController.RightMouseClick.AddListener(a => StartCoroutine(Attac()) );
        }else
        {
            TriggerAttack(new InputAction.CallbackContext());
        }

    }

    public void TriggerAttack(InputAction.CallbackContext context)
    {
        ///Debug.Log("Attack Triggered");
        AttackAction = AttackAction ?? new Action(()=>{
            //Debug.Log("Firing");
            if (CanAttack())
                Attack();
        });

        AttackWhile = AttackWhile ?? new Predicate<object>(
            (o) =>
            {
                ///Debug.Log("Checking Attack");
                return !IsPlayer || (IsPlayer && buttonControl.isPressed);
            }
        );

        AttackRateInstruction = AttackRateInstruction ?? (()=>new WaitForSeconds(AttackRate));

        this.StartSingletonCoroutine(AttackAction, AttackWhile, AttackRateInstruction);
    }
    //public IEnumerator Attac()
    //{
    //    if (!_alreadyAttacking)
    //    {
    //        _alreadyAttacking = true;
    //        while () //InputController.IsLeftMouseDown))
    //        {
    //            Debug.Log("Firing");
    //            if (CanAttack())
    //                Attack();
    //            yield return new WaitForSeconds(AttackRate);
    //        }
    //        Debug.Log("Stop Firing");

        //        _alreadyAttacking = false;
        //    }
        //}

    public virtual void Attack()
    {
        audio.Play();
        //attack anyone but your team. if no team then everyone
        var teamName = _heParticipate?.TeamName;
        foreach(var team in HeParticipate.ParticipatingTeams)
        {
            if(team.Key != teamName)
            {
                team.Value?.ForEach(
                    t=>Damage(t)
                );
            }
        }
    }

    public virtual bool CanAttack()
    {
        return true;
    }

    public virtual void Damage(HeParticipate participant)
    {
        Debug.Log($"{participant.name} was damaged by {name}!!");
    }
}
