using System;
using System.Windows.Forms;

namespace Proyecto2
{
    public partial class INICIO : Form
    {
        public INICIO()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Abrir la ventana del juego
            GAME gameForm = new GAME(49, 28);
            gameForm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
