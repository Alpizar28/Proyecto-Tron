using System;
using System.Windows.Forms;

namespace Proyecto2
{
    public class PODERESBOT
    {
        private BOTS bot;
        private Timer escudoTimer;
        private Timer velocidadTimer;
        private int velocidadOriginal;

        public PODERESBOT(BOTS bot)
        {
            this.bot = bot;
            velocidadOriginal = bot.Velocidad;
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
            Console.WriteLine("Escudo activado para el bot");
            bot.tieneEscudo = true;

            if (escudoTimer != null)
            {
                escudoTimer.Stop();
                escudoTimer.Dispose();
            }

            escudoTimer = new Timer
            {
                Interval = 7500
            };
            escudoTimer.Tick += (sender, e) =>
            {
                bot.tieneEscudo = false;
                escudoTimer.Stop();
                escudoTimer.Dispose();
                Console.WriteLine("Escudo desactivado para el bot");
                bot.BotPoderesStack.Pop();
            };
            escudoTimer.Start();
        }

        private void ActivarHiperVelocidad()
        {
            if (velocidadTimer != null)
            {
                velocidadTimer.Stop();
                velocidadTimer.Dispose();
            }

            int newInterval = velocidadOriginal / 4;
            bot.Velocidad = newInterval > 0 ? newInterval : 1;

            Console.WriteLine($"Nueva velocidad del bot (intervalo): {bot.Velocidad}");

            bot.ConfigurarTemporizador();

            velocidadTimer = new Timer
            {
                Interval = 7500
            };
            velocidadTimer.Tick += (sender, e) =>
            {
                bot.Velocidad = velocidadOriginal; 
                Console.WriteLine("Velocidad restaurada al valor original.");
                bot.ConfigurarTemporizador(); // Reconfigura el timer con la velocidad original
                velocidadTimer.Stop();
                velocidadTimer.Dispose();
                bot.BotPoderesStack.Pop();
            };
            velocidadTimer.Start();
        }
    }
}
