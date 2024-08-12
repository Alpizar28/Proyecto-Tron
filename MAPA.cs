using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2__Tron
{
    public class Casilla
    {
        public Casilla Arriba { get; set; }    // Referencia a la casilla arriba
        public Casilla Abajo { get; set; }     // Referencia a la casilla abajo
        public Casilla Izquierda { get; set; } // Referencia a la casilla izquierda
        public Casilla Derecha { get; set; }   // Referencia a la casilla derecha

        public int X { get; set; } // Posición X (columna) en el grid
        public int Y { get; set; } // Posición Y (fila) en el grid

        public Casilla(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Grid
    {
        public Casilla[,] Malla { get; private set; }

        public Grid(int filas, int columnas)
        {
            Malla = new Casilla[filas, columnas];

            // Crear cada casilla en la malla
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Malla[i, j] = new Casilla(i, j);
                }
            }

            // Conectar las casillas entre sí
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (i > 0) Malla[i, j].Arriba = Malla[i - 1, j];    // Conectar con la casilla de arriba
                    if (i < filas - 1) Malla[i, j].Abajo = Malla[i + 1, j];  // Conectar con la casilla de abajo
                    if (j > 0) Malla[i, j].Izquierda = Malla[i, j - 1]; // Conectar con la casilla a la izquierda
                    if (j < columnas - 1) Malla[i, j].Derecha = Malla[i, j + 1]; // Conectar con la casilla a la derecha
                }
            }
        }
    }


}
