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
        posiciones.AddLast((x, y));

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
        foreach (var (X, Y) in posiciones)
        {
            mapa.ColocarImagenEnCelda(X, Y, null);
            mapa.ColorearCelda(X, Y, Color.MediumPurple); 
            Casilla casilla = mapa.ObtenerCasilla(X, Y);
            casilla.EsParteDeEstela = false;
            casilla.EsBot = false; 
        }

        posiciones.Clear(); 
    }


}
