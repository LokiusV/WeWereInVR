using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WeWereHereVR
{
    //Attention: requires cooldown to work
    public class SnapTurnProvider
    {
        //positive
        public static bool SnapRight()
        {
            if (!Var.snapDone && Input.GetAxis("joy_1_axis_3") >= 0.7f)
            {
                Var.snapDone = true;
                return true;

            }
            if (Var.snapDone && Input.GetAxis("joy_1_axis_3") <= 0.25f)//&& Input.GetAxis("joy_1_axis_3")>=-0.25f)
            {
                Var.snapDone = false;
                return false;
            }
            return false;

        }

        //negative
        public static bool SnapLeft()
        {
            if (!Var.snapDone && Input.GetAxis("joy_1_axis_3") <= -0.7f)
            {
                Var.snapDone = true;
                return true;

            }
            if (Var.snapDone && Input.GetAxis("joy_1_axis_3") >= -0.25f)//&& Input.GetAxis("joy_1_axis_3") <= 0.25f) //wayyyyy too high to fix this shit rn
            {
                Var.snapDone = false;
                return false;
            }
            return false;

        }
        public static void Snap()
        {
            if(SnapRight())
            {
                try
                {
                    Var.playerGameObject.transform.Rotate(0, 45, 0);
                }
                catch (Exception e) { Debug.LogException(e); }
            }
            if (SnapLeft())
            {
                try
                {
                    Var.playerGameObject.transform.Rotate(0, -45, 0);
                }
                catch (Exception e) { Debug.LogException(e); }
            }

        }


    }
}
