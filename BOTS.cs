﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Proyecto2
{
    public class BOTS : MOTO
    {
        public Stack<string> BotPoderesStack { get; private set; } 
        private static Random rand = new Random();
        private Timer botTimer;
        private bool eliminado = false; 
        private PODERESBOT poderesBot;

        public BOTS(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial, GAME game)
            : base(velocidad, tamaño_estela, combustible, items, poderes, posicionInicial, game)
        {
            BotPoderesStack = new Stack<string>();
            poderesBot = new PODERESBOT(this);
            ConfigurarTemporizador();
        }

        public void ConfigurarTemporizador()
        {
            if (botTimer != null)
            {
                botTimer.Stop();
                botTimer.Dispose();
            }

            botTimer = new Timer
            {
                Interval = Velocidad
            };
            botTimer.Tick += MoverBot;
            botTimer.Start();
        }

        public void MoverBot(object sender, EventArgs e)
        {
            if (eliminado)
                return; 

            MAPA mapa = game.mapa;
            int columnas = game.columnas;
            int filas = game.filas;
            MOTO jugador = game.moto;

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

            ActualizarImagenBot(mapa, direccion); 

            if (mapa.EsEstela(PosicionActual))
            {
                eliminado = true; 
                game.MatarBot(this);
            }
        }

        public void ActualizarImagenBot(MAPA mapa, Keys direccion)
        {
            Image imagenActual = ObtenerImagenBot(direccion);
            mapa.ColocarImagenEnCelda(PosicionActual.X, PosicionActual.Y, imagenActual);
        }

        private Image ObtenerImagenBot(Keys direccionActual)
        {
            return direccionActual switch
            {
                Keys.Right => Properties.Resources.BotDerecha,
                Keys.Left => Properties.Resources.BotIzquierda,
                Keys.Up => Properties.Resources.BotArriba,
                Keys.Down => Properties.Resources.BotAbajo,
                _ => Properties.Resources.BotDerecha,
            };
        }
        private bool Mover_(Keys direccion, MAPA mapa, int columnas, int filas)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion(direccion);

            if (!EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                return false;
            }

            if (mapa.EsEstela(nuevaPosicion) || mapa.EsBot(nuevaPosicion) || mapa.EsBomba(nuevaPosicion))
            {
                if (tieneEscudo)
                {
                    MovimientoEfectivo(mapa, nuevaPosicion);
                    return true;
                }
                else
                {
                    eliminado = true;
                    game.MatarBot(this);
                    return false;
                }
            }

            if (nuevaPosicion.TipoItem != null)
            {
                ITEM nuevoItem = new ITEM(nuevaPosicion.TipoItem);
                nuevoItem.Aplicar(this);

                mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
                nuevaPosicion.TipoItem = null;
            }

            if (mapa.EsPoder(nuevaPosicion))
            {
                AplicarPoderBot(nuevaPosicion.TipoPoder);
                nuevaPosicion.TipoPoder = null;
                mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
            }

            MovimientoEfectivo(mapa, nuevaPosicion);

            return true; // Movimiento exitoso
        }

        public void AplicarPoderBot(string tipoPoder)
        {
            BotPoderesStack.Push(tipoPoder); 
            poderesBot.ActivarPoder(tipoPoder);
        }

        public void SoltarItemsYPoderes()
        {
            if (BotPoderesStack.Count == 0) return;

            Random random = new Random();

            while (BotPoderesStack.Count > 0)
            {
                string poder = BotPoderesStack.Pop();
                bool poderColocado = false;

                while (!poderColocado)
                {
                    int x = random.Next(0, game.columnas);
                    int y = random.Next(0, game.filas);
                    Casilla casilla = game.mapa.ObtenerCasilla(x, y);

                    if (casilla != null && !casilla.EsParteDeEstela && casilla.TipoPoder == null && !casilla.EsBot)
                    {
                        Image imagenPoder = ObtenerImagenPoder(poder);
                        game.mapa.ColocarImagenEnCelda(x, y, imagenPoder);
                        casilla.TipoPoder = poder;
                        poderColocado = true;
                    }
                }
            }
        }

        private Image ObtenerImagenPoder(string tipoPoder)
        {
            if (tipoPoder == "Escudo") return Properties.Resources.Escudo;
            else if (tipoPoder == "HiperVelocidad") return Properties.Resources.HiperVelocidad;
            else return null;
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
                if (deltaX > 0)
                {
                    mejorDireccion = Keys.Right;
                }
                else
                {
                    mejorDireccion = Keys.Left;
                }
            }
            else
            {
                if (deltaY > 0)
                {
                    mejorDireccion = Keys.Down;
                }
                else
                {
                    mejorDireccion = Keys.Up;
                }
            }


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
