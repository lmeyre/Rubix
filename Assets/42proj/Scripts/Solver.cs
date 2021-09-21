using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Solver : MonoBehaviour
{
    //on a pas les centre car on resonne pas en terme de faces, juste faut les edge et corner soit bien positionnes et orienter (pas tres clair et chelou en vrai)
    private static readonly int[,] AFFECTED_CUBIES =
    {
        //4 premier de chaque ligne sont les edge, puis les corner
        //dans les edge ca monte jusqu;a 11, car yen a 12 au total
        //dans les corner ca monte a 7 car yen a 8 au total
        //on voit par exemple que dans les edge, ligne 1 on a le 3, ca part de 6 oclock, counterclockwise , donc 0 = edge "bas", 1 = edge droit, etc
        //donc edge 3 de la face UP, c'est a gauche, et on voit que la face LEFT, a aussi l'edge a ID 3, mais elle l'a en premier, car en haut pour la face 3!
        //On pourrait changer et faire un truc qui parte de 12 qui va dans le sens des aiguilles d'une montre
        {  0,  1,  2,  3,  0,  1,  2,  3 },   // U
		{  4,  7,  6,  5,  4,  5,  6,  7 },   // D
		{  0,  9,  4,  8,  0,  3,  5,  4 },   // F          voir a quoi servent ces values quand meme
		{  2, 10,  6, 11,  2,  1,  7,  6 },   // B          enfin elles servent a faire le lien avec GOAL state, mais on dirait que pour la partie cornier il aurait pu direct mettre les valeurs de corner au lieu de repartir de 0-8 et rajouter +12 dans l'apply move
		{  3, 11,  7,  9,  3,  2,  6,  5 },   // L
		{  1,  8,  5, 10,  1,  0,  4,  7 },   // R
	};

    private static readonly int[] APPLICABLE_MOVES = { 0, 262143, 259263, 74943, 74898 };

    private const int FORWARD = 1;
    private const int BACKWARD = 2;

    /* These 40 integers represent the entire Rubik's cube state:
	 * 0 - 11:	edge positions		(UF UR UB UL DF DR DB DL FR FL BR BL)	{0, ..., 11} -> yen a 12 (les arretes), l'edge du front, cote haut, c'est la meme edge que celle de la top face, cote front, il existe 12 arretes et 12 edge faces
	 * 12 - 19:	corner positions	(UFR URB UBL ULF DRF DFL DLB DBR)		{12, ..., 19} -> 8 corner 
	 * 20 - 31:	edge orientations	(UF UR UB UL DF DR DB DL FR FL BR BL)	{0, 1} -> edge peut avoir 2 etat, par exemple edge UF (up front), elle peut avoir ses 2 face visible dans un sens ou l'autre (genre rouge en U et blanc en F, ou l'inverse)
	 * 32 - 39:	corner orientations	(UFR URB UBL ULF DRF DFL DLB DBR)		{0, 1, 2} -> corner pareil, 3 face visible, donc 3 etate possible, 32=39 car yen a 8
	 */
    //du coup ca fait pas references au faces, mais plutot aux edges et aux corner, pour dire leur position (debut), puis leur orientation(a partir de 20)
    //donc par exemple ci dessus GOALSTATE[0], dit que l'index 0 doit etre en UF(aka value 0) (voir tableau parenthese ci dessus)
    //GOALSTATE[20] dit que l'edge a cet index, a pour goal d'etre en orientation 0 (une des 3 possible, qui est celle de base et donc le goal)
    private static readonly int[] GOAL_STATE = {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, // les edge a leur position de goal
		12, 13, 14, 15, 16, 17, 18, 19, // les  corner, et leur position de goal
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // a nouveau  edge et leur orientation de goal
		0, 0, 0, 0, 0, 0, 0, 0 // a nouveau  corner et leur orientation de goal
    };

    private int[] currentState;

    [SerializeField] private Text movesLogText;

    void Start()
    {
        this.Reset(new List<AMove>());
    }

    public void DoMove(AMove move)
    {
        this.currentState = this.ApplyMove(move.ToInt(), this.currentState);
    }

    public void UndoMove(AMove move)
    {
        int inverseMove = this.GetInverseMove(move.ToInt());
        this.currentState = this.ApplyMove(inverseMove, this.currentState);
    }

    public void Reset(List<AMove> initialMoves)
    {
        this.currentState = (int[])GOAL_STATE.Clone();
        foreach (AMove move in initialMoves)
            this.currentState = this.ApplyMove(move.ToInt(), this.currentState);
    }

    /* Thistlewaite's Algorithm */
    public List<AMove> Solve()
    {
        int phase = 0;
        List<AMove> moves = new List<AMove>();
        this.movesLogText.text = "[Solver]";

        while (++phase < 5)
        {
            this.movesLogText.text += "\nPhase " + phase + ": ";
            int[] currentID = this.GetID(this.currentState, phase, true);
            int[] goalID = this.GetID(GOAL_STATE, phase);

            if (currentID.SequenceEqual(goalID))
                continue;

            LinkedList<int> moveInts = this.DoBFS(phase, currentID, goalID);

            foreach (int moveInt in moveInts)
            {
                // mutate the current cube representation with this move
                this.currentState = this.ApplyMove(moveInt, this.currentState);

                // add and log this move
                AMove moveObj = MoveFactory.Instance.CreateMoveFromInt(moveInt);
                moves.Add(moveObj);
                this.movesLogText.text += moveObj.ToSymbol() + " ";
            }
        }
        this.LogAllMoves(moves);
        return moves;
    }

    /* Bidirectional Breadth First Search */
    private LinkedList<int> DoBFS(int phase, int[] currentID, int[] goalID)
    {
        // Initialize queue and dictionaries
        Queue<int[]> q = new Queue<int[]>();
        q.Enqueue(this.currentState);
        q.Enqueue(GOAL_STATE);
        //on enqueue les 2 car on fait du bidirectional donc 1 cran de curr vers goal, puis un de goal vers curr
        // et comme on est dans une q ca alterne a chaque fois qu'on en ajoute un a la fin (curr, goal, curr2, goal2, etc)

        Dictionary<int[], int[]> predecessor = new Dictionary<int[], int[]>(new MyIntArrayComparer());
        Dictionary<int[], int> direction = new Dictionary<int[], int>(new MyIntArrayComparer());
        Dictionary<int[], int> lastMove = new Dictionary<int[], int>(new MyIntArrayComparer());
        direction[currentID] = FORWARD;
        direction[goalID] = BACKWARD;

        while (true)
        {
            // Get next state from queue, find its ID and direction
            int[] oldState = q.Dequeue();
            int[] oldID = this.GetID(oldState, phase);
            int oldDir = direction[oldID];

            for (int move = 0; move < 18; move++) // 18 moves car ya 6 faces, qu'on peut faire aller d'un ou 2 cran clockwise ou counter, donc 6 x 4, mais faire avancer de 2 cran en clockwise ou counter clockwise revient au meme donc 6x3
            {
                // only try the allowed moves in the current phase
                if ((APPLICABLE_MOVES[phase] & (1 << move)) > 0) // faudrait bien comprendre les moves store dans le binaire du coup
                {                               // ci dessus, si on est au move 2, ben on decale le "1" de 2 bit, comme ca il est en deuxieme (ou troisieme a voir) position, donc genre 0001 0000 veut dire qu'on a le 5eme move c'est tres simple
                    // generate a new state from the old state
                    int[] newState = this.ApplyMove(move, oldState);
                    int[] newID = this.GetID(newState, phase);
                    int newDir = 0;
                    direction.TryGetValue(newID, out newDir);// equivalent a :    -> a verif mais a priori c'est bon on peut mettre le commentaire a la place
                    // if (direction.ContainsKey(newID))
                    //     newDir = direction[newID];

                    // if we have already found this new state from the other direction, then we can construct a full path
                    if (newDir != 0 && newDir != oldDir)// apparement si on a deja vu ce chemin, si c'est pas celui juste avant d ou on vient, ca suffit a deduire que cest l'autre cote, a voir
                    {                                   //Ce qui est bizarre c'est que l'algo se brute force ca parait tres improbable, il faut surement verifier que ce qu'on trouve c'est bien de l'autre direction
                                                        // Donc a faire gaffe ici, ca parait foireux
                                                        //Aaaaah en fait non ca verifie pas juste le dernier noeud en position, mais en terme de direction
                                                        //donc ca verifie bien si c'est l'oppose !
                                                        // swap directions if necessary
                        if (oldDir == BACKWARD)
                        {
                            int[] tempID = newID;
                            newID = oldID;
                            oldID = tempID;
                            move = this.GetInverseMove(move);
                        }

                        // build a linked list for the moves found in this phase
                        LinkedList<int> moveInts = new LinkedList<int>();
                        moveInts.AddFirst(move);

                        // traverse backward to beginning state
                        while (!oldID.SequenceEqual(currentID))
                        {
                            moveInts.AddFirst(lastMove[oldID]);
                            oldID = predecessor[oldID];
                        }

                        // traverse forward to goal state
                        while (!newID.SequenceEqual(goalID))
                        {
                            moveInts.AddLast(this.GetInverseMove(lastMove[newID]));
                            newID = predecessor[newID];
                        }

                        return moveInts;
                    }
                    // if we have not seen this new state before, add it to queue and dictionaries
                    else if (newDir == 0)
                    {
                        q.Enqueue(newState);
                        direction[newID] = oldDir;
                        lastMove[newID] = move; // pourrait etre mieux fait c'est juste poru lier l'id au move et pouvoir remonter au move d'avant, genre liste chainee
                        predecessor[newID] = oldID;// on pourrait donc le faire avec des objet et node, qui contienne leur move et le precedant mais ptet pas ouf pour la speed
                                                   // on va doncgarder ce format, ou voir si on trouve mieux, mais renommer ca mieux
                    }
                    //ce cas arrive, donc on veut pas un if/else au dessus
                    // else
                    //     Debug.Log("Troisieme cas qui n'arrive jamais ? verif du premier if useless ? (newdir != olddir)");
                }
            }
        }
    }

    private void LogAllMoves(List<AMove> moves)
    {
        this.movesLogText.text += "\n\nMoves: " + moves.Count + "\n\n";
        foreach (AMove move in moves)
            this.movesLogText.text += move.ToSymbol() + " ";
    }

    private class MyIntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked { result = result * 23 + obj[i]; }
            }
            return result;
        }
    }

    /* Move		int	-> inverse int
	 * U		0	-> 2
	 * U2		1	-> 1
	 * U'		2	-> 0
	 * D		3	-> 5
	 * D2		4	-> 4
	 * D'		5	-> 3
	 * F		6	-> 8
	 * F2		7	-> 7
	 * F'		8	-> 6
	 * B		9	-> 11
	 * B2		10	-> 10
	 * B'		11	-> 9
	 * L		12	-> 14
	 * L2		13	-> 13
	 * L'		14	-> 12
	 * R		15	-> 17
	 * R2		16	-> 16
	 * R'		17	-> 15
	 */
    private int GetInverseMove(int move)
    {
        return move + 2 - 2 * (move % 3);
    }

    //ptet refaire cette methode nous meme
    private int[] ApplyMove(int move, int[] state)
    {
        int turns = move % 3 + 1;
        int face = move / 3;
        state = (int[])state.Clone();

        while (turns-- > 0)
        {
            int[] oldState = (int[])state.Clone();
            for (int i = 0; i < 8; i++)// un move influe 8 cubie d'une face (pas le centre)
            {
                // il a mit d'abord les edge en 0 1 2 3 puis les corner en 0 1 2 3 comme ca dans la boucle on separe les corner des edge, corner a i = 5
                int isCorner = Convert.ToInt32(i > 3);
                Debug.Log(i + " -> " + isCorner);
                int target = AFFECTED_CUBIES[face, i] + isCorner * 12;// la il trouve le corner, et jcrois qu'il rajoute 12 si corner (iscorner = 1), pour que le premier corner vaille 12, et pas 0, pour le differentier du premier edge (0)
                Debug.Log(target);                                      //car on traite le tableau qui est au meme format que GOAL state, donc 0-11 edge, puis a partir de 12 les corner commencent
                if (isCorner > 1)
                    Debug.LogError("notre analyze est pas bonne!");
                int killer = AFFECTED_CUBIES[face, (i & 3) == 3 ? i - 3 : i + 1] + isCorner * 12; ;
                int orientationDelta = (i < 4) ? Convert.ToInt32(face > 1 && face < 4) :
                    (face < 2) ? 0 : 2 - (i & 1); // pour capter quel orientation change, pas besoin de comprendre de ouf forcement, ya 2 maniere pour corner ou edge
                state[target] = oldState[killer];
                //au dessus on remplace la position
                //ci dessous l'orientation
                state[target + 20] = oldState[killer + 20] + orientationDelta;
                if (turns == 0)// si on est sur la derniere rotation du move
                    state[target + 20] %= 2 + isCorner;//on remet les valeurs, genre ca va entre 0 et 2, a chque rotation on ajoute de la rotation, si on se retrouve a 4, c'est comme si on etait a 2, on le fait a la fin
            }
        }
        return state;
    }

    /* Get a subset of the current cube state representation relevant for the current phase */
    //en fait ce qui est bien c'est que comme on l'appelle en boucle quand on test les move
    //quand on essai de bien mettre les edge par exemple on copie que cette info et on manipule des donnees plus faible en boucle, en ignorant le reste
    private int[] GetID(int[] state, int phase, bool test = false)
    {
        int[] result;

        switch (phase)
        {
            // Phase 1: Edge orientations
            case 1:
                result = new int[12];
                Array.Copy(state, 20, result, 0, 12); // on copie juste la partie qui parle de l'edge orientation
                break;
            // Phase 2: Corner orientations, E slice edges
            case 2:
                result = new int[8];
                Array.Copy(state, 31, result, 0, 8); // copie juste la partie du goal / current state qui parle de l'orientation des corner
                if (test)
                {
                    Debug.Log("base array ;");
                    foreach (int val in result)
                        Debug.Log(val);
                    Debug.Log("final array ;");
                }
                for (int e = 0; e < 12; e++)
                    result[0] |= (state[e] / 8) << e; // A COMPRENDRE, ou ptet pas, on peut dire que c'etait dans la formule de maths et voila
                if (test)
                    foreach (int val in result)
                        Debug.Log(val);
                break;
            // Phase 3: Edge slices M and S, corner tetrads, overall parity

            // ajout : 
            // The end goal of phase 3 is to get every square on all sides of the cube either correct or the opposite color. 
            // All squares on the Red side should be Red or Orange, all squares on the Blue side should be Blue or Green, etc. 
            // This can be done using only F, B, F^2, B^2, L^2, R^2, U^2, and D^2 moves, and can be done in 13 or fewer moves.
            case 3:
                result = new int[3] { 0, 0, 0 };
                for (int e = 0; e < 12; e++)
                    result[0] |= ((state[e] > 7) ? 2 : (state[e] & 1)) << (2 * e);
                for (int c = 0; c < 8; c++)
                    result[1] |= ((state[c + 12] - 12) & 5) << (3 * c);
                for (int i = 12; i < 20; i++)
                    for (int j = i + 1; j < 20; j++)
                        result[2] ^= Convert.ToInt32(state[i] > state[j]);
                break;
            // Phase 4: Everything
            default:
                result = state;
                break;
        }
        return result;
    }
}
