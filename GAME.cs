using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


namespace Proyecto2
{
    public partial class GAME : Form
    {
        private MOTO moto;
        private int cellSize = 20;  // Tamaño de cada celda en píxeles
        private Panel[,] gridPanels;  // Matriz de Paneles que representarán el grid
        private int columnas;
        private int filas;
        private Grid grid;  // Declarar la variable grid

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.Size = new Size(1050, 750);  // Ajusta el tamaño de la ventana del juego
            this.columnas = columnas;
            this.filas = filas;
            CrearGrid(columnas, filas);  // Crear el grid en esta ventana
            InicializarMoto();
        }

        private void CrearGrid(int columnas, int filas)
        {
            // Inicializar el grid lógico
            grid = new Grid(filas, columnas);

            // Tamaño total del grid
            int gridWidth = columnas * cellSize;
            int gridHeight = filas * cellSize;

            // Calcular las coordenadas iniciales para centrar el grid
            int startX = (this.ClientSize.Width - gridWidth) / 2;
            int startY = (this.ClientSize.Height - gridHeight) / 2;

            // Inicializar la matriz de Paneles
            gridPanels = new Panel[filas, columnas];

            // Dibujar el grid visualmente
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Panel panel = new Panel();
                    panel.Size = new Size(cellSize, cellSize);

                    // Ajustar la posición inicial del grid para centrarlo
                    panel.Location = new Point(startX + j * cellSize, startY + i * cellSize);

                    panel.BorderStyle = BorderStyle.None;  // Eliminar bordes
                    panel.BackColor = Color.LightGreen;  // Establecer el color de fondo

                    // Establecer la imagen de fondo
                    panel.BackgroundImage = Image.FromFile("C:\\Users\\Pablo\\OneDrive - Estudiantes ITCR\\TEC\\Semestre 2\\00 Datos\\P2\\Proyecto2\\Resources\\celda.jpg");  // Reemplaza con la ruta de tu imagen
                    panel.BackgroundImageLayout = ImageLayout.Stretch;

                    // Guardar el panel en la matriz
                    gridPanels[i, j] = panel;

                    // Agregar el panel al formulario
                    this.Controls.Add(panel);
                }
            }
        }

        private void InicializarMoto()
        {
            // Inicializa la moto en la posición (0, 0)
            Casilla posicionInicial = grid.ObtenerCasilla(0, 0);  // Usar el método ObtenerCasilla del grid
            moto = new MOTO(10, 3, 100, new List<string>(), new List<string>(), posicionInicial);
            ActualizarPosicionMoto();
        }

        private void ActualizarPosicionMoto()
        {
            // Limpia el color anterior
            foreach (var panel in gridPanels)
            {
                panel.BackColor = Color.LightGreen;
            }

            // Colorea la nueva posición de la moto
            int x = moto.PosicionActual.X;
            int y = moto.PosicionActual.Y;
            gridPanels[x, y].BackColor = Color.Red;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Casilla nuevaPosicion = null;

            switch (keyData)
            {
                case Keys.Up:
                    nuevaPosicion = moto.PosicionActual.Arriba;
                    break;
                case Keys.Down:
                    nuevaPosicion = moto.PosicionActual.Abajo;
                    break;
                case Keys.Left:
                    nuevaPosicion = moto.PosicionActual.Izquierda;
                    break;
                case Keys.Right:
                    nuevaPosicion = moto.PosicionActual.Derecha;
                    break;
            }

            if (nuevaPosicion != null)
            {
                moto.Mover(nuevaPosicion);
                ActualizarPosicionMoto();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GAME_Load(object sender, EventArgs e)
        {

        }
    }
}
