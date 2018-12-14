using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubson
{
    class Video
    {

        public int Duracao = 0;
        public int tamanho = 0;
        public DateTime Postado;
        public string EnderVideo="";
        // private string tDuracao = "";

        // Passar o processamento da direção para uma função genérica
        // Colocar o processamento da data de postagem, pela função genérica

        public void setsDuracao(string Sduracao)
        {
            Duracao = RetTempo(Sduracao);
            // Duracao = 1;
        }

        private int RetTempo(string Texto)
        {
            Boolean Sair = false;
            int Dur = 0;
            while (Sair == false)
            {
                if (Texto.IndexOf("hora") > 0)
                {
                    Dur += TrazHoras(ref Texto);
                }
                else
                {
                    int indMinuto = Texto.IndexOf("minuto");
                    if (indMinuto > 0)
                    {
                        Dur += TrazMinutos(indMinuto, ref Texto);
                    }
                    else
                    {
                        if (Texto.IndexOf("segundo") > 0)
                        {
                            Dur += TrazSegundos(Texto);
                            Sair = true;
                        }
                        else
                        {
                            Sair = true;
                        }
                    }
                }
            }
            return Dur;
        }

        public void setPostado(string postado)
        {
            int TmpAtras = RetTempo(postado);
            Postado = DateTime.Now.AddSeconds(-TmpAtras);
        }

        private int TrazHoras(ref string Texto)
        {
            Texto = Texto.Replace("Transmitido ", "");
            int PosE = Texto.IndexOf(" ");
            int Hor = Convert.ToInt16(Texto.Substring(0, PosE));
            Texto = Texto.Substring(PosE + 11);
            return Hor * 60*60;
        }

        private int TrazSegundos(string Texto)
        {
            int PosE = Texto.IndexOf(" ");
            int Seg = Convert.ToInt16(Texto.Substring(0, PosE));
            return Seg;
        }

        private int TrazMinutos(int indMinuto, ref string Texto)
        {
            int PosE = Texto.IndexOf(" ", indMinuto);
            int Min = Convert.ToInt16(Texto.Substring(indMinuto - 2, 2));
            Texto = Texto.Substring(PosE + 3);
            return Min*60;
        }

    }
}
