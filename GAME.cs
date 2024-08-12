using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2__Tron
{
    public partial class GAME : Form
    {
        private int cellSize = 20;  // Tamaño de cada celda en píxeles
        private Panel[,] gridPanels;  // Matriz de Paneles que representarán el grid
        private int columnas;
        private int filas;

        public GAME(int columnas, int filas)
        {
            InitializeComponent();
            this.Size = new Size(1050, 750);  // Ajusta el tamaño de la ventana del juego
            CrearGrid(columnas, filas);  // Crear el grid en esta ventana
        }

        private void CrearGrid(int columnas, int filas)
        {
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
                    panel.BackgroundImage = Image.FromFile("C:\\Users\\Pablo\\OneDrive - Estudiantes ITCR\\TEC\\Semestre 2\\00 Datos\\TRON\\TRON\\TRON\\Resources\\celda.jpg");  // Reemplaza con la ruta de tu imagen
                    panel.BackgroundImageLayout = ImageLayout.Stretch;

                    // Guardar el panel en la matriz
                    gridPanels[i, j] = panel;

                    // Agregar el panel al formulario
                    this.Controls.Add(panel);
                }
            }
        }


        private void GAME_Load(object sender, EventArgs e)
        {

        }
    }
}
