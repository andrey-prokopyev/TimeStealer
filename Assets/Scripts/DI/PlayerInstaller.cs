﻿using System;
using Control;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField]
    private Components components;

    public override void InstallBindings()
    {
        this.Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle()
            .WithArguments(this.components.CharacterController, this.components.ViewPort);

        this.Container.BindInterfacesAndSelfTo<PlayerAnimator>().AsSingle().WithArguments(this.components.Animator);
    }

    [Serializable]
    public class Components
    {
        public CharacterController CharacterController;
        public Transform ViewPort;
        public Animator Animator;
    }
}
