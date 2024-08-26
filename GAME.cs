using Proyecto2.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Media;
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
        public List<BOTS> bots;
        public ListBox listaPoderes;
        public Label combustibleLabel { get; private set; }
        private WindowsMediaPlayer player = new WindowsMediaPlayer();

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * 20 + 100, filas * 20 + 170);  // tamaño de la ventana del juego

            mapa = new MAPA(filas, columnas, 20, this);  // Crear el grid en esta ventana
            mapa.ColocarPoderesAleatorios(10); //5 poderes aleatorios en el mapa
            mapa.ColocarItemsAleatorios(10);
            InicializarMoto();
            InicializarBots(4);  // Inicializa 4 bots
            ConfigurarTemporizador();
            MostrarCombustible();
            InicializarListaPoderes();
            moto.Poderes.ActualizarListaPoderes();

        }

        private void InicializarMoto()
        {
            Casilla posicionInicial = mapa.ObtenerCasilla(24, 26);

            moto = new MOTO(150, 3, 300, new List<string>(), new List<string>(), posicionInicial, this);

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
        private void InicializarListaPoderes()
        {
            listaPoderes = new ListBox
            {
                Size = new Size(200, 60)  // Tamaño del ListBox
            };

            int x = 40;
            int y = this.ClientSize.Height - listaPoderes.Height - 10;

            listaPoderes.Location = new Point(x, y);

            this.Controls.Add(listaPoderes);
        }
        public void ActualizarCombustible()
        {
            if (moto.Combustible <= 0)
            {
                FinalizarJuego("Te has quedado sin combustible");
            }

            // Asegurarse de que la actualización ocurra en el hilo correcto
            if (combustibleLabel.InvokeRequired)
            {
                combustibleLabel.Invoke(new Action(() =>
                {
                    combustibleLabel.Text = $"Combustible: {moto.Combustible}";
                }));
            }
            else
            {
                combustibleLabel.Text = $"Combustible: {moto.Combustible}";
            }
        }

        private void MostrarCombustible()
        {
            combustibleLabel = new Label
            {
                Text = $"Combustible: {moto.Combustible}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                AutoSize = true,
                Location = new Point(300, this.ClientSize.Height - 50) // Adjust location as needed
            };

            this.Controls.Add(combustibleLabel);
        }

        public void MatarBot(BOTS bot)
        {
            if (bot == null || !bots.Contains(bot))
            {
                return; // Evita eliminar el mismo bot más de una vez
            }

            bot.SoltarItemsYPoderes();
            bot.DetenerBot();

            // Limpiar la imagen del bot en su posición actual
            mapa.ColocarImagenEnCelda(bot.PosicionActual.X, bot.PosicionActual.Y, null);

            bot.PosicionActual.EsBot = false;

            // Limpiar todas las posiciones de la estela del bot
            foreach (var (X, Y) in bot.Estela.ObtenerPosiciones())
            {
                mapa.ColorearCelda(X, Y, Color.MediumPurple);
            }

            PlayMp3File("muerte");

            bots.Remove(bot);

            Console.WriteLine("Un bot ha sido eliminado.");
            if (bots.Count == 0)
            {
                MostrarPantallaVictoria(); // Mostrar pantalla de victoria
            }
        }
        public void PlayMp3File(string filePath)
        {
            try
            {
                SoundPlayer player = new SoundPlayer((System.IO.Stream)Properties.Resources.ResourceManager.GetObject(filePath));
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reproducir el sonido: {ex.Message}");
            }
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Mapear WASD a las teclas de flecha correspondientes
            if (keyData == Keys.W) keyData = Keys.Up;
            if (keyData == Keys.S) keyData = Keys.Down;
            if (keyData == Keys.A) keyData = Keys.Left;
            if (keyData == Keys.D) keyData = Keys.Right;

            // Mover poderes en la pila usando 1 y 2
            if (keyData == Keys.D1)
            {
                moto.Poderes.MoverPoderArriba();
                return true;
            }
            else if (keyData == Keys.D2)
            {
                moto.Poderes.MoverPoderAbajo();
                return true;
            }
            // Aplicar poder usando Enter
            else if (keyData == Keys.Enter)
            {
                moto.Poderes.AplicarPoder();
                return true;
            }

            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                direccionActual = keyData;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FinalizarJuego(string razonMuerte)
        {
            PlayMp3File("Game_over");
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

        private void MostrarPantallaVictoria()
        {
            PlayMp3File("win");
            movimientoTimer.Stop();

            this.Close(); // Cerrar el juego actual

            Form pantallaVictoria = new Form
            {
                Text = "¡VICTORIA!",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            Label label = new Label
            {
                Text = "¡Felicidades! Has eliminado a todos los bots.",
                Font = new Font("Arial", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            pantallaVictoria.Controls.Add(label);
            pantallaVictoria.ShowDialog();

            pantallaVictoria.Dispose(); // Libera los recursos utilizados por la pantalla de victoria
        }

        private void GAME_Load(object sender, EventArgs e)
        {
        }
    }
}