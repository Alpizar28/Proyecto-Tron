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

    public void IncrementarMaxLongitud(int incremento)
    {
        MaxLongitud += incremento;
    }

    public IEnumerable<(int X, int Y)> ObtenerPosiciones()
    {
        return posiciones;
    }

    public int Longitud => posiciones.Count;

    public void LimpiarEstela(MAPA mapa)
    {
        // Limpiar todas las posiciones de la estela
        foreach (var (X, Y) in posiciones)
        {
            mapa.ColocarImagenEnCelda(X, Y, null); // Eliminar la imagen de la estela
            mapa.ColorearCelda(X, Y, Color.MediumPurple); // Restaurar color original
            Casilla casilla = mapa.ObtenerCasilla(X, Y);
            casilla.EsParteDeEstela = false;  // Desmarcar como estela
            casilla.EsBot = false;  // Asegurar que no esté marcado como bot
        }

        posiciones.Clear();  // Limpiar la lista de posiciones de la estela
    }


}
