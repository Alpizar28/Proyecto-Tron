using System;
using System.Collections.Generic;

namespace Proyecto2
{
    public class Nodo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Nodo Siguiente { get; set; }

        public Nodo(int x, int y)
        {
            X = x;
            Y = y;
            Siguiente = null;
        }
    }

    public class Estela
    {
        public Nodo Cabeza { get; private set; }
        public int Longitud { get; private set; }
        public int MaxLongitud { get; set; }

        public Estela(int maxLongitud)
        {
            Cabeza = null;
            Longitud = 0;
            MaxLongitud = maxLongitud;
        }

        public void AgregarNodo(int x, int y)
        {
            Nodo nuevoNodo = new Nodo(x, y);
            nuevoNodo.Siguiente = Cabeza;
            Cabeza = nuevoNodo;
            Longitud++;

            if (Longitud > MaxLongitud)
            {
                EliminarUltimoNodo();
            }
        }

        public void EliminarUltimoNodo()
        {
            if (Cabeza == null || Cabeza.Siguiente == null)
            {
                Cabeza = null;
                Longitud = 0;
                return;
            }

            Nodo actual = Cabeza;
            while (actual.Siguiente.Siguiente != null)
            {
                actual = actual.Siguiente;
            }

            actual.Siguiente = null;
            Longitud--;
        }
    }
}
