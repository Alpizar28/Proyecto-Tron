using Proyecto2.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Proyecto2
{
    public partial class GAME : Form
    {
        public MOTO moto { get; protected set; }
        public MAPA mapa { get; private set; }
        public int columnas { get; private set; }
        public int filas { get; private set; }
        public Timer movimientoTimer;
        private Keys direccionActual = Keys.Up;
        private List<BOTS> bots;

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * 20 + 100, filas * 20 + 100);  // Ajusta el tamaño de la ventana del juego

            mapa = new MAPA(filas, columnas, 20, this);  // Crear el grid en esta ventana
            mapa.ColocarPoderesAleatorios(5); // Colocar 5 poderes aleatorios en el mapa
            InicializarMoto();
            InicializarBots(4);  // Inicializa 4 bots
            ConfigurarTemporizador();
        }

        private void InicializarMoto()
        {
            Casilla posicionInicial = mapa.ObtenerCasilla(24, 26);

            moto = new MOTO(150, 13, 300, new List<string>(), new List<string>(), posicionInicial, this);

            moto.ConfigurarImagenes(
                Properties.Resources.MotoDerecha,
                Properties.Resources.MotoIzquierda,
                Properties.Resources.MotoArriba,
                Properties.Resources.MotoAbajo
            );

            moto.ActualizarImagen(mapa, direccionActual);
        }

        public void ConfigurarTemporizador()
        {
            if (movimientoTimer != null)
            {
                movimientoTimer.Stop();
                movimientoTimer.Dispose();
            }

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
        }

        private void InicializarBots(int cantidadBots)
        {
            bots = new List<BOTS>();
            Random random = new Random();

            for (int i = 0; i < cantidadBots; i++)
            {
                int x = random.Next(0, columnas);
                int y = random.Next(0, filas);
                Casilla posicionInicial = mapa.ObtenerCasilla(x, y);

                BOTS bot = new BOTS(400, 3, 100, new List<string>(), new List<string>(), posicionInicial, this);
                bot.ConfigurarImagenes(
                    Properties.Resources.BotDerecha,
                    Properties.Resources.BotIzquierda,
                    Properties.Resources.BotArriba,
                    Properties.Resources.BotAbajo
                );

                bots.Add(bot);
                Console.WriteLine($"Bot {i + 1} inicializado en posición ({x}, {y})");
            }
        }

        public void MatarBot(BOTS bot)
        {
            if (bot == null || !bots.Contains(bot))
            {
                return; // Evita eliminar el mismo bot más de una vez
            }

            bot.DetenerBot();

            // Limpiar la imagen del bot en su posición actual
            mapa.ColocarImagenEnCelda(bot.PosicionActual.X, bot.PosicionActual.Y, null);

            bot.PosicionActual.EsBot = false;

            // Limpiar todas las posiciones de la estela del bot
            foreach (var (X, Y) in bot.Estela.ObtenerPosiciones())
            {
                mapa.ColorearCelda(X, Y, Color.MediumPurple);
            }

            PlayMp3File("C:\\Users\\Pablo\\OneDrive - Estudiantes ITCR\\TEC\\Semestre 2\\00 Datos\\P2\\Proyecto2\\Resources\\muerte.mp3", 20);

            bots.Remove(bot);

            Console.WriteLine("Un bot ha sido eliminado.");
        }

        public void PlayMp3File(string filePath, int volumen)
        {
            WindowsMediaPlayer player = new WindowsMediaPlayer();
            player.URL = filePath;
            player.settings.volume = volumen;
            player.controls.play();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Mapear WASD a las teclas de flecha correspondientes
            if (keyData == Keys.W) keyData = Keys.Up;
            if (keyData == Keys.S) keyData = Keys.Down;
            if (keyData == Keys.A) keyData = Keys.Left;
            if (keyData == Keys.D) keyData = Keys.Right;

            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                direccionActual = keyData;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FinalizarJuego(string razonMuerte)
        {
            movimientoTimer.Stop();

            // Detener todos los bots
            foreach (var bot in bots)
            {
                bot.DetenerBot();
            }

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
