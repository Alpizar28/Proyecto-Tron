using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Proyecto2
{
    public class PODERES
    {
        private MOTO moto;
        private Timer escudoTimer;
        private Timer velocidadTimer;
        private int velocidadOriginal;

        public PODERES(MOTO moto)
        {
            this.moto = moto;
            velocidadOriginal = moto.Velocidad;
        }

        public void ActivarPoder(string poder)
        {
            if (poder == "Escudo")
            {
                ActivarEscudo();
            }
            else if (poder == "HiperVelocidad")
            {
                ActivarHiperVelocidad();
            }
        }

        public void ActivarEscudo()
        {
            Console.WriteLine("Escudo activado para el jugador");
            moto.tieneEscudo = true;
            moto.ActualizarImagen(moto.game.mapa, moto.game.direccionActual); // Actualizar la imagen de la moto con el escudo

            if (escudoTimer != null)
            {
                escudoTimer.Stop();
                escudoTimer.Dispose();
            }

            escudoTimer = new Timer();
            escudoTimer.Interval = 7500; // Escudo dura exactamente 7.5 segundos
            escudoTimer.Tick += (sender, e) =>
            {
                moto.tieneEscudo = false;
                moto.ActualizarImagen(moto.game.mapa, moto.game.direccionActual); // Volver a la imagen original
                escudoTimer.Stop();
                escudoTimer.Dispose();
                Console.WriteLine("Escudo desactivado para el jugador");
            };
            escudoTimer.Start();
        }


        public void ActivarHiperVelocidad()
        {
            if (velocidadTimer != null)
            {
                velocidadTimer.Stop();
                velocidadTimer.Dispose();
            }

            moto.Velocidad = velocidadOriginal / 4;
            moto.enHiperVelocidad = true; 
            moto.ActualizarImagen(moto.game.mapa, moto.game.direccionActual); 
            moto.game.ConfigurarTemporizador();

            velocidadTimer = new Timer();
            velocidadTimer.Interval = 7500; // HiperVelocidad dura  7.5 segundos
            velocidadTimer.Tick += (sender, e) =>
            {
                moto.Velocidad = velocidadOriginal;
                moto.enHiperVelocidad = false;
                moto.ActualizarImagen(moto.game.mapa, moto.game.direccionActual); // Volver a la imagen original
                moto.game.ConfigurarTemporizador();
                velocidadTimer.Stop();
                velocidadTimer.Dispose();
            };
            velocidadTimer.Start();
        }

        public void ActualizarListaPoderes()
        {
            moto.game.panelPoderes.Controls.Clear();  // Limpiar el panel visual de poderes

            foreach (var poder in moto.PoderesStack)
            {
                PictureBox pictureBoxPoder = new PictureBox
                {
                    Size = new Size(50, 50), 
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = ObtenerImagenPoder(poder),  
                    BorderStyle = BorderStyle.FixedSingle
                };

                moto.game.panelPoderes.Controls.Add(pictureBoxPoder);
            }
        }

        private Image ObtenerImagenPoder(string poder)
        {
            if (poder == "Escudo")
            {
                return Properties.Resources.EscudoMuerto;
            }
            else if (poder == "HiperVelocidad")
            {
                return Properties.Resources.HiperVelocidad; 
            }
            return null;  
        }

        public void MoverPoder(string direccion)
        {
            if (moto.PoderesStack.Count > 1)
            {
                var poderes = moto.PoderesStack.ToArray();
                moto.PoderesStack.Clear();

                if (direccion == "Arriba")
                {
                    moto.PoderesStack.Push(poderes[0]);
                    for (int i = 1; i < poderes.Length; i++)
                    {
                        moto.PoderesStack.Push(poderes[i]);
                    }
                }
                else if (direccion == "Abajo")
                {
                    moto.PoderesStack.Push(poderes[poderes.Length - 1]);
                    for (int i = 0; i < poderes.Length - 1; i++)
                    {
                        moto.PoderesStack.Push(poderes[i]);
                    }
                }

                ActualizarListaPoderes();
            }
        }


        public void AplicarPoder()
        {
            if (moto.PoderesStack.Count > 0)
            {
                var poder = moto.PoderesStack.Pop();
                ActivarPoder(poder);
                ActualizarListaPoderes();
            }
        }
    }
}
