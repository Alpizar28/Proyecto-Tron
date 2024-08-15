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
        //Game(49,28)
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

                    panel.BorderStyle = BorderStyle.FixedSingle;  // Eliminar bordes
                    panel.BackColor = Color.MediumPurple;  // Establecer el color de fondo

                    // Guardar el panel en la matriz
                    gridPanels[i, j] = panel;

                    // Agregar el panel al formulario
                    this.Controls.Add(panel);
                }
            }

            // Forzar la actualización de la interfaz
            this.Invalidate();
        }

        private void InicializarMoto()
        {
            // Asegúrate de que la casilla inicial (0, 0) existe en el grid
            Casilla posicionInicial = grid.ObtenerCasilla(0, 0);
            if (posicionInicial != null)
            {
                moto = new MOTO(10, 3, 100, new List<string>(), new List<string>(), posicionInicial);
                ActualizarPosicionMoto();
            }
            else
            {
                MessageBox.Show("Error: No se pudo obtener la posición inicial para la moto.");
            }
        }


        private void ActualizarPosicionMoto()
        {
            // Primero, restablecer el color de la casilla más antigua de la estela si se excede el tamaño
            if (moto.Estela.Longitud > moto.Tamaño_Estela)
            {
                Nodo actual = moto.Estela.Cabeza;
                Nodo previo = null;

                while (actual.Siguiente != null)
                {
                    previo = actual;
                    actual = actual.Siguiente;
                }

                // Restablecer el color de la última casilla
                int xEliminar = actual.X;
                int yEliminar = actual.Y;

                if (xEliminar >= 0 && xEliminar < columnas && yEliminar >= 0 && yEliminar < filas)
                {
                    gridPanels[xEliminar, yEliminar].BackColor = Color.MediumPurple; // Restaurar color original
                }

                // Eliminar el último nodo utilizando el método de la clase Estela
                moto.Estela.EliminarUltimoNodoManualmente();
            }

            // Colorea la nueva posición de la moto
            int x = moto.PosicionActual.X;
            int y = moto.PosicionActual.Y;

            if (x >= 0 && x < columnas && y >= 0 && y < filas)
            {
                gridPanels[x, y].BackColor = Color.Red; // Color de la moto
            }
            else
            {
                MessageBox.Show($"Error: La posición de la moto ({x}, {y}) está fuera de los límites del grid.");
                return;
            }

            // Colorea la estela de la moto
            Nodo nodoEstela = moto.Estela.Cabeza;
            while (nodoEstela != null)
            {
                if (nodoEstela.X >= 0 && nodoEstela.X < columnas && nodoEstela.Y >= 0 && nodoEstela.Y < filas)
                {
                    gridPanels[nodoEstela.X, nodoEstela.Y].BackColor = Color.LightBlue; // Color de la estela
                }
                nodoEstela = nodoEstela.Siguiente;
            }
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

            if (nuevaPosicion != null && nuevaPosicion.X >= 0 && nuevaPosicion.X < filas )
            {
                if (nuevaPosicion.Y >= 0 && nuevaPosicion.Y < columnas)
                { 
                    moto.Mover(nuevaPosicion);
                    ActualizarPosicionMoto();
                }
                else
                {
                    MessageBox.Show("error en las x");
                }
                    
            }
            else
            {
                MessageBox.Show($"Error: La posición de la moto ({nuevaPosicion?.X}, {nuevaPosicion?.Y}) está fuera de los límites del grid.");
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void GAME_Load(object sender, EventArgs e)
        {

        }
    }


}
