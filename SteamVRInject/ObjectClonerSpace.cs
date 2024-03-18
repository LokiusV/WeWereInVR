using UnityEngine;
using WeWereHereVR;
namespace WeWereHereVR
{
    public class ObjectCloner : MonoBehaviour
    {
        

        

        public static void CloneObject(string objectToCloneName,string name)
        {
            GameObject objectToClone = GameObject.Find(objectToCloneName);
            
            GameObject clonedObject = Instantiate(objectToClone);
            clonedObject.name = name;
            
        }
    }
}