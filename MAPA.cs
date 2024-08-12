using System;

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
        public Casilla PrimerCasilla { get; private set; }

        public Grid(int filas, int columnas)
        {
            // Crear la primera casilla
            PrimerCasilla = new Casilla(0, 0);

            Casilla actual = PrimerCasilla;

            // Crear el grid usando listas enlazadas
            for (int i = 0; i < filas; i++)
            {
                Casilla filaActual = actual;

                for (int j = 1; j < columnas; j++)
                {
                    Casilla nueva = new Casilla(i, j);
                    actual.Derecha = nueva;
                    nueva.Izquierda = actual;
                    actual = nueva;
                }

                // Si no es la última fila, conectamos la fila actual con la fila de abajo
                if (i < filas - 1)
                {
                    Casilla nuevaFila = new Casilla(i + 1, 0);
                    filaActual.Abajo = nuevaFila;
                    nuevaFila.Arriba = filaActual;
                    actual = nuevaFila;

                    Casilla nodoAnterior = filaActual;
                    Casilla nodoActual = nuevaFila;

                    // Conectar las casillas de la nueva fila con las de la fila anterior
                    for (int j = 1; j < columnas; j++)
                    {
                        nodoAnterior = nodoAnterior.Derecha;
                        Casilla nuevoNodo = new Casilla(i + 1, j);
                        nodoActual.Derecha = nuevoNodo;
                        nuevoNodo.Izquierda = nodoActual;
                        nodoAnterior.Abajo = nuevoNodo;
                        nuevoNodo.Arriba = nodoAnterior;
                        nodoActual = nuevoNodo;
                    }
                }
            }
        }

        public Casilla ObtenerCasilla(int x, int y)
        {
            Casilla actual = PrimerCasilla;

            while (actual != null && actual.X != x)
            {
                actual = actual.Abajo;
            }

            while (actual != null && actual.Y != y)
            {
                actual = actual.Derecha;
            }

            return actual;
        }
    }
}
