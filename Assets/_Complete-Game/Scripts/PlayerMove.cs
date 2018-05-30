using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{

    public class PlayerMove : MonoBehaviour
    {
        public LayerMask blockingLayer;         //Layer on which collision will be checked.
        public float moveTime = 0.1f;           //Time it will take object to move, in seconds.

        private BoxCollider2D boxCollider;
        private Rigidbody2D rb2D;
        private float inverseMoveTime;          //Used to make movement more efficient.
        public PlayerAttack playerAttack;

        void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            playerAttack = gameObject.GetComponent<PlayerAttack>();

            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
            inverseMoveTime = 1f / moveTime;
        }

        // Update is called once per frame
        void Update()
        {
            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");
            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0)
            {
                vertical = 0;
            }
            //Check if we have a non-zero value for horizontal or vertical
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove(horizontal, vertical);
            }
        }

        void AttemptMove(int xDir, int yDir)
        {
            RaycastHit2D hit;

            bool canMove = Move(xDir, yDir, out hit);


            //Check if nothing was hit by linecast
            if (hit.transform == null)
                //If nothing was hit, return and don't execute further code.
                return;

            //Get a component reference to the component of type T attached to the object that was hit
            if (hit.transform.GetComponent<Wall>())
            {
                Wall wallHit = hit.transform.GetComponent<Wall>();
                //.OnCantMove<Wall>(hit.transform.GetComponent<Wall>());
                playerAttack.Attack<Wall>(wallHit);
                //wallHit.DamageWall(1);

                //Attack Wall in Player abilities script
            }
            if (hit.transform.GetComponent<Enemy>())
            {
                //playerAttack.Attack();               
            }
            //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
            /*if (!canMove && hitComponent != null)

                //Call the OnCantMove function and pass it hitComponent as a parameter.
                OnCantMove(hitComponent);*/
        }
        /*void OnCantMove<T>(T component)
            where T : Component
        {
            Wall wall = component as Wall;
        }*/
        bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 startPosition = transform.position;

            Vector2 endPosition = startPosition + new Vector2(xDir, yDir);

            boxCollider.enabled = false;

            hit = Physics2D.Linecast(startPosition, endPosition, blockingLayer);

            boxCollider.enabled = true;

            if (hit.transform == null)
            {
                //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
                StartCoroutine(SmoothMovement(endPosition));

                return true;
            }
            //If something was hit, return false, Move was unsuccesful.
            return false;

        }
        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected IEnumerator SmoothMovement(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
        }
    }
}
