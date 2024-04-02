using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WeWereHereVR
{
    public class TriggerProvider
    {

        public static bool CheckPressed()
        {
            if(!Var.triggerDone&& Input.GetAxis("joy_1_axis_2")>=0.8f) 
            {
                Var.triggerDone = true;
                return true;
            
            }
            if (Var.triggerDone&& Input.GetAxis("joy_1_axis_2") <= 0.3f)
            {
                Var.triggerDone = false;
                return false;
            }
            return false;
            
        }
    }
    
}
