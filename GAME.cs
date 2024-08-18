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
        private List<BOTS> bots;

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * 20 + 100, filas * 20 + 100);  // Ajusta el tamaño de la ventana del juego

            mapa = new MAPA(filas, columnas, 20, this);  // Crear el grid en esta ventana
            InicializarMoto();
            InicializarBots(4);  // Inicializa 4 bots
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

            moto.ActualizarImagen(mapa, direccionActual);
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
            moto.Mover(direccionActual, mapa, columnas, filas);
            moto.ActualizarEstela(mapa);
            moto.ActualizarImagen(mapa, direccionActual);
            MoverBots(); // Mueve los bots después de mover la moto del jugador
        }

        private void InicializarBots(int cantidadBots)
        {
            bots = new List<BOTS>();

            for (int i = 0; i < cantidadBots; i++)
            {
                int x = new Random().Next(0, columnas);
                int y = new Random().Next(0, filas);
                Casilla posicionInicial = mapa.ObtenerCasilla(x, y);

                BOTS bot = new BOTS(150, 3, 100, new List<string>(), new List<string>(), posicionInicial);
                bot.ConfigurarImagenes(
                    Properties.Resources.MotoDerecha,
                    Properties.Resources.MotoIzquierda,
                    Properties.Resources.MotoArriba,
                    Properties.Resources.MotoAbajo
                );

                bots.Add(bot);
                Console.WriteLine($"Bot {i + 1} inicializado en posición ({x}, {y})");
            }
        }

        private void MoverBots()
        {
            foreach (var bot in bots)
            {
                bot.MoverBot(mapa, columnas, filas);
                bot.ActualizarEstela(mapa);
                bot.ActualizarImagen(mapa, bot.ObtenerDireccionAleatoria());
            }
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
