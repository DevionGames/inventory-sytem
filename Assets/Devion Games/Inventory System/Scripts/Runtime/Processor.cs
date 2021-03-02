using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class Processor : CallbackHandler
    {
        public override string[] Callbacks
        {
            get
            {
                return new string[] { "OnStart","OnFailedToStart", "OnComplete", "OnFailed", "OnStop" };
            }
        }

        public abstract void StartProcess();

        public abstract void StopProcess();

        protected virtual bool CanStart(){ return true; }

    }
}
