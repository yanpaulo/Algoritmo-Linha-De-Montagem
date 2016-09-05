using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static System.Console;

namespace AlgoritmoLinhaDeMontagem
{
    #region Modelos
    class Estacao
    {
        public Estacao(int tempoProducao, int? tempoTransferencia = null)
        {
            TempoProducao = tempoProducao;
            TempoTransferencia = tempoTransferencia;
        }
        public int TempoProducao { get; set; }
        public int? TempoTransferencia { get; set; }
        public int? TempoRealProducao { get; set; }
        public LinhaProducao Linha { get; set; }

    }

    class LinhaProducao
    {
        public int TempoEntrada { get; set; }
        public int TempoSaida { get; set; }
        public List<Estacao> Estacoes { get; set; }
        public int Numero { get; internal set; }

        public Estacao this[int index]
        {
            get { return Estacoes[index]; }
            set { Estacoes[index] = value; }
        }
    }

    #endregion
    class Program
    {
        static void Main(string[] args)
        {
            //Criação declarativa (não-programática) das Linhas f1 e f2
            var f1 = new LinhaProducao
            {
                Numero = 1,
                TempoEntrada = 2,
                TempoSaida = 3,
                Estacoes = new List<Estacao>
                {
                    new Estacao(7, 2), new Estacao(9, 3), new Estacao(3, 1),
                    new Estacao(4, 3), new Estacao(8, 4), new Estacao(4)
                }
            };
            var f2 = new LinhaProducao
            {
                Numero = 2,
                TempoEntrada = 4,
                TempoSaida = 2,
                Estacoes = new List<Estacao>()
                {
                    new Estacao(8, 2), new Estacao(5, 1), new Estacao(6, 2),
                    new Estacao(4, 2), new Estacao(5, 1), new Estacao(7)
                }
            };
            var fList = new List<LinhaProducao> { f1, f2 };
            //Linq Query para atribuir a linha à qual cada estação pertence.
            fList.ForEach(f => f.Estacoes.ForEach(e => e.Linha = f));

            //Já sabemos o que AlgoritmoDinamico(f1, f2) retorna.
            ///Não sabe? Passe o mouse em cima! :D
            var path = AlgoritmoDinamico(f1, f2).ToList();
            
            path.Select((e, i) =>
                new
                {
                    Linha = e.Linha.Numero,
                    Estacao = i + 1,
                    Tempo = e.TempoRealProducao
                }
            ).ToList().ForEach(e => WriteLine($"Linha: {e.Linha}, Estação: {e.Estacao}, Tempo: {e.Tempo}"));

            WriteLine();

            foreach (var f in fList)
            {
                f.Estacoes.ForEach(e => { Write($"{e.TempoRealProducao}{(path.Contains(e) ? "*" : "")}\t"); });
                WriteLine();
            }
                

            ReadKey();
        }

        /// <summary>
        /// Calcula o TRP (Tempo Real de Produção) para as estações da linha f1 e f2.
        /// </summary>
        /// <param name="f1">Linha de produção 1</param>
        /// <param name="f2">Linha de produção 2</param>
        /// <returns>Estacao[]: Vetor com o caminho de menor custo.</returns>
        static Estacao[] AlgoritmoDinamico(LinhaProducao f1, LinhaProducao f2)
        {
            //Vetor de saída. Representa o caminho mais curto.
            var path = new List<Estacao>();
            //Variáveis para "segurar" as estações correntes e1 e e2 (_e1 e _e2 indicam as estações anteriores)
            Estacao e1 = f1[0], e2 = f2[0], _e1, _e2;
            //Pra "segurar" os valores calculados para a estação horizontalmente adjacente
            //e para a percentence à outra linha, respectivamente.
            int v1, v2;
            e1.TempoRealProducao = f1.TempoEntrada + e1.TempoProducao;
            e2.TempoRealProducao = f2.TempoEntrada + e2.TempoProducao;

            //Sempre que calcular o TRP de um par (e1, e2), adicionar a estação com menor TRP ao vetor de saída.
            path.Add(e1.TempoRealProducao <= e2.TempoRealProducao ? e1 : e2);

            for (int i = 1; i < f1.Estacoes.Count; i++)
            {
                e1 = f1[i]; e2 = f2[i]; _e1 = f1[i - 1]; _e2 = f2[i - 1];
                //TRP e TempoTransferencia devem ter valores nesse ponto.
                //Se não tiver, LASCA exception na cara do programador imundo.
                v1 = _e1.TempoRealProducao.Value + e1.TempoProducao;
                v2 = _e2.TempoRealProducao.Value + _e2.TempoTransferencia.GetValueOrDefault(0) + e1.TempoProducao;

                e1.TempoRealProducao = Min(v1, v2);
                
                //Note que agora v1 é referente ao tempo de _e2, e não de _e1
                v1 = _e2.TempoRealProducao.Value + e2.TempoProducao;
                v2 = _e1.TempoRealProducao.Value + _e1.TempoTransferencia.GetValueOrDefault(0) + e2.TempoProducao;

                e2.TempoRealProducao = Min(v1, v2);

                //Sempre que calcular o TRP de um par (e1, e2), adicionar a estação com menor TRP ao vetor de saída.
                path.Add(e1.TempoRealProducao <= e2.TempoRealProducao ? e1 : e2);
            }
            //e1 e e2 representam as estações e1 e e2 da última posição, vide o loop.
            //Já de posse dessas referências, acrescentar a cada uma o tempo de saída de
            //suas respectivas linhas de produção
            e1.TempoRealProducao += f1.TempoSaida;
            e2.TempoRealProducao += f2.TempoSaida;

            //Sobrescreve a última posição com a estação com menor TRP após a adição do tempo de saída.
            path[path.Count - 1] = e1.TempoRealProducao <= e2.TempoRealProducao ? e1 : e2;
            return path.ToArray();
        }
    }
}
