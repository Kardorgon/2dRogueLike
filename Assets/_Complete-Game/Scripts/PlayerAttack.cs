using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Completed
{
    public class PlayerAttack : MonoBehaviour
    {
        private Wall wall;
        public int wallDamage = 1;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Attack<T>(T component)
            where T : Component
        {
            //Set hitWall to equal the component passed in as a parameter.
            if (component.transform.GetComponent<Wall>())
            {
                Wall hitWall = component as Wall;
                //Call the DamageWall function of the Wall we are hitting.
                hitWall.DamageWall(wallDamage);
            }

        }
    }
}
