using Godot;
using System;

namespace RTS.Gameplay
{
    [GlobalClass]
    public abstract partial class Ability:Resource
    {
        public abstract void OnClick();
        public Texture2D Icon;//TODO: Need to get it from somewhere also should probably set it as abstract too so it has to be set

        public abstract string Text { get; }//So everyone has to implement it
        private Key shortcut = Key.None;
        public Key Shortcut { get => shortcut; set => shortcut = value; }
    }
}

