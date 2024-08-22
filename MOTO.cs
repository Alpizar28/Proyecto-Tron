using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers; // Usando específicamente System.Timers
using System.Windows.Forms;

namespace Proyecto2
{
    public class MOTO
    {
        public GAME game;
        public int Velocidad { get; set; }
        public int Tamaño_Estela { get; set; }
        public int Combustible { get; set; }
        public int CombustibleMaximo { get; private set; } = 100;
        public Queue<ITEM> ColaItems { get; private set; }

        private System.Timers.Timer itemTimer; // Especifica System.Timers.Timer
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
            ColaItems = new Queue<ITEM>(); // Inicializar la cola de ítems
            Estela.AgregarNodo(posicionInicial.X, posicionInicial.Y, null);
            this.game = game;
            Poderes = new PODERES(this);

            itemTimer = new System.Timers.Timer(1000); // 1 segundo de delay entre aplicaciones
            itemTimer.Elapsed += AplicarSiguienteItem;
            itemTimer.Start();
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

            if (!EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                game.FinalizarJuego("Ubicación inválida");
                return;
            }

            if (Combustible == 0)
            {
                game.FinalizarJuego("Te quedaste sin gasolina");
                return;
            }

            if (mapa.EsEstela(nuevaPosicion) || mapa.EsBot(nuevaPosicion))
            {
                ManejarColision();
                return;
            }

            if (nuevaPosicion.TipoPoder != null)
            {
                ManejarPoderEncontrado(nuevaPosicion, mapa);
            }

            if (nuevaPosicion.TipoItem != null)
            {
                ManejarItemsEncontrados(nuevaPosicion, mapa);
            }

            MovimientoEfectivo(mapa, nuevaPosicion);
        }

        private void ManejarItemsEncontrados(Casilla nuevaPosicion, MAPA mapa)
        {
            ITEM item = new ITEM(nuevaPosicion.TipoItem); // Crear el ítem basado en el tipo encontrado
            ColaItems.Enqueue(item); // Agregar el ítem a la cola de ítems

            mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
            nuevaPosicion.TipoItem = null;
        }

        private void ManejarColision()
        {
            if (!tieneEscudo)
            {
                game.FinalizarJuego("Has muerto");
            }
        }

        public void ManejarPoderEncontrado(Casilla nuevaPosicion, MAPA mapa)
        {
            PoderesStack.Push(nuevaPosicion.TipoPoder);
            Poderes.ActualizarListaPoderes();
            mapa.ColocarImagenEnCelda(nuevaPosicion.X, nuevaPosicion.Y, null);
            nuevaPosicion.TipoPoder = null;
        }

        public void MovimientoEfectivo(MAPA mapa, Casilla nuevaPosicion)
        {
            Estela.AgregarNodo(PosicionActual.X, PosicionActual.Y, mapa);
            PosicionActual = nuevaPosicion;
            Combustible -= 1;
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

        public void AgregarItem(ITEM item)
        {
            ColaItems.Enqueue(item);
        }

        private void AplicarSiguienteItem(object sender, ElapsedEventArgs e)
        {
            if (ColaItems.Count > 0)
            {
                ITEM siguienteItem = ColaItems.Dequeue();
                siguienteItem.Aplicar(this); // Aplicar el ítem

                if (siguienteItem.Tipo == "Combustible" && Combustible == CombustibleMaximo)
                {
                    ColaItems.Enqueue(siguienteItem); // Reinsertar en la cola si el combustible está lleno
                    Console.WriteLine("Combustible lleno, celda de combustible reinserta en la cola.");
                }
            }
        }

        public void Explotar()
        {
            Console.WriteLine("¡La moto ha explotado!");
        }
    }
}
