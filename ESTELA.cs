using Proyecto2;
using System.Collections.Generic;
using System.Drawing;

public class Estela
{
    private Queue<(int X, int Y)> posiciones;
    public int MaxLongitud { get; set; }

    public Estela(int maxLongitud)
    {
        posiciones = new Queue<(int, int)>();
        MaxLongitud = maxLongitud;
    }

    public void AgregarNodo(int x, int y, MAPA mapa)
    {
        if (posiciones.Count >= MaxLongitud)
        {
            var nodoEliminado = posiciones.Dequeue();
            mapa.ColorearCelda(nodoEliminado.X, nodoEliminado.Y, Color.MediumPurple);
        }
        posiciones.Enqueue((x, y));
    }

    public IEnumerable<(int X, int Y)> ObtenerPosiciones()
    {
        return posiciones;
    }

    public int Longitud => posiciones.Count;
}
