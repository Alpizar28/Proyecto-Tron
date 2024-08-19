using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public class MOTO
    {
        protected GAME game;
        public int Velocidad { get; set; }
        public int Tamaño_Estela { get; set; }
        public int Combustible { get; set; }
        public List<string> Items { get; set; }
        public List<string> Poderes { get; set; }
        public Casilla PosicionActual { get; private set; }
        public Estela Estela { get; private set; }
        private Image motoImageDerecha;
        private Image motoImageIzquierda;
        private Image motoImageArriba;
        private Image motoImageAbajo;

        public MOTO(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial, GAME game)
        {
            Velocidad = velocidad;
            Tamaño_Estela = tamaño_estela;
            Combustible = combustible;
            Items = items;
            Poderes = poderes;
            PosicionActual = posicionInicial;
            Estela = new Estela(tamaño_estela);

            Estela.AgregarNodo(posicionInicial.X, posicionInicial.Y, null);
            this.game = game;
        }

        public void ConfigurarImagenes(Image derecha, Image izquierda, Image arriba, Image abajo)
        {
            motoImageDerecha = derecha;
            motoImageIzquierda = izquierda;
            motoImageArriba = arriba;
            motoImageAbajo = abajo;
        }

        public void Mover(Keys direccion, MAPA mapa, int columnas, int filas)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion(direccion);
            if (EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                if (Combustible > 0)
                {
                    Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
                    PosicionActual = nuevaPosicion;
                    Combustible -= 1;
                }
                else
                {
                    game.FinalizarJuego("Combustible agotado");
                }
            }
            else
            {
                game.FinalizarJuego("Ubicacion invalida");
            }
        }

        public Casilla ObtenerNuevaPosicion(Keys direccion)
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
        public Image ObtenerImagenActual(Keys direccionActual)
        {
            return direccionActual switch
            {
                Keys.Right => motoImageDerecha,
                Keys.Left => motoImageIzquierda,
                Keys.Up => motoImageArriba,
                Keys.Down => motoImageAbajo,
                _ => motoImageDerecha,
            };
        }

        public bool EsPosicionValida(Casilla nuevaPosicion, int columnas, int filas)
        {
            if (nuevaPosicion == null)
            {
                Console.WriteLine("Nueva posición es null");
                return false;
            }

            bool esValida = nuevaPosicion.X >= 0 && nuevaPosicion.X < columnas &&
                            nuevaPosicion.Y >= 0 && nuevaPosicion.Y < filas;

            return esValida;
        }

        public void ActualizarImagen(MAPA mapa, Keys direccion)
        {
            Image imagenActual = ObtenerImagenActual(direccion);
            mapa.ColocarImagenEnCelda(PosicionActual.X, PosicionActual.Y, imagenActual);
        }

        public void ActualizarEstela(MAPA mapa)
        {
            foreach (var (X, Y) in Estela.ObtenerPosiciones())
            {
                mapa.ColorearCelda(X, Y, Color.SkyBlue);
            }
        }
    }
}
