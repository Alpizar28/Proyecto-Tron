using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Proyecto2
{
    public class BOTS : MOTO
    {
        private static Random rand = new Random();

        public BOTS(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial, GAME game)
            : base(velocidad, tamaño_estela, combustible, items, poderes, posicionInicial, game)
        {
        }

        public Keys ObtenerDireccionAleatoria()
        {
            var direcciones = new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
            return direcciones[rand.Next(direcciones.Length)];
        }

        public void MoverBot(MAPA mapa, MOTO jugador, int columnas, int filas)
        {
            Keys direccion = ObtenerMejorDireccion(jugador, mapa, columnas, filas);
            Casilla nuevaPosicion = ObtenerNuevaPosicion(direccion);

            if (EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                if (mapa.EsEstela(nuevaPosicion))
                {
                    game.MatarBot(this);
                }
                else
                {
                    Mover(direccion, mapa, columnas, filas);
                }
            }
            else
            {
                // Intentar otras direcciones si la inicial no es válida
                var direcciones = new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
                foreach (var dir in direcciones)
                {
                    nuevaPosicion = ObtenerNuevaPosicion(dir);
                    if (EsPosicionValida(nuevaPosicion, columnas, filas) && !mapa.EsEstela(nuevaPosicion))
                    {
                        Mover(dir, mapa, columnas, filas);
                        break;
                    }
                }
            }
        }


        private Keys ObtenerMejorDireccion(MOTO jugador, MAPA mapa, int columnas, int filas)
        {
            int deltaX = jugador.PosicionActual.X - this.PosicionActual.X;
            int deltaY = jugador.PosicionActual.Y - this.PosicionActual.Y;

            Keys mejorDireccion = Keys.Right;

            // Elige la dirección que acerque al jugador
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                mejorDireccion = deltaX > 0 ? Keys.Right : Keys.Left;
            }
            else
            {
                mejorDireccion = deltaY > 0 ? Keys.Down : Keys.Up;
            }

            // Verifica si la dirección es válida (dentro del mapa y no es estela)
            Casilla nuevaPosicion = ObtenerNuevaPosicion(mejorDireccion);
            if (EsPosicionValida(nuevaPosicion, columnas, filas) && !mapa.EsEstela(nuevaPosicion))
            {
                return mejorDireccion;
            }


            // Si la mejor dirección no es válida, prueba otras direcciones
            var direcciones = new List<Keys> { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
            foreach (var dir in direcciones)
            {
                nuevaPosicion = ObtenerNuevaPosicion(dir);
                if (EsPosicionValida(nuevaPosicion, columnas, filas) && !mapa.EsEstela(nuevaPosicion))
                {
                    return dir;
                }
            }

            // Si ninguna dirección es válida, mantén la misma dirección
            return mejorDireccion;
        }
    }
}
