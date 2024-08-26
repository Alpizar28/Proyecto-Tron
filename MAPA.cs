using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public class MAPA
    {
        private readonly int filas;
        private readonly int columnas;
        private readonly int cellSize;
        private Panel[,] gridPanels;  // Matriz de Paneles que representarán el grid
        public Grid Grid { get; private set; } // Mapa lógico de casillas

        public MAPA(int filas, int columnas, int cellSize, Control parentControl)
        {
            this.filas = filas;
            this.columnas = columnas;
            this.cellSize = cellSize;
            CrearGrid(parentControl);
        }

        private void CrearGrid(Control parentControl)
        {
            // Inicializar el grid lógico
            Grid = new Grid(filas, columnas);

            // Calcular las coordenadas iniciales para centrar el grid
            int startX = (parentControl.ClientSize.Width - columnas * cellSize) / 2;
            int startY = 55;  // Aplica el margen superior

            // Inicializar la matriz de Paneles
            gridPanels = new Panel[filas, columnas];

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    gridPanels[i, j] = CrearPanel(startX, startY, i, j);
                    parentControl.Controls.Add(gridPanels[i, j]);
                }
            }
        }

        private Panel CrearPanel(int startX, int startY, int fila, int columna)
        {
            Panel panel = new Panel
            {
                Size = new Size(cellSize, cellSize),
                Location = new Point(startX + columna * cellSize, startY + fila * cellSize),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.MediumPurple,
                BackgroundImageLayout = ImageLayout.Stretch  // Asegura que la imagen se ajuste al tamaño del panel
            };
            return panel;
        }

        public void ColorearCelda(int x, int y, Color color)
        {
            if (x >= 0 && x < columnas && y >= 0 && y < filas)
            {
                Panel panel = gridPanels[y, x];
                panel.BackColor = color;
                panel.BackgroundImage = null;  // Restablecer la imagen para que se vea solo el color

                // Marcar la casilla como parte de una estela si es de un color específico
                Casilla casilla = Grid.ObtenerCasilla(x, y);
                if (color == Color.SkyBlue) // Colores de estela
                {
                    casilla.EsParteDeEstela = true;
                }
                else if (color == Color.MediumPurple)
                {
                    casilla.EsParteDeEstela = false;
                }
            }
        }  

        public void ColocarImagenEnCelda(int x, int y, Image imagen)
        {
            if (x >= 0 && x < columnas && y >= 0 && y < filas)
            {
                Panel panel = gridPanels[y, x];
                panel.BackgroundImage = imagen;
                panel.Invalidate();  // Forzar la actualización del panel
            }
        }
        public void ColocarPoderesAleatorios(int cantidadPoderes)
        {
            Random random = new Random();
            for (int i = 0; i < cantidadPoderes; i++)
            {
                int x = random.Next(0, columnas);
                int y = random.Next(0, filas);

                Image imagenPoder;
                string tipoPoder = (random.Next(0, 2) == 0) ? "Escudo" : "HiperVelocidad";

                if (tipoPoder == "Escudo")
                {
                    imagenPoder = Properties.Resources.Escudo;
                }
                else
                {
                    imagenPoder = Properties.Resources.HiperVelocidad;
                }

                ColocarImagenEnCelda(x, y, imagenPoder);

                Grid.ObtenerCasilla(x, y).TipoPoder = tipoPoder;
            }
        }

        public void ColocarItemsAleatorios(int cantidadItems)
        {
            Random random = new Random();
            for (int i = 0; i < cantidadItems; i++)
            {
                int x = random.Next(0, columnas);
                int y = random.Next(0, filas);

                Image imagenItem;
                int randomValue = random.Next(0, 3);
                string tipoItem = randomValue == 0 ? "Combustible" : randomValue == 1 ? "Estela" : "Bomba";

                if (tipoItem == "Combustible")
                {
                    imagenItem = Properties.Resources.Combustible;
                }
                else if (tipoItem == "Estela")
                {
                    imagenItem = Properties.Resources.Poderes;
                }
                else
                {
                    imagenItem = Properties.Resources.Bomba;
                }


                ColocarImagenEnCelda(x, y, imagenItem);

                Grid.ObtenerCasilla(x, y).TipoItem = tipoItem;
            }
        }


        public Casilla ObtenerCasilla(int x, int y)
        {
            return Grid.ObtenerCasilla(x, y);
        }

        public bool EsEstela(Casilla casilla)
        {
            return casilla.EsParteDeEstela;
        }
        public bool EsPoder(Casilla casilla)
        {
            return casilla.TipoPoder != null;
        }
        public bool EsBot(Casilla casilla)
        {
            return casilla.EsBot;
        }
    }

    public class Casilla
    {
        public Casilla Arriba { get; set; }
        public Casilla Abajo { get; set; }
        public Casilla Izquierda { get; set; }
        public Casilla Derecha { get; set; }

        public int X { get; set; } // Posición X (columna) en el grid
        public int Y { get; set; } // Posición Y (fila) en el grid

        public bool EsParteDeEstela { get; set; } = false;
        public string TipoPoder { get; set; }
        public string TipoItem { get; set; }
        public bool EsBot { get; set; } = false;
        public Casilla(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Grid
    {
        public Casilla PrimerCasilla { get; private set; }

        public Grid(int filas, int columnas)
        {
            if (filas <= 0 || columnas <= 0) throw new ArgumentException("Las filas y columnas deben ser mayores que cero.");

            Casilla[,] grid = new Casilla[filas, columnas];

            // Crear el grid con las posiciones correspondientes
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    grid[i, j] = new Casilla(j, i); // X es columna, Y es fila
                }
            }

            // Establecer las referencias de las casillas
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Casilla actual = grid[i, j];

                    // Establecer la referencia a la casilla de arriba
                    if (i > 0)
                    {
                        actual.Arriba = grid[i - 1, j];
                    }

                    // Establecer la referencia a la casilla de abajo
                    if (i < filas - 1)
                    {
                        actual.Abajo = grid[i + 1, j];
                    }

                    // Establecer la referencia a la casilla de la izquierda
                    if (j > 0)
                    {
                        actual.Izquierda = grid[i, j - 1];
                    }

                    // Establecer la referencia a la casilla de la derecha
                    if (j < columnas - 1)
                    {
                        actual.Derecha = grid[i, j + 1];
                    }
                }
            }

            // Establecer la primera casilla como la posición inicial
            PrimerCasilla = grid[0, 0];
        }

        public Casilla ObtenerCasilla(int x, int y)
        {
            if (x < 0 || y < 0) return null;

            Casilla actual = PrimerCasilla;

            // Mover hacia abajo hasta la fila correcta (Y)
            while (actual != null && actual.Y != y)
            {
                actual = actual.Abajo;
            }

            // Ahora mover hacia la derecha hasta la columna correcta (X)
            while (actual != null && actual.X != x)
            {
                actual = actual.Derecha;
            }

            return actual;
        }

    }
}
