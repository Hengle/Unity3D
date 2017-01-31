Pardo Robles, Juan Antonio 5th May 2016 
 
 
 
 	 
Table of contents 
Framework .................................................................................................................................... 3 
Loading the Map ........................................................................................................................... 3 
Agents ........................................................................................................................................... 4 
Sprites ........................................................................................................................................ 4 
Particles ..................................................................................................................................... 4 
Path Planning ................................................................................................................................ 5 
JPS+  ........................................................................................................................................... 5 
How to identify Jump Points ..................................................................................................... 5 
Pseudo Code  ............................................................................................................................. 6 
Summarize .................................................................................................................................... 8 
Conclusion ..................................................................................................................................... 8 
 	  
Framework 
SDL 2.0 has been used as framework in addition to a personal library to manage the memory based on C++ 11 Smart Pointers and an override of vector to provide extra features such as order by Node, pop_back and return the value, among others. 
Loading the Map 
The map is loaded using a function of SDL called GetPixel(..), GetPixel needs 3 values, The first is the SDL_Surface*, and the next values are the position. 
Then, at the start of the program, an image of the map is loaded to the SDL_Surface, and with GetPixel, each pixel is translated into value and saved into a two-dimensional array to be interpreted as wood, water, wall or free cells later. 
  ![alt tag](https://raw.githubusercontent.com/JaPardoRobles/Unity3D/master/f1.PNG)
Figure 1.Translating pixels into numeric values. 
 
A camera had also been created using the SDL_BlitSurface function of SDL 2.0, when the mouse is in a limit of the screen, the camera will move to that direction. This feature allow load a map with 4096x4096 pixels in a screen with 1024x768 pixels. 
 	 
Agents 
Agent is a class created to allow different kind of agents in the game, this class inherit from a sprite class and a particle class. 
Sprite class has been created to draw the sprite image of each agent. 
Particle class has been created to allow the movement and positions of each agent. 
  ![alt tag](https://raw.githubusercontent.com/JaPardoRobles/Unity3D/master/f2.PNG)
Figure 2. Example of a basic agent for testing. 
 	 
 
Path Planning 
JPS+ 
Jump point search plus (JPS+) is an optimal path finding algorithm that can speed up searches on uniform cost grid maps, whit a better result than A* because avoid redundant paths on grids, among others features. Both JPS and JPS+ have a little problem, it not support dynamic maps, but for this exercise is a very good choice. 
Both JPS and JPS+ use a state-space pruning stage. JPS only puts relevant nodes, known as jump points, on the open list. The JPS open list is much reduced by comparison with the A* open list. 
There are four types of Jump Points. Primary Jump points, Straight Jump points, Target jump points and diagonal jump points. 
Jump points are the intermediate points on the map to travel through for at least one optimal path. 
How to identify the jump points 
•	Primary jump points have a forced neighbour. 
•	Straight jump points can be both a primary and straight jump point, are nodes where traveling in a cardinal direction will run into a primary jump point for that direction of travel. 
•	Diagonal jump points are any node which a diagonal direction of travel. 
 	 
Before looking for a path is possible to do a map pre-process to identify all primary jump points, setting a directional flag in each node. Later, it is needed to mark with distance for each direction, looking for walls, straight jumps and diagonal jumps. 
This pre-process allow to the main algorithm search a path only taking into account the jump points, it is faster than traditional JPS because the pre-process erase the recursive step of JPS. 
  ![alt tag](https://raw.githubusercontent.com/JaPardoRobles/Unity3D/master/f3.PNG)
Figure 3. Example of running a complicated path, it only takes 0.006 milliseconds. Red points are Jump Points. 
 
Pseudo code of a path finding: 
Path 	findPath( Node* _start, Node* _goal ){ 
 	OpenList.clear(); 
 	ClosedList.clear();  	Node* startNode;   	startNode.copyNode( _start ); 
 	Node* goalNode; 
goalNode.copyNode( _goal ); 
 
if( goalNode is blocked ) return null; startNode->F = nodeHeuristic( startNode, goalNode ); openList.push( startNode ); while( !openList.empty() ){ 
 	openList.order(); 
 	Node* current = openList.pop_back(); 
 	If( current =  goal ){ 
 	 	While( current != null ) { 
 	 	 	foundPath.push( current ); 
 	 	 	 	current = current->parent; 
} 
Return foundPath; 
} 
 	closedList.push( current );  	vector< Node* > successor = identifySuccessors( current , startNode, goalNode); 
 	For each node( as jumpNode) in successor { 
 	 	If( closedList.find( jumpNode ) != null) break; 
 	 	Int score = current->G + current->distanceTo( jumpNode ); 
 	 	If( score < jumpNode->G OR openList.find( jumpNode) == false ){ 
Node* newJumpNode; newJumpNode  = jumpNode; newJumpNode->parent = current; newJumpNode->G = score; 
newJumpNode->F = newJumpNode->G + nodeHeuristic( newJumpNode, 
goalNode ); if( openList.find( jumpNode ) == false ) openList.push( newJumpNode ); 
else openList.remplace( jumpNode ,  newJumpNode ); 
} 
} 
} 
Return null; ( only if there are not any path) 
} 
 
 	 
Summarize  
There has been talking about what framework and libraries had been used, how the map is charged and translated into values to consult in an array, what is JPS+ and how it works, a pseudo code example and what is the use of Agents.  

