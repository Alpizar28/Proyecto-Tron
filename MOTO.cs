using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public class MOTO
    {
        public GAME game;
        public int Velocidad { get; set; }
        public int Tamaño_Estela { get; set; }
        public int Combustible { get; set; }
        public List<string> Items { get; set; }
        public Stack<string> PoderesStack { get; set; }
        public Casilla PosicionActual { get; protected set; }
        public Estela Estela { get; private set; }
        private Image motoImageDerecha;
        private Image motoImageIzquierda;
        private Image motoImageArriba;
        private Image motoImageAbajo;

        public bool tieneEscudo { get; set; }
        public PODERES Poderes { get; private set; }

        public MOTO(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial, GAME game)
        {
            Velocidad = velocidad;
            Tamaño_Estela = tamaño_estela;
            Combustible = combustible;
            Items = items;
            PoderesStack = new Stack<string>(poderes);
            PosicionActual = posicionInicial;
            Estela = new Estela(tamaño_estela);
            Estela.AgregarNodo(posicionInicial.X, posicionInicial.Y, null);
            this.game = game;
            Poderes = new PODERES(this);
        }

        public void ConfigurarImagenes(Image derecha, Image izquierda, Image arriba, Image abajo)
        {
            motoImageDerecha = derecha;
            motoImageIzquierda = izquierda;
            motoImageArriba = arriba;
            motoImageAbajo = abajo;
        }

        public virtual void Mover(Keys direccion, MAPA mapa, int columnas, int filas)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion(direccion);

            if (EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                if (!tieneEscudo)
                {
                    if (mapa.EsEstela(nuevaPosicion))
                    {
                        Console.WriteLine("Colisión con estela");
                        game.FinalizarJuego("Colisión con estela");
                    }
                    else if (mapa.EsBot(nuevaPosicion))
                    {
                        Console.WriteLine("Colisión con bot");
                        game.FinalizarJuego("Colisión con bot");
                    }
                    else if (Combustible == 0)
                    {
                        game.FinalizarJuego("Combustible agotado");
                    }
                    else
                    {
                        if (nuevaPosicion.TipoPoder != null)
                        {
                            PoderesStack.Push(nuevaPosicion.TipoPoder);
                            Poderes.ActualizarListaPoderes();
                            mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
                            nuevaPosicion.TipoPoder = null;
                        }

                        Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
                        PosicionActual = nuevaPosicion;
                        Combustible -= 1;
                    }
                }
                else
                {
                    Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
                    PosicionActual = nuevaPosicion;
                    Combustible -= 1;
                }

            }
            else
            {
                game.FinalizarJuego("Ubicación inválida");
            }
        }


        public void SoltarItemsYPoderes()
        {
            if (PoderesStack.Count > 0)
            {
                Random random = new Random();

                foreach (var poder in PoderesStack)
                {
                    int x = random.Next(0, game.columnas);
                    int y = random.Next(0, game.filas);
                    Casilla casilla = game.mapa.ObtenerCasilla(x, y);

                    if (casilla != null && !casilla.EsParteDeEstela && casilla.TipoPoder == null)
                    {
                        game.mapa.ColocarImagenEnCelda(x, y, Properties.Resources.Poderes);
                        casilla.TipoPoder = poder;
                    }
                }
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
