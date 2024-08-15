﻿using System;
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
            // Limpia el color anterior
            foreach (var panel in gridPanels)
            {
                panel.BackColor = Color.MediumPurple;
            }

            // Colorea la nueva posición de la moto
            int x = moto.PosicionActual.X;
            int y = moto.PosicionActual.Y;

            // Verifica que las coordenadas estén dentro de los límites del grid
            if (x >= 0 && x<= 1050 && y >= 0 && y<= 1050)
            {
                gridPanels[x, y].BackColor = Color.Red;
            }
            else
            {
                MessageBox.Show($"Error: La posición de la moto ({x}, {y}) está fuera de los límites del grid.");
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
