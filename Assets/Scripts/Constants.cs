using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    //on a pas les centre car on resonne pas en terme de faces, juste faut les edge et corner soit bien positionnes et orienter (pas tres clair et chelou en vrai)
    public static readonly int[,] CUBIES_POSITIONS =
    {
        //4 premier de chaque ligne sont les edge, puis les corner
        //dans les edge ca monte jusqu;a 11, car yen a 12 au total
        //dans les corner ca monte a 7 car yen a 8 au total
        //on voit par exemple que dans les edge, ligne 1 on a le 3, ca part de 6 oclock, counterclockwise , donc 0 = edge "bas", 1 = edge droit, etc
        //donc edge 3 de la face UP, c'est a gauche, et on voit que la face LEFT, a aussi l'edge a ID 3, mais elle l'a en premier, car en haut pour la face 3!
        //On pourrait changer et faire un truc qui parte de 12 qui va dans le sens des aiguilles d'une montre  => A FAIRE QUAND CA MARCHE PEUT ETRE PLUS DISCRETION
        //ok en fait les valeurs sont counterclockewise car celui apres (genre 1 vis a vis de 0)
        //c'est celui qui va remplacer quand on va faire tourner clockwise
        //donc si on fait tourner clockwise 1 vient a la position du 0, et 0 a la position du 3, 3 a la pos du 2, etc
        //en fait changer les valeurs ca va foutre la merrde dans des maths compliquees genre les GETID phase 3 avec les bin shift etc
        {  0,  1,  2,  3,  12,  13,  14,  15 },   // U
		{  4,  7,  6,  5,  16,  17,  18,  19 },   // D   //tester de mettre 4 5 6 7, mais et faire les apply move individuellement, mais avec des apply move tout seul sans algo
		{  0,  9,  4,  8,  12,  15,  17,  16 },   // F
		{  2, 10,  6, 11,  14,  13,  19,  18 },   // B
		{  3, 11,  7,  9,  15,  14,  18,  17 },   // L
		{  1,  8,  5, 10,  13,  12,  16,  19 },   // R
        //on fait marcher avec son tableau et apres on remettre dans l'ordre, genre le D ca pourrait pas etre le bordel je pense ca change rien ou on attribue les faces


        //Ca marchait avec ca, mettre ca quand ona finit : valeurs plus ordonnees

        // {  0,  1,  2,  3,  0,  1,  2,  3 },   // U
		// {  4,  5,  6,  7,  4,  5,  6,  7 },   // D
		// {  6,  8,  0,  9,  0,  3,  5,  4 },   // F
		// {  4, 11,  2, 10,  2,  1,  7,  6 },    //B
		// {  7, 9,  3,  11,  3,  2,  6,  5 }, //L
		// {  5,  10,  1, 8,  1,  0,  4,  7 }//R
	};


    // private const int FORWARD = 1;
    // private const int BACKWARD = 2;

    //du coup si on change le tableau nous les positions marquee la sont plus les bonnes mais apparement osef 
    /* These 40 integers represent the entire Rubik's cube state:
	 * 0 - 11:	edge positions		(UF UR UB UL DF DR DB DL FR FL BR BL)	{0, ..., 11} -> yen a 12 (les arretes), l'edge du front, cote haut, c'est la meme edge que celle de la top face, cote front, il existe 12 arretes et 12 edge faces
	 * 12 - 19:	corner positions	(UFR URB UBL ULF DRF DFL DLB DBR)		{12, ..., 19} -> 8 corner 
	 * 20 - 31:	edge orientations	(UF UR UB UL DF DR DB DL FR FL BR BL)	{0, 1} -> edge peut avoir 2 etat, par exemple edge UF (up front), elle peut avoir ses 2 face visible dans un sens ou l'autre (genre rouge en U et blanc en F, ou l'inverse)
	 * 32 - 39:	corner orientations	(UFR URB UBL ULF DRF DFL DLB DBR)		{0, 1, 2} -> corner pareil, 3 face visible, donc 3 etate possible, 32=39 car yen a 8
	 */
    //du coup ca fait pas references au faces, mais plutot aux edges et aux corner, pour dire leur position (debut), puis leur orientation(a partir de 20)
    //donc par exemple ci dessus GOALSTATE[0], dit que l'index 0 doit etre en UF(aka value 0) (voir tableau parenthese ci dessus)
    //GOALSTATE[20] dit que l'edge a cet index, a pour goal d'etre en orientation 0 (une des 3 possible, qui est celle de base et donc le goal)

    public static readonly int[] SOLVED_STATE =
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, // les edge a leur position de goal
		12, 13, 14, 15, 16, 17, 18, 19, // les  corner, et leur position de goal
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // a nouveau  edge et leur orientation de goal
		0, 0, 0, 0, 0, 0, 0, 0 // a nouveau  corner et leur orientation de goal
    };

    public static readonly int[] PHASES_MOVES = { 0, 262143, 259263, 74943, 74898 };

    public static readonly int EDGES_COUNT = 12;
    public static readonly int CORNERS_COUNT = 8;
    public static readonly int FULL_STATE_SIZE = 40;

    //010 010 011 011 010 010
}
