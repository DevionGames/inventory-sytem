using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{

    public interface IControllerEventHandler
    {
    }

    public interface IControllerGrounded : IControllerEventHandler
    {
        void OnControllerGrounded(bool grounded);
    }

    public interface IControllerAim : IControllerEventHandler
    {
        void OnControllerAim(bool aim);
    }
}