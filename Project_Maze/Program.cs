using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Maze
{
    class Program
    {
        // Esta función permite imprimir los bordes del laberinto en consola de color amarillo.
        static void paintBorder(char c)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(c);
        }

        // Esta función permite imprimir los bloques o paredes del laberinto en consola de color plomo.
        static void paintWall(char c)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(c);
        }

        // Esta función permite imprimir el camino de solucion del laberinto en consola de color rojo.
        static void paintRoad(char c)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(c);
        }

        // Esta función permite imprimir el texto en consola de color verde.
        static void printText(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
        }

        // Esta función nos permite verificar si la coordenada donde se encontrará el auto es segura.
        static bool isSafe(char[,] maze, int x, int y)
        {
            //Verifica que esté entre las coordenadas del laberinto y en una posicion libre '0'.
            if (x >= 1 && y >= 1 && x <= 30 && y <= 30 && maze[x, y] == '0')
                return true;
            return false;
        }

        // Esta función imprime el laberinto
        static void printMaze(char[,] maze)
        {
            char carac = ' ';
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    // Guarda el caracter de esa posicion y evalua las opciones
                    carac = maze[i, j];
                    switch (carac)
                    {
                        // Si es un camino por donde va el auto lo imprime de otro color
                        case '1':
                            paintRoad(carac);
                            break;
                        // Si es un camino vacio 
                        case '0':
                            paintRoad(' ');
                            break;
                       // Si es un borde imprime de otro color
                        case '█':
                            paintWall(carac);
                            break;
                        // Si no es ninguna de las anteriore opciones, pues se trata de borde del laberinto
                        default:
                            paintBorder(carac);
                            break;
                    }                   
                }
                Console.WriteLine();
            }
        }

        static void findRoad(char[,] maze, int x, int y, int xEnd, int yEnd, List<Step> steps, List<Road> roads)
        {
            // Se verifica si la coordenada actual es igual al final del camino
            if (x == xEnd && y == yEnd)            
            {
                // Si estamos en el final del camino, se crea un nuevo Camino, y se crea un nuevo paso
                Road newRoad = new Road();                
                /* Se agrega la lista de pasos al nuevo camino encontrado, se castea de List<> a IEnumerable<> para que se creen nuevas referencias 
                   en la lista de objetos y no exista problemas.*/
                newRoad.Steps = (steps as IEnumerable<Step>).ToList();     
                // Se agrega el nuevo camino a la lista de caminos que puede recorrer el auto para llegar a la meta.
                roads.Add(newRoad);
                // Se imprime el siguiente texto en consola.
                printText(" FINALIZÓ EL CAMINO CON " + steps.Count() + " PASOS");                                
                // Se Imprime el laberinto con el nuevo camino encontrado.
                printMaze(maze);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                // Se crea un bucle de cuatro iteraciones para que se busque si se puede dar un paso en cualquiera de las 4 direcciones ( →, ↓, ←, ↑)
                {
                    int posX = 0, posY = 0; // Se declaran la posicion en X y en Y que seran reemplazadas por los valores de las coordenadas
                    switch (i)
                    {
                        case 0:
                            // Se verifica si se puede dar un paso hacia la derecha(→) y si es posible se asignan las coordenadas en X, Y
                            if (isSafe(maze, x, y + 1)) { posX = x; posY = y + 1; }
                            break;
                        case 1:
                            // Se verifica si se puede dar un paso hacia abajo(↓) y si es posible se asignan las coordenadas en X, Y
                            if (isSafe(maze, x + 1, y)) { posX = x + 1; posY = y; }
                            break;
                        case 2:
                            // Se verifica si se puede dar un paso hacia la izquierda (←) y si es posible se asignan las coordenadas en X, Y
                            if (isSafe(maze, x, y - 1)) { posX = x; posY = y - 1; }
                            break;
                        case 3:
                            // Se verifica si se puede dar un paso hacia arriba (↑) y si es posible se asignan las coordenadas en X, Y
                            if (isSafe(maze, x - 1, y)) { posX = x - 1; posY = y; }
                            break;
                    }
                    // Se verifica que las variables estén con coordenadas en cualquiera de las 4 direcciones ( →, ↓, ←, ↑)
                    if (posX != 0 && posY != 0)
                    {
                        //Se marca como visitado '1'.
                        maze[posX, posY] = '1';
                        // Se crea un nuevo paso y se añade a la lista de pasos
                        Step s = new Step { PositionX = posX, PositionY = posY};
                        steps.Add(s);
                        // Se busca recursivamente ahora con la posicion del auto en alguna de las sgtes direcciones( →, ↓, ←, ↑).
                        findRoad(maze, posX, posY, xEnd, yEnd, steps, roads);
                        /* CABE RESALTAR QUE SE TRATA DE UN ALGORITMO DE BACKTRACKING (vuelta atrás) ya que se va volviendo a un punto anterior para probar alternativas 
                         en este caso se marca como no visitado '0' para que pueda ser utilizado para otro camino*/
                        maze[posX, posY] = '0';
                        // Se elimina el paso de la lista
                        steps.Remove(s);
                    }                                        
                }
            }
        }



        static void Main(string[] args)
        {            
            int solutionOptimalSteps = 0, numberOfRoad = 0, numberRoadOptimal = 0;
            List<Road> roads = new List<Road>(); // Se crea una lista de Caminos, donde c/u tendrá los pasos que se utilizaron hasta llegar a la meta.
            List<Step> steps = new List<Step>(); // Se crea una lista de pasos que son necesarios para llegar a la meta.

            //Se inicializa la matriz del laberinto, en donde '0' representará los pasos libres que se pueden elegir para que el auto pueda llegar a la meta.
            //El caracter '█' representa una pared o un bloque donde el auto no puede transitar.
            //Se tendrá en cuenta el caracter '1' para marcar que esa posicion es parte del camino de la solucion y el auto puede transitar.
            //El auto inicia en el lugar (1,1)

            char[,] maze = new char[32, 32]
            {
                {'╔','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','╗'},
                {'║','1','0','0','█','█','█','0','0','0','0','0','0','0','0','0','0','0','0','█','█','█','█','█','█','█','█','█','0','0','0','║'},
                {'║','█','█','0','0','0','0','0','█','█','█','█','█','█','█','█','█','█','0','0','0','0','0','0','0','0','0','█','0','█','0','║' },
                {'║','█','█','0','█','█','█','█','█','█','█','█','█','█','█','█','█','█','0','█','█','█','█','█','█','█','0','█','0','█','0','║' },
                {'║','█','█','0','█','0','0','0','█','0','0','0','█','█','0','0','0','0','0','0','0','0','0','0','█','█','0','█','0','0','0','║' },
                {'║','█','0','0','█','0','0','0','█','0','█','0','█','█','0','█','█','█','█','█','█','█','█','0','█','█','0','█','0','█','█','║' },
                {'║','█','0','█','█','0','█','0','█','0','█','0','0','0','0','█','0','█','0','0','0','█','█','0','█','█','0','█','0','█','█','║' },
                {'║','█','0','█','0','0','█','0','█','0','█','█','█','█','█','█','0','█','0','█','0','█','█','0','█','█','0','█','0','█','█','║' },
                {'║','█','0','█','0','█','█','0','█','0','█','█','█','█','█','0','0','█','0','█','0','█','█','0','█','█','0','█','0','█','█','║' },
                {'║','█','0','█','0','0','0','0','█','0','0','0','0','0','█','█','0','█','0','█','0','0','0','0','█','█','0','█','0','█','█','║' },
                {'║','█','0','█','█','█','█','█','█','█','█','█','█','0','█','█','█','█','0','█','█','█','█','█','█','█','0','█','0','0','0','║' },
                {'║','█','0','0','0','0','0','0','█','0','0','0','█','0','█','█','0','0','0','█','0','0','0','0','█','0','0','█','0','█','0','║' },
                {'║','█','█','█','█','█','█','0','█','0','█','0','█','0','█','█','0','█','█','█','█','█','█','█','█','0','█','0','0','█','0','║' },
                {'║','█','█','█','█','█','█','0','█','0','0','0','█','0','█','█','0','0','0','0','0','0','0','█','█','0','█','█','█','█','█','║' },
                {'║','0','0','0','0','0','0','0','█','█','█','█','█','0','█','█','█','█','█','█','█','█','0','█','█','0','0','0','0','0','█','║' },
                {'║','0','█','█','█','█','█','█','█','█','0','0','0','0','█','0','0','0','0','█','0','0','0','█','█','█','█','█','█','0','█','║' },
                {'║','0','█','0','0','0','0','0','0','█','0','█','█','█','█','█','█','█','0','█','0','█','█','█','█','█','█','█','█','0','█','║' },
                {'║','0','█','0','0','0','0','0','0','█','0','0','0','0','0','0','0','█','0','█','0','0','0','0','0','0','0','█','█','0','█','║' },
                {'║','0','█','0','█','█','█','█','█','█','█','█','█','█','█','█','0','█','0','█','█','█','█','█','█','█','0','█','█','0','█','║' },
                {'║','0','█','0','█','0','0','0','0','█','0','0','0','0','0','█','0','█','█','█','█','█','█','█','█','█','0','█','█','0','█','║' },
                {'║','0','█','█','█','0','█','█','0','█','0','█','█','█','0','█','0','0','0','0','0','0','0','0','█','█','0','█','█','0','█','║' },
                {'║','0','0','0','0','0','█','█','0','█','0','█','0','0','0','█','█','█','█','█','█','█','█','0','█','█','0','█','█','0','█','║' },
                {'║','█','█','█','█','█','█','█','0','█','0','█','0','█','█','█','█','█','█','█','█','█','█','0','█','█','0','0','0','0','█','║' },
                {'║','█','0','0','█','0','0','0','0','█','0','█','0','█','0','0','0','0','0','0','0','█','█','0','█','█','█','█','█','0','█','║' },
                {'║','█','0','0','█','0','█','█','█','█','0','█','0','█','0','█','█','█','█','█','0','█','█','0','0','0','█','█','█','0','0','║' },
                {'║','█','0','0','█','0','█','0','0','0','0','█','0','█','0','█','█','█','█','█','0','█','█','█','█','0','0','█','█','█','0','║' },
                {'║','█','0','█','█','0','█','0','█','█','█','█','0','█','0','0','0','0','█','█','0','█','█','0','█','█','0','0','█','█','0','║' },
                {'║','█','0','█','0','0','█','0','0','0','0','0','0','█','█','█','█','0','█','█','0','█','█','0','0','█','█','0','0','0','0','║' },
                {'║','█','0','█','0','█','█','█','█','█','█','█','█','█','0','0','█','0','█','█','0','█','█','█','█','0','█','█','█','█','0','║' },
                {'║','█','0','█','0','0','0','0','0','0','0','0','0','0','█','█','█','0','█','█','0','0','0','0','█','█','█','█','█','█','0','║' },
                {'║','█','█','█','█','█','█','█','█','█','█','█','█','0','0','0','0','0','█','█','█','█','█','0','0','0','0','0','0','0','0','║' },
                {'╚','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','═','╝' }
            };

            //Se llama a la función findRoad (Buscar camino) la cual nos permitirá encontrar los caminos que el auto puede tomar para llegar a la meta.  
            //Se pasa la posicion (1,1) la cual es el inicio del auto y la (30,30) que es la meta.            
            findRoad(maze, 1, 1, 30, 30, steps, roads);

            //Ahora ya tenemos los caminos de las posibles soluciones que puede tomar el auto para llegar a la meta. Se eligirá el más optimo, el de menor pasos.                        
            numberOfRoad = 1;
            foreach(Road r in roads)
            {       
                if(numberOfRoad == 1)
                {
                    // Como tenemos varios caminos iniciaremos con que el primer camino es el más óptimo.                    
                    solutionOptimalSteps = r.Steps.Count();
                    // Se asigna el numero del camino
                    numberRoadOptimal = numberOfRoad;                
                }                    
                else
                {
                    //Se verifica si el camino actual tiene menos pasos que el camino optimo elegido hasta el momento.
                    if (r.Steps.Count() < solutionOptimalSteps)
                    {
                        solutionOptimalSteps = r.Steps.Count();
                        numberRoadOptimal = numberOfRoad;
                    }                        
                }
                // Se posiciona en una coordenada especifica de la consola para imprimir. Solo un detalle para que se pueda observar mejor.
                Console.SetCursorPosition(37, numberOfRoad + 1);
                Console.WriteLine("CAMINO " + numberOfRoad + " => " + r.Steps.Count() + " pasos.");
                // Va sumando el numero de caminos por iteración.
                numberOfRoad++;
            }
            // Se posiciona en una coordenada especifica de la consola para imprimir. Solo un detalle para que se pueda observar mejor.
            Console.SetCursorPosition(37, numberOfRoad + 1);            
            Console.WriteLine("El camino más óptimo es el N° " + numberRoadOptimal + " con " + solutionOptimalSteps + " pasos.");            
            Console.SetCursorPosition(37, numberOfRoad + 2);
            Console.ReadKey();

        }
    }
}
