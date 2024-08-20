using System;
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

        private void ActivarEscudo()
        {
            moto.tieneEscudo = true;
            escudoTimer = new Timer();
            escudoTimer.Interval = 7500; // Escudo dura exactamente 7.5 segundos
            escudoTimer.Tick += Timer_Tick;
            escudoTimer.Start();
        }

        private void ActivarHiperVelocidad()
        {
            velocidadTimer = new Timer();
            moto.Velocidad = velocidadOriginal / 2; // Velocidad 2 veces mayor
            moto.game.ConfigurarTemporizador(); // Actualiza el temporizador de movimiento en la clase GAME
            velocidadTimer.Interval = 7500; // HiperVelocidad dura 7.5 segundos
            velocidadTimer.Tick += Timer_Tick;
            velocidadTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;

            if (timer == escudoTimer)
            {
                moto.tieneEscudo = false;
            }
            else if (timer == velocidadTimer)
            {
                moto.Velocidad = velocidadOriginal;
                moto.game.ConfigurarTemporizador(); // Restaurar el temporizador de movimiento en GAME
            }

            timer.Stop();
            timer.Dispose();
        }
    }
}
