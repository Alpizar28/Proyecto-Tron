using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public partial class GAME : Form
    {
        private MOTO moto;
        private MAPA mapa;
        private Timer movimientoTimer;
        private Keys direccionActual = Keys.Up;
        private int columnas;
        private int filas;

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * 20 + 100, filas * 20 + 100);  // Ajusta el tamaño de la ventana del juego

            mapa = new MAPA(filas, columnas, 20, this);  // Crear el grid en esta ventana
            InicializarMoto();
            ConfigurarTemporizador();
        }

        private void InicializarMoto()
        {
            Casilla posicionInicial = mapa.ObtenerCasilla(24, 26);

            moto = new MOTO(150, 3, 100, new List<string>(), new List<string>(), posicionInicial);

            moto.ConfigurarImagenes(
                Properties.Resources.MotoDerecha,
                Properties.Resources.MotoIzquierda,
                Properties.Resources.MotoArriba,
                Properties.Resources.MotoAbajo
            );

            ActualizarPosicionMoto();
        }

        private void ConfigurarTemporizador()
        {
            movimientoTimer = new Timer
            {
                Interval = moto.Velocidad // Intervalo en milisegundos
            };
            movimientoTimer.Tick += MoverMoto;
            movimientoTimer.Start();
        }

        private void MoverMoto(object sender, EventArgs e)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion();

            if (!moto.EsPosicionValida(nuevaPosicion, columnas, filas))
            {
                FinalizarJuego("Se salió de los límites");
            }
            else if (moto.Combustible <= 0)
            {
                FinalizarJuego("Se quedó sin combustible");
            }
            else
            {
                moto.Mover(nuevaPosicion,mapa);
                ActualizarPosicionMoto();
            }
        }
        private Casilla ObtenerNuevaPosicion()
        {
            Casilla nuevaPosicion = direccionActual switch
            {
                Keys.Up => moto.PosicionActual.Arriba,
                Keys.Down => moto.PosicionActual.Abajo,
                Keys.Left => moto.PosicionActual.Izquierda,
                Keys.Right => moto.PosicionActual.Derecha,
                _ => null,
            };

            if (nuevaPosicion == null)
            {
                Console.WriteLine("La nueva posición es null");
            }

            return nuevaPosicion;
        }

        private void ActualizarPosicionMoto()
        {
            moto.ActualizarEstela(mapa);

            Image imagenActual = moto.ObtenerImagenActual(direccionActual);
            mapa.ColocarImagenEnCelda(moto.PosicionActual.X, moto.PosicionActual.Y, imagenActual);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                direccionActual = keyData;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void FinalizarJuego(string razonMuerte)
        {
            movimientoTimer.Stop();
            this.Close();

            Form pantallaFin = new Form
            {
                Text = "FIN DEL JUEGO",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            Label label = new Label
            {
                Text = $"¡Fin del juego! Razón: {razonMuerte}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            pantallaFin.Controls.Add(label);
            pantallaFin.ShowDialog();

            pantallaFin.Dispose(); // Libera los recursos utilizados por la pantalla de fin
        }

        private void GAME_Load(object sender, EventArgs e)
        {
        }
    }
}
