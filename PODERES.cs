using System;
using System.Collections.Generic;
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
            moto.tieneEscudo = true;

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
                escudoTimer.Stop();
                escudoTimer.Dispose();
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

            moto.Velocidad = velocidadOriginal / 4; // Aumentar la velocidad (dividir el intervalo para hacer que el bot se mueva más rápido)
            moto.game.ConfigurarTemporizador(); // Reconfigura el temporizador para reflejar la nueva velocidad

            velocidadTimer = new Timer();
            velocidadTimer.Interval = 7500; // La hiper velocidad dura 7.5 segundos
            velocidadTimer.Tick += (sender, e) =>
            {
                moto.Velocidad = velocidadOriginal; // Restaurar la velocidad original después de que termine la hiper velocidad
                moto.game.ConfigurarTemporizador(); // Reconfigura el temporizador para volver a la velocidad normal
                velocidadTimer.Stop();
                velocidadTimer.Dispose();
            };
            velocidadTimer.Start();
        }

        public void ActualizarListaPoderes()
        {
            moto.game.listaPoderes.Items.Clear();
            foreach (var poder in moto.PoderesStack)
            {
                moto.game.listaPoderes.Items.Add(poder);
            }
        }

        public void MoverPoderArriba()
        {
            if (moto.PoderesStack.Count > 1)
            {
                var poder = moto.PoderesStack.Pop();
                moto.PoderesStack = new Stack<string>(new[] { poder }.Concat(moto.PoderesStack));
                ActualizarListaPoderes();
            }
        }

        public void MoverPoderAbajo()
        {
            if (moto.PoderesStack.Count > 1)
            {
                var poderes = moto.PoderesStack.ToArray();
                moto.PoderesStack.Clear();
                moto.PoderesStack.Push(poderes[poderes.Length - 1]);
                for (int i = 0; i < poderes.Length - 1; i++)
                {
                    moto.PoderesStack.Push(poderes[i]);
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
