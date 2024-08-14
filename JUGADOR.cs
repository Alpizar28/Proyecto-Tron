using System;
using System.Collections.Generic;

namespace Proyecto2
{
    public class MOTO
    {
        public int Velocidad { get; set; }
        public int Tamaño_Estela { get; set; }
        public int Combustible { get; set; }
        public List<string> Items { get; set; }
        public List<string> Poderes { get; set; }
        public Casilla PosicionActual { get; private set; }

        public MOTO(int velocidad, int tamaño_estela, int combustible, List<string> items, List<string> poderes, Casilla posicionInicial)
        {
            Velocidad = velocidad;
            Tamaño_Estela = tamaño_estela;
            Combustible = combustible;
            Items = items;
            Poderes = poderes;
            PosicionActual = posicionInicial;
        }

        public void Mover(Casilla nuevaPosicion)
        {
            if (Combustible > 0)
            {
                PosicionActual = nuevaPosicion;
                Combustible -= 1;
            }
        }
    }
}