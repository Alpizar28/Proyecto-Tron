using System;

namespace Proyecto2
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
            if (filas <= 0 || columnas <= 0) throw new ArgumentException("Las filas y columnas deben ser mayores que cero.");

            // Crear la primera casilla
            PrimerCasilla = new Casilla(0, 0);

            // Mantener una referencia a la fila anterior
            Casilla filaAnterior = PrimerCasilla;
            Casilla filaActual = PrimerCasilla;

            // Crear el grid usando listas enlazadas
            for (int i = 0; i < filas; i++)
            {
                if (i > 0)
                {
                    filaActual = new Casilla(i, 0);
                    filaAnterior.Abajo = filaActual;
                    filaActual.Arriba = filaAnterior;
                }

                Casilla actual = filaActual;
                Casilla nodoAnterior = filaAnterior;

                for (int j = 1; j < columnas; j++)
                {
                    Casilla nueva = new Casilla(i, j);
                    actual.Derecha = nueva;
                    nueva.Izquierda = actual;

                    if (i > 0)
                    {
                        nodoAnterior = nodoAnterior?.Derecha;
                        if (nodoAnterior != null)
                        {
                            nodoAnterior.Abajo = nueva;
                            nueva.Arriba = nodoAnterior;
                        }

                    }

                    actual = nueva;
                }

                filaAnterior = filaActual;
            }
        }

        public Casilla ObtenerCasilla(int x, int y)
        {
            if (x < 0 || y < 0) return null;

            Casilla actual = PrimerCasilla;

            // Mover hacia abajo hasta la fila correcta
            while (actual != null && actual.X != x)
            {
                actual = actual.Abajo;
            }

            // Ahora mover hacia la derecha hasta la columna correcta
            while (actual != null && actual.Y != y)
            {
                actual = actual.Derecha;
            }

            return actual;
        }

    }
}
