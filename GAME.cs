using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using System;
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
        public FlowLayoutPanel panelPoderes;
        private ProgressBar progressBarCombustible;
        private Panel panelTotal;
        private PictureBox pictureBoxCombustible;
        private Panel panelCombustible;


        private WindowsMediaPlayer player = new WindowsMediaPlayer();

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * 20 + 100, filas * 20 + 170);  // tamaño de la ventana del juego
            Image backgroundImage = Properties.Resources.futuristic_background;
            this.BackgroundImage = backgroundImage;
            this.BackgroundImageLayout = ImageLayout.Stretch;  // Ajusta la imagen para que cubra todo el fondo

            mapa = new MAPA(filas, columnas, 20, this);  // Crear el grid en esta ventana
            mapa.ColocarPoderesAleatorios(10);
            mapa.ColocarItemsAleatorios(10);
            InicializarMoto();
            InicializarBots(4);
            ConfigurarTemporizador();
            MostrarCombustible();
            InicializarPanelPoderes();
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

                BOTS bot = new BOTS(300, 3, 100, new List<string>(), new List<string>(), posicionInicial, this);
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

        private void InicializarPanelPoderes()
        {
            panelPoderes = new FlowLayoutPanel
            {
                Size = new Size(300, 60),  // Tamaño del panel
                AutoScroll = true,         // Activar scroll si hay muchos poderes
                FlowDirection = FlowDirection.LeftToRight
            };

            int x = 40;
            int y = this.ClientSize.Height - 70;

            panelPoderes.Location = new Point(x, y);
            this.Controls.Add(panelPoderes);
        }

        public void ActualizarCombustible()
        {
            if (moto.Combustible <= 0)
            {
                FinalizarJuego("Te has quedado sin combustible");
                return;
            }

            // Verificar si el método se ejecuta en un hilo diferente y usar Invoke si es necesario
            if (panelCombustible.InvokeRequired)
            {
                panelCombustible.Invoke(new Action(() =>
                {
                    ActualizarBarraCombustible();
                }));
            }
            else
            {
                ActualizarBarraCombustible();
            }
        }

        private void ActualizarBarraCombustible()
        {
            // Calcular el porcentaje de combustible restante
            double porcentajeCombustible = (double)moto.Combustible / moto.CombustibleMaximo * 100;

            // Ajustar el tamaño del Panel según el porcentaje
            panelCombustible.Width = (int)(200 * (porcentajeCombustible / 100));

            if (porcentajeCombustible <= 25)
            {
                panelCombustible.BackColor = Color.Red;
            }
            else if (porcentajeCombustible <= 50)
            {
                panelCombustible.BackColor = Color.Yellow;
            }
            else
            {
                panelCombustible.BackColor = Color.Green;
            }
        }



        public void MostrarCombustible()
        {
            pictureBoxCombustible = new PictureBox
            {
                Image = Properties.Resources.Combustible,
                Size = new Size(50, 50),
                Location = new Point(795, this.ClientSize.Height - 70),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            panelCombustible = new Panel
            {
                Size = new Size(200, 30),
                Location = new Point(800, this.ClientSize.Height - 60),
                BackColor = Color.Green // Comienza en verde
            };
            panelTotal = new Panel
            {
                Size = new Size(200, 30),
                Location = new Point(800, this.ClientSize.Height - 60),
                BackColor = Color.Gray
            };
            this.Controls.Add(pictureBoxCombustible);
            this.Controls.Add(panelCombustible);
            this.Controls.Add(panelTotal);
        }


        public void MatarBot(BOTS bot)
        {
            if (bot == null || !bots.Contains(bot))
            {
                return; // Evita eliminar el mismo bot más de una vez
            }

            bot.SoltarItemsYPoderes(); // Suelta los ítems y poderes del bot
            bot.DetenerBot(); // Detiene el movimiento del bot

            bot.Estela.LimpiarEstela(mapa);

            var cabeza = bot.PosicionActual;
            mapa.ColocarImagenEnCelda(cabeza.X, cabeza.Y, null);
            cabeza.EsBot = false;

            PlayMp3File("muerte");
            bots.Remove(bot);

            Console.WriteLine("Un bot ha sido eliminado.");

            if (bots.Count == 0)
            {
                MostrarPantallaVictoria();
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
            if (keyData == Keys.W) keyData = Keys.Up;
            if (keyData == Keys.S) keyData = Keys.Down;
            if (keyData == Keys.A) keyData = Keys.Left;
            if (keyData == Keys.D) keyData = Keys.Right;

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

            pantallaFin.Dispose();
        }

        private void MostrarPantallaVictoria()
        {
            PlayMp3File("win");
            movimientoTimer.Stop();

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.Close();

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
                    pantallaVictoria.Dispose();
                }));
            }
            else
            {
                this.Close();

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
                pantallaVictoria.Dispose();
            }
        }
        private void GAME_Load(object sender, EventArgs e)
        {
        }
    }
}
