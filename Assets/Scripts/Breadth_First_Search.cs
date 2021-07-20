﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search : MonoBehaviour
{
    private static GameObject A;
    private static GameObject B;
    private static GameObject C;
    private static GameObject Table;

    private static bool move = false;
    public float speed = 1.0f;                                                                                  // Adjust the speed for the application
    private static GameObject start;
    private static GameObject target;
    private static float horizontal_distance = 0f;                                                              //zero movement
    private static float vertical_distance = 0f;


    public static bool Clear(GameObject gameObject)
    {
        //return Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));

        RaycastHit hit;
        var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));
        if (Physics.Raycast(ray, out hit, 25))
        {
            Debug.Log(hit.collider.name);
            return true;
        }
        return false;
    }

    void Update()
    {
        if (move)
        {
            float step = speed * Time.deltaTime; // calculate distance to move                                  // Move our position a step closer to the target.
            if (horizontal_distance != 0f) 
            {
                start.transform.position = Vector3.MoveTowards(new Vector3( 0f, 0f, start.transform.position.z), new Vector3( 0f, 0f, target.transform.position.z), step);
                horizontal_distance = Mathf.Abs(horizontal_distance - start.transform.position.z);
            }
            else if(vertical_distance != 0f){
                start.transform.position = Vector3.MoveTowards(new Vector3(0f, start.transform.position.y, 0f), new Vector3(0f, target.transform.position.y, 0f), step);
                vertical_distance = Mathf.Abs(vertical_distance - start.transform.position.y);
            }
            //start.transform.position = Vector3.MoveTowards(start.transform.position, target.transform.position, step); 
            Debug.Log("should move");
            // Check if the position of the start and target are approximately equal
            if (Vector3.Distance(new Vector3( 0f, start.transform.position.y, 0f), new Vector3( 0f, target.transform.position.y, 0f)) < 0.001f)
            {
                move = false;
            }
        }
    }

    public static State Search(GameObject a, GameObject b, GameObject c, GameObject table) 
    {
        A = a; B = b; C = c; Table = table;
        State initial_state = new State();
        /*A B C
        initial_state.on_top_of.Add(A,Table); 
        initial_state.on_top_of.Add(B, Table); 
        initial_state.on_top_of.Add(C, Table);
        initial_state.Clear(A);
        initial_state.Clear(B);
        initial_state.Clear(C);*/

        //B
        //A  C
        initial_state.on_top_of.Add(A, Table);
        initial_state.on_top_of.Add(B, A);
        initial_state.on_top_of.Add(C, Table);
        initial_state.clear_on_top.Add(A, false);
        initial_state.clear_on_top.Add(B, true);
        initial_state.clear_on_top.Add(C, true);

        if (initial_state.ProblemSolved()) {
            return initial_state;
        }
        Stack<State> search_frontier = new Stack<State>();
        List<State> closed_set = new List<State>();

        search_frontier.Push(initial_state);
        while (search_frontier.Count != 0) 
        {
            State current_state = search_frontier.Pop();                                                    //removes & returns the object at the top of the Stack
            if (current_state.ProblemSolved()) {
                return current_state;
            }
            closed_set.Add(current_state);
            List<State> children = SequentialStates(current_state);
            foreach (State child in children) 
            {
                if (!(closed_set.Contains(child)) || !(search_frontier.Contains(child)))
                {
                    search_frontier.Push(child);
                }
            }
        }
        return null;
    }

    public static List<State> SequentialStates(State current_state) { 
        List<State> children = new List<State>();
        State new_state;

        if (current_state.clear_on_top[A])                                                                  //A block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[A];

            //--------------------------------------move on the table-------------------------------------
            new_state = new State();

            new_state.on_top_of.Add(A, Table);
            new_state.on_top_of.Add(B, current_state.on_top_of[B]);
            new_state.on_top_of.Add(C, current_state.on_top_of[C]);

            new_state.clear_on_top.Add(A, true);
            if (aboveBlock == B)
            {
                new_state.clear_on_top.Add(B, true);
                new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
            }
            else if (aboveBlock == C)
            {
                new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                new_state.clear_on_top.Add(C, true);
            }
            else {
                new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
            }

            if (new_state.validMove())
            {                                                                                               //check for best move
                new_state.parent = current_state;                                                           //keep previous state
                children.Add(new_state);
            }

            //---------------------------------move on top of another block--------------------------------
            if (current_state.clear_on_top[B])                                                              //A can be placed on top of B
            {
                new_state = new State();                                                                    //initialize & construct new state 
                new_state.on_top_of.Add(A,B);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);

                new_state.clear_on_top.Add(B, false);
                new_state.clear_on_top.Add(A, true);
                if (aboveBlock == C)
                {
                    new_state.clear_on_top.Add(C, true);
                }else {
                    new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                }

                if (new_state.validMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }

            if (current_state.clear_on_top[C])                                                              //A can be placed on top of C
            {
                new_state = new State();                                                                    //initialize & construct new state 
                
                new_state.on_top_of.Add(A, C);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);

                
                new_state.clear_on_top.Add(C, false);
                new_state.clear_on_top.Add(A, true);
                if (aboveBlock == B) {
                    new_state.clear_on_top.Add(B, true);
                } else {
                    new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                }

                if (new_state.validMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }
        }

        if (current_state.clear_on_top[B])                                                                  //B block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[B];

            //--------------------------------------move on the table-------------------------------------
            new_state = new State();

            new_state.on_top_of.Add(B, Table);
            new_state.on_top_of.Add(A,current_state.on_top_of[A]);
            new_state.on_top_of.Add(C, current_state.on_top_of[C]);

            new_state.clear_on_top.Add(B, true);
            if (aboveBlock == A)
            {
                new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                new_state.clear_on_top.Add(A, true);
            }
            else if (aboveBlock == C)
            {
                new_state.clear_on_top.Add(C, true);
                new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
            }
            else {
                new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
            }

            if (new_state.validMove())
            {                                                                                               //check for best move
                new_state.parent = current_state;                                                           //keep previous state
                children.Add(new_state);
            }

            //---------------------------------move on top of another block--------------------------------
            if (current_state.clear_on_top[A])                                                              //B can be placed on top of A
            {
                new_state = new State();                                                                    //initialize & construct new state 
                
                new_state.on_top_of.Add(B, A);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);

                new_state.clear_on_top.Add(B, true);
                new_state.clear_on_top.Add(A, false);
                if (aboveBlock == C)
                {
                    new_state.clear_on_top.Add(C, true);
                }
                else {
                    new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                }

                if (new_state.validMove()) {                                                                //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }

            if (current_state.clear_on_top[C])                                                              //B can be placed on top of C
            {
                new_state = new State();
                
                new_state.on_top_of.Add(B, C);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);

                new_state.clear_on_top.Add(B, true);
                new_state.clear_on_top.Add(C, false);
                if (aboveBlock == A) {
                    new_state.clear_on_top.Add(A, true);
                } else{
                    new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                }

                if (new_state.validMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }
        }

        if (current_state.clear_on_top[C])                                                                  //C block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[C];

            //--------------------------------------move on the table-------------------------------------
            new_state = new State();

            new_state.on_top_of.Add(C, Table);
            new_state.on_top_of.Add(A, current_state.on_top_of[A]);
            new_state.on_top_of.Add(B, current_state.on_top_of[B]);

            new_state.clear_on_top.Add(C, true);
            if (aboveBlock == A)
            {
                new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                new_state.clear_on_top.Add(A, true);
            }
            else if (aboveBlock == B) {
                new_state.clear_on_top.Add(B, true);
                new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
            }
            else
            {
                new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
            }

            if (new_state.validMove())
            {                                                                                               //check for best move
                new_state.parent = current_state;                                                           //keep previous state
                children.Add(new_state);
            }

            //---------------------------------move on top of another block--------------------------------
            if (current_state.clear_on_top[A])                                                              //C can be placed on top of A
            {
                new_state = new State();                                                                    //initialize & construct new state 
                
                new_state.on_top_of.Add(C, A);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);

                new_state.clear_on_top.Add(C, true);
                new_state.clear_on_top.Add(A, false);
                if (aboveBlock == B)
                {
                    new_state.clear_on_top.Add(B, true);
                }else {
                    new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                }

                if (new_state.validMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }

            if (current_state.clear_on_top[B])                                                              //C can be placed on top of B
            {
                new_state = new State();
               
                new_state.on_top_of.Add(C, B);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);

                new_state.clear_on_top.Add(B, false);
                new_state.clear_on_top.Add(C, true);
                if (aboveBlock == A)
                {
                    new_state.clear_on_top.Add(A, true);
                }else {
                    new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                }

                if (new_state.validMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }
        }

        return children;
    }

    public static void printSolution(State solution) {
        List<State> path = new List<State>();
        path.Add(solution);
        State parent = solution.parent;
        while (parent != null) {
            path.Add(parent);
            parent = parent.parent;
        }

        Debug.Log("-------------Solution above-------");
        for (int i = 0; i<path.Count; i++) {
            State state = path[path.Count - i - 1];
            Debug.Log("Move " + i);
            Debug.Log("A on top of " + state.on_top_of[A].name);
            Debug.Log("A clear on top " + state.clear_on_top[A] + "\n");

            Debug.Log("B on top of " + state.on_top_of[B].name);
            Debug.Log("B clear on top " + state.clear_on_top[B] + "\n");

            Debug.Log("C on top of " + state.on_top_of[C].name);
            Debug.Log("C clear on top " + state.clear_on_top[C] + "\n");
        }
        return;
    }

    public class State {

        public Dictionary<GameObject, GameObject> on_top_of = new Dictionary<GameObject, GameObject>();     //(key) block on top of block or table (value) 
        public Dictionary<GameObject, bool> clear_on_top = new Dictionary<GameObject, bool>();              //(key) block (value) clear on top
        public State parent;

        public State() { }

        public void On(GameObject b, GameObject x) {                                                        //block b on top of x (block or table)
            RaycastHit hit;
            var ray = new Ray(x.transform.position, x.transform.TransformDirection(Vector3.up));
            if (Physics.Raycast(ray, out hit, 25))
            {
                if (hit.collider.name.Equals(b.name)) {
                    on_top_of.Add(b, x);
                }
            }
        }

        public void Clear(GameObject x) {                                                                   //a block can be placed on top of x
            RaycastHit hit;
            var ray = new Ray(x.transform.position, x.transform.TransformDirection(Vector3.up));
            if (!Physics.Raycast(ray, out hit, 25))
            {
                clear_on_top.Add(x, true);
            }
            else {
                clear_on_top.Add(x, false);
            }
        }

        public string GetOnTop(GameObject gameObject)
        {
            RaycastHit hit;
            var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));
            if (Physics.Raycast(ray, out hit, 25))
            {
                return hit.collider.name;
            }
            return null;
        }

        public bool ProblemSolved() {
            if (on_top_of[A] == B && on_top_of[B] == C && on_top_of[C] == Table) {
                return true;                                                                                //goal state, problem solved
            }
            return false;
        }

        public bool validMove() {
            if (on_top_of[C] == Table) 
            {
                if (on_top_of[B] == C || on_top_of[A] == B) {
                    return true;
                }
            }
            return false;
        }
    }
}
