using UnityEngine;

public class InputHandler : IInputHandler
{
    //return true if mouse button 0 is pressed
    public bool GetMouseButton()
    {
        return Input.GetMouseButton(0);
    }

    // return true if r pressed
    public bool GetRButton()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}