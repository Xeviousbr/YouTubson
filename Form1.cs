using System;
using System.Data;
using System.Windows.Forms;

/*
 * No segundo canal
 * não ta indo pro vídeo do canal
 * mas direto pra ver o canal enquanto a pagina ainda é a do vídeo
 * 
 * Ver como vai pra página do vídeo, apartir a do canal, no primeiro canal * 
 * 
 */

namespace YouTubson
{
    public partial class Form1 : Form
    {
        private const int colNome = 0;
        private const int colNivel = 1;
        private const int colData = 2;
        private const int colEndereco = 3;

        private DataSet dsResultado = new DataSet();
        private int IndiceCanal = 0;
        private int IndiceVideo = 0;
        private string PagAtual = "";
        private string ConteudoPagCanal = "";
        private DateTime UltVis;
        private int iniCanais=0;

        private DateTime HoraFim;
        private int TmpVideo = 0;
        private int AuxTempo = 0;

        enum tStatus { Nada, Canal, CarregandoVideo, AMostrarOVideo, MostrandoVideo, ProxCanal };
        private tStatus Status = tStatus.Nada;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            /* Nome
            Endereço
            Nivel
            Data de última visualização de video
            Data que foi postado o último vídeo no canal
            Data de checagem */
            //cIni = new Ini();
            //UltData = cIni.getData();

            CarregaLista();
            MostrarCanal();
        }

