using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Proyecto2
{
    public class BOTS : MOTO
    {
        private static Random rand = new Random();
        private Timer botTimer;
        private bool eliminado = false; // Flag para evitar múltiples eliminaciones

        public BOTS(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial, GAME game)
            : base(velocidad, tamaño_estela, combustible, items, poderes, posicionInicial, game)
        {
            ConfigurarTemporizador();
        }

        private void ConfigurarTemporizador()
        {
            botTimer = new Timer
            {
                Interval = Velocidad // Intervalo en milisegundos específico para este bot
            };
            botTimer.Tick += MoverBot; // Asociar el evento de movimiento del bot
            botTimer.Start();
        }

        public void MoverBot(object sender, EventArgs e)
        {
            if (eliminado)
                return; // Evita mover el bot si ya ha sido eliminado

            MAPA mapa = game.mapa;
            int columnas = game.columnas;
            int filas = game.filas;
            MOTO jugador = game.moto;

            // Desmarcar la casilla anterior como "EsBot"
            PosicionActual.EsBot = false;

            Keys direccion = ObtenerMejorDireccion(jugador, mapa, columnas, filas);
            bool movimientoExitoso = Mover_(direccion, mapa, columnas, filas);

            if (!movimientoExitoso)
            {
                // Intentar otras direcciones si la inicial no es válida o hay colisión con la estela
                var direccionesAlternativas = new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
                foreach (var dir in direccionesAlternativas)
                {
                    if (dir != direccion && Mover_(dir, mapa, columnas, filas))
                    {
                        break;
                    }
                }
            }

            PosicionActual.EsBot = true;

            ActualizarEstela(mapa);
            ActualizarImagen(mapa, direccion);
            if (mapa.EsEstela(PosicionActual))
            {
                eliminado = true; // Marca el bot como eliminado
                game.MatarBot(this);  // Eliminar el bot si choca con una estela
            }
        }

        private bool Mover_(Keys direccion, MAPA mapa, int columnas, int filas)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion(direccion);

            if (!EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                return false; // La nueva posición no es válida, no se realiza el movimiento
            }

            // Si la nueva posición es una estela o contiene otro bot
            if (mapa.EsEstela(nuevaPosicion) || mapa.EsBot(nuevaPosicion))
            {
                if (tieneEscudo)
                {
                    // El bot tiene escudo, ignora la colisión
                    // Avanza a la nueva posición y continúa el juego
                    Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
                    PosicionActual = nuevaPosicion;
                    Combustible -= 1;
                    return true;
                }
                else
                {
                    // El bot no tiene escudo, es eliminado
                    eliminado = true;
                    game.MatarBot(this);
                    return false;
                }
            }

            // Si la nueva posición contiene un poder
            if (mapa.EsPoder(nuevaPosicion))
            {
                AplicarPoderBot(nuevaPosicion.TipoPoder);
                nuevaPosicion.TipoPoder = null;
                mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
            }

            // Mueve el bot a la nueva posición
            Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
            PosicionActual = nuevaPosicion;
            Combustible -= 1;

            return true; // Movimiento exitoso
        }



        public void AplicarPoderBot(string tipoPoder)
        {
            if (tipoPoder == "Escudo")
            {
                Poderes.ActivarEscudo();
            }
            else if (tipoPoder == "HiperVelocidad")
            {
                Poderes.ActivarHiperVelocidad();
            }
        }

        public Keys ObtenerDireccionAleatoria()
        {
            var direcciones = new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
            return direcciones[rand.Next(direcciones.Length)];
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

        public void DetenerBot()
        {
            botTimer.Stop();
        }
    }
}
