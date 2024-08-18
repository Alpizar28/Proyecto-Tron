using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public class BOTS : MOTO
    {
        private static Random rand = new Random();

        public BOTS(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial)
            : base(velocidad, tamaño_estela, combustible, items, poderes, posicionInicial)
        {
        }

        public Keys ObtenerDireccionAleatoria()
        {
            var direcciones = new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
            return direcciones[rand.Next(direcciones.Length)];
        }

        public void MoverBot(MAPA mapa, int columnas, int filas)
        {
            Keys direccion = ObtenerDireccionAleatoria();
            Mover(direccion, mapa, columnas, filas);
        }


        private Casilla ObtenerNuevaPosicion(Keys direccion)
        {
            return direccion switch
            {
                Keys.Up => PosicionActual.Arriba,
                Keys.Down => PosicionActual.Abajo,
                Keys.Left => PosicionActual.Izquierda,
                Keys.Right => PosicionActual.Derecha,
                _ => null,
            };
        }
    }
}
