using Proyecto2__Tron;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2__Tron
{
    public partial class Form1 : Form
    {
        private int cellSize = 10;  // Tamaño de cada celda en píxeles
        private Panel[,] gridPanels;  // Matriz de Paneles que representarán el grid

        public Form1()
        {
            InitializeComponent();
        }

        //1380, 620
        private void button1_Click(object sender, EventArgs e)
        {
            CrearGrid(60, 135);
        }

        private void CrearGrid(int filas, int columnas)
        {
            Grid grid = new Grid(filas, columnas);

            // Inicializar la matriz de Paneles
            gridPanels = new Panel[filas, columnas];

            // Limpiar cualquier panel anterior
            this.Controls.Clear();

            // Dibujar el grid visualmente
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Panel panel = new Panel();
                    panel.Size = new Size(cellSize, cellSize);
                    panel.Location = new Point(j * cellSize, i * cellSize);
                    panel.BorderStyle = BorderStyle.FixedSingle;

                    // Guardar el panel en la matriz
                    gridPanels[i, j] = panel;

                    // Agregar el panel al formulario
                    this.Controls.Add(panel);
                }
            }

            MessageBox.Show($"Grid de {filas}x{columnas} creado visualmente");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
