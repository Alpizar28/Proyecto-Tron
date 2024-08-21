using Proyecto2;
using System.Collections.Generic;
using System.Drawing;

public class Estela
{
    private LinkedList<(int X, int Y)> posiciones;
    public int MaxLongitud { get; set; }

    public Estela(int maxLongitud)
    {
        posiciones = new LinkedList<(int, int)>();
        MaxLongitud = maxLongitud;
    }

    public void AgregarNodo(int x, int y, MAPA mapa)
    {
        // Agrega un nuevo nodo al final de la lista (la nueva posición)
        posiciones.AddLast((x, y));

        // Si la longitud de la estela supera la longitud máxima, elimina el primer nodo
        if (posiciones.Count > MaxLongitud)
        {
            var nodoEliminado = posiciones.First.Value;
            posiciones.RemoveFirst(); // Elimina el nodo del inicio de la lista
            mapa.ColorearCelda(nodoEliminado.X, nodoEliminado.Y, Color.MediumPurple);
        }
    }

    public IEnumerable<(int X, int Y)> ObtenerPosiciones()
    {
        return posiciones;
    }

    public int Longitud => posiciones.Count;
}