        private void CarregaLista()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\CanaisYouTube.xml";
            dsResultado.ReadXml(path);
            int NrCanais = dsResultado.Tables[0].Rows.Count;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CadListas Tela = new CadListas();
            Tela.ShowDialog();
        }

        private void MostrarCanal()
        {
            Loga("MostrarCanal");
            string Canal = dsResultado.Tables[0].Rows[IndiceCanal][colEndereco].ToString();
            string sData = dsResultado.Tables[0].Rows[IndiceCanal][colData].ToString();
            if (sData == "")
            {
                UltVis = new DateTime(2000, 1, 1);
            }
            else
            {
                UltVis = Convert.ToDateTime(sData);
            }

            PagAtual = Canal;
            Status = tStatus.Canal;
            webBrowser1.Navigate(Canal);            
            /* PagAtual = Canal + @"/videos";            
            webBrowser1.Navigate(PagAtual); */
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Loga("webBrowser1_DocumentCompleted");
            Loga("Status "+ Status);            
            Loga("PagAtual " + PagAtual);
            Loga("AbsoluteUri " + e.Url.AbsoluteUri);
            Loga(" "); 

            if (this.Text != webBrowser1.DocumentTitle)
            {
                this.Text = webBrowser1.DocumentTitle;
                Loga(this.Text);
            }                        
            switch (Status)
            {
                case tStatus.Canal:
                    if (e.Url.AbsoluteUri == PagAtual)
                    {
                        if (IndiceVideo==0)
                        {
                            ConteudoPagCanal = webBrowser1.DocumentText;
                            iniCanais = ConteudoPagCanal.IndexOf("yt-lockup-content");
                        }                        
                        CriterioVideo();
                        IndiceVideo++;
                        Loga("Video nr " + IndiceVideo.ToString());
                    }
                    break;
                case tStatus.CarregandoVideo:
                    if (e.Url.AbsoluteUri != "about:blank")
                    {
                        if (e.Url.AbsoluteUri != PagAtual)
                        {
                            if (e.Url.AbsoluteUri.IndexOf("google")<0)
                            {
                                // A princípio não da ver se o vídeo terminou
                                // acho que o ideal seria colocar um temporizador
                                // mas se colocar, não iria funcionar, caso for parado ou adiantado o vídeo


                                // Chegou até aqui porque terminou o video e foi pro próximo
                                // Verificar 
                                // 1) Se já não foi visto
                                // 2) Se o vídeo é muito grande ou não
                                CriterioVideo();
                            }                            
                        } else
                        {
                            Status = tStatus.AMostrarOVideo;
                        }
                    }                    
                    break;
                case tStatus.AMostrarOVideo:
                    HoraFim = DateTime.Now.AddSeconds(TmpVideo);
                    timer1.Enabled = true;
                    Status = tStatus.MostrandoVideo;
                    Loga("Status = MostrandoVideo");
                    break;
                default:
                    break;
            }
        }

        private void CriterioVideo()
        {
            Loga("CriterioVideo");
            Boolean Sair = false;
            Boolean Tocar = false;
            Video Esse = null;            
            while (Sair==false)
            {
                Esse = LocalizaVideo();
                if (Esse.Duracao == -1)
                {
                    Sair = true;
                } else {
                    if (Esse.Postado > UltVis)
                    {
                        if (Esse.Duracao < 162000)  // 45 min
                        {
                            Tocar = true;
                            Sair = true;
                        }
                        else
                        {
                            int x = 0;
                        }
                    }
                    if (Tocar)
                    {
                        TocaVideo(Esse);
                    }
                    else
                    {
                        // Passar para o próximo canal
                        int x = 0;
                    }
                }
            }
        }

        private void TocaVideo(Video Esse)
        {
            PagAtual = "https://www.youtube.com/watch?v=" + Esse.EnderVideo;

            /* if (PagAtual== "https://www.youtube.com/watch?v=7IU222pEcnE")
            {
                int x = 0;
            } */

            Loga("Carregando vídeo "+ PagAtual);
            webBrowser1.Navigate(PagAtual);
            Status = tStatus.CarregandoVideo;

            Loga("Tempo do vídeo em segundos: " + Esse.Duracao.ToString());
            TmpVideo = Esse.Duracao;            
        }

        private Video LocalizaVideo()
        {
            Loga("LocalizaVideo");
            Video Esse = new Video();
            int posDI = ConteudoPagCanal.IndexOf("Duração:")+9;
            if (posDI==-1)
            {
                // Não há mais vídeos neste canal
                // Passar para o próximo
                int x = 0;
            } else
            {

                // Passar para SÓ "visualizações"
                int posPostI = ConteudoPagCanal.IndexOf("visualizações");
                // int posPostI = ConteudoPagCanal.IndexOf("visualizações</li><li>");

                if (posPostI>0)
                {
                    // VERIFICAR AQUI
                    // EXEMPLO CORRETO: "9 horas atrás"
                    int postPosF = ConteudoPagCanal.IndexOf("</li></ul>", posPostI);
                    string PedPost = ConteudoPagCanal.Substring(posPostI + 22, postPosF - posPostI - 22);
                    Esse.setPostado(PedPost);
                    Loga("Video postado em " + Esse.Postado.ToShortDateString() + " " + Esse.Postado.ToShortTimeString());

                    int posDF = ConteudoPagCanal.IndexOf(".", posDI);
                    string PedDur = ConteudoPagCanal.Substring(posDI, posDF - posDI);
                    Esse.setsDuracao(PedDur);

                    int posEndI = ConteudoPagCanal.IndexOf("data-context-item-id=");
                    int posEndF = ConteudoPagCanal.IndexOf("data-visibility-tracking", posEndI);
                    Esse.EnderVideo = ConteudoPagCanal.Substring(posEndI + 22, posEndF - posEndI - 30);

                    ConteudoPagCanal = ConteudoPagCanal.Substring(postPosF + 10);
                } else
                {
                    Loga("PASSAR PARA O PRÓXIMO CANAL");
                    // ACABOU O CANAL
                    // PASSAR PARA O PRÓXIMO
                    Status = tStatus.ProxCanal;
                    timer1.Enabled = false;
                    AuxTempo = 0;
                    IndiceCanal++;
                    IndiceVideo = 0;
                    MostrarCanal();
                    Esse.Duracao = -1;
                }
            }
            return Esse;
        }


        /* private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            Loga("ProgressChanged");
            Loga("Status = " + Status.ToString());
            Loga("IndiceVideo = " + IndiceVideo.ToString());
            Loga(" ");

            if (Status == tStatus.MostrandoVideo)
            {
                CriterioVideo();
            }
        } */

        private void Loga(string Texto)
        {
            string Agora = DateTime.Now.ToLongTimeString();
            Console.WriteLine(Agora + " " + Texto);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            AuxTempo++;
            Loga("Timer = " + AuxTempo.ToString());
            if (DateTime.Now > HoraFim)
            {
                if (Status == tStatus.MostrandoVideo)
                {
                    ChamaProxVideo();
                }
            }
        }      

        private void ChamaProxVideo()
        {
            AuxTempo = 0;
            timer1.Enabled = false;
            Loga("Timer: Status = " + tStatus.MostrandoVideo.ToString());
            CriterioVideo();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Pular lista
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ChamaProxVideo();
        }
    }
}
