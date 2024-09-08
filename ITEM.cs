using System;
using System.Collections.Generic;

namespace Proyecto2
{
    public class ITEM
    {
        public string Tipo { get; private set; }
        public int Valor { get; private set; }

        public ITEM(string tipo)
        {
            Tipo = tipo;

            Random rand = new Random();
            switch (tipo)
            {
                case "Combustible":
                    Valor = rand.Next(10, 51); // Capacidad aleatoria entre 10 y 50
                    break;
                case "Estela":
                    Valor = rand.Next(1, 11); // Incremento aleatorio entre 1 y 10
                    break;
                case "Bomba":
                    Valor = 0; // Las bombas no necesitan un valor numérico
                    break;
            }
        }

        public void Aplicar(MOTO moto)
        {
            switch (Tipo)
            {
                case "Combustible":
                    AplicarCombustible(moto);
                    break;
                case "Estela":
                    AplicarCrecimientoEstela(moto);
                    break;
                case "Bomba":
                    AplicarBomba(moto);
                    break;
            }
        }

        private void AplicarCombustible(MOTO moto)
        {
            if (moto.Combustible < moto.CombustibleMaximo)
            {
                int incremento = Math.Min(Valor, moto.CombustibleMaximo - moto.Combustible);
                if (incremento < 0)
                {
                    incremento = 0; 
                }
                moto.Combustible += incremento;
                moto.game.ActualizarCombustible();
                Console.WriteLine($"Celda de combustible aplicada: {incremento} unidades");
            }
            else
            {
                Console.WriteLine("Combustible lleno"  );
            }
        }


        private void AplicarCrecimientoEstela(MOTO moto)
        {
            moto.Tamaño_Estela += Valor;
            moto.Estela.IncrementarMaxLongitud(Valor);
        }

        private void AplicarBomba(MOTO moto)
        {
            moto.Explotar();
        }
    }
}
