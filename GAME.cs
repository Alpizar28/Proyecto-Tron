using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Proyecto2
{
    public partial class GAME : Form
    {
        private MOTO moto;
        private readonly int cellSize = 20;  // Tamaño de cada celda en píxeles
        private Panel[,] gridPanels;  // Matriz de Paneles que representarán el grid
        private readonly int columnas;
        private readonly int filas;
        private Grid grid;  // Declarar la variable grid
        private Timer movimientoTimer; // Temporizador para el movimiento continuo
        private Keys direccionActual = Keys.Right;  // Comienza moviéndose hacia la derecha

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.columnas = columnas;
            this.filas = filas;
            this.Size = new Size(columnas * cellSize + 100, filas * cellSize + 100);  // Ajusta el tamaño de la ventana del juego
            CrearGrid();  // Crear el grid en esta ventana
            InicializarMoto();
            ConfigurarTemporizador();
        }

        private void CrearGrid()
        {
            // Inicializar el grid lógico
            grid = new Grid(filas, columnas);

            // Calcular las coordenadas iniciales para centrar el grid
            int startX = (this.ClientSize.Width - columnas * cellSize) / 2;
            int startY = 55;  // Aplica el margen superior

            // Inicializar la matriz de Paneles
            gridPanels = new Panel[filas, columnas];

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    gridPanels[i, j] = CrearPanel(startX, startY, i, j);
                    this.Controls.Add(gridPanels[i, j]);
                }
            }

            // Forzar la actualización de la interfaz
            this.Invalidate();
        }

        private Panel CrearPanel(int startX, int startY, int fila, int columna)
        {
            Panel panel = new Panel
            {
                Size = new Size(cellSize, cellSize),
                Location = new Point(startX + columna * cellSize, startY + fila * cellSize),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.MediumPurple
            };
            return panel;
        }

        private void InicializarMoto()
        {
            Casilla posicionInicial = grid.ObtenerCasilla(0, 0);
            moto = new MOTO(10, 3, 100, new List<string>(), new List<string>(), posicionInicial);
            ActualizarPosicionMoto();
        }

        private void ConfigurarTemporizador()
        {
            movimientoTimer = new Timer
            {
                Interval = 150 // Intervalo en milisegundos
            };
            movimientoTimer.Tick += MoverMoto;
            movimientoTimer.Start();
        }

        private void MoverMoto(object sender, EventArgs e)
        {
            Casilla nuevaPosicion = ObtenerNuevaPosicion();

            if (EsPosicionValida(nuevaPosicion))
            {
                moto.Mover(nuevaPosicion);
                ActualizarPosicionMoto();
            }
            else
            {
                FinalizarJuego();
            }
        }

        private Casilla ObtenerNuevaPosicion()
        {
            switch (direccionActual)
            {
                case Keys.Up:
                    return moto.PosicionActual.Arriba;
                case Keys.Down:
                    return moto.PosicionActual.Abajo;
                case Keys.Left:
                    return moto.PosicionActual.Izquierda;
                case Keys.Right:
                    return moto.PosicionActual.Derecha;
                default:
                    return null;
            }
        }

        private bool EsPosicionValida(Casilla posicion)
        {
            return posicion != null &&
                   posicion.X >= 0 && posicion.X < columnas &&
                   posicion.Y >= 0 && posicion.Y < filas;
        }

        private void FinalizarJuego()
        {
            movimientoTimer.Stop();
            this.Close(); // Cierra la ventana del juego actual
            MostrarPantallaFin();
        }

        private void MostrarPantallaFin()
        {
            Form pantallaFin = new Form
            {
                Text = "FIN DEL JUEGO",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            Label label = new Label
            {
                Text = "¡Fin del juego!",
                Font = new Font("Arial", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            pantallaFin.Controls.Add(label);
            pantallaFin.ShowDialog(); // Muestra la pantalla de "FIN" como un cuadro de diálogo modal

            // Libera los recursos del formulario "pantallaFin"
            pantallaFin.Dispose();
        }

        private void ActualizarPosicionMoto()
        {
            if (moto.Estela.Longitud > moto.Estela.MaxLongitud)
            {
                EliminarNodoDeEstela();
            }

            ColorearCelda(moto.PosicionActual.X, moto.PosicionActual.Y, Color.Red);

            // Colorea la estela de la moto
            Nodo nodoEstela = moto.Estela.Cabeza;
            while (nodoEstela != null)
            {
                ColorearCelda(nodoEstela.X, nodoEstela.Y, Color.LightBlue);
                nodoEstela = nodoEstela.Siguiente;
            }
        }

        private void EliminarNodoDeEstela()
        {
            Nodo actual = moto.Estela.Cabeza;
            Nodo previo = null;

            while (actual.Siguiente != null)
            {
                previo = actual;
                actual = actual.Siguiente;
            }

            // Restablecer el color de la última casilla
            ColorearCelda(actual.X, actual.Y, Color.MediumPurple);

            moto.Estela.EliminarUltimoNodo();
        }

        private void ColorearCelda(int x, int y, Color color)
        {
            if (x >= 0 && x < columnas && y >= 0 && y < filas)
            {
                gridPanels[y, x].BackColor = color;
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

        private void MostrarMensajeError(string mensaje)
        {
            MessageBox.Show(mensaje);
        }

        private void GAME_Load(object sender, EventArgs e)
        {
        }
    }
}