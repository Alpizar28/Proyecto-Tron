using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto2
{
    public partial class INICIO : Form
    {
        public INICIO()
        {
            InitializeComponent();
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            this.Size = new Size(1280, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Pantalla de Inicio";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.BackgroundImage = Properties.Resources.fondo2;
            this.BackgroundImageLayout = ImageLayout.Zoom;

            button1.Font = new Font("Arial", 24, FontStyle.Bold);
            button1.BackColor = Color.Green;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Size = new Size(200, 70);
            button1.Location = new Point((this.Width / 2) - 60, (this.Height / 2) - 35);
            button1.Click += button1_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GAME gameForm = new GAME(49, 28);
            gameForm.ShowDialog();  

            this.Close(); 
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
