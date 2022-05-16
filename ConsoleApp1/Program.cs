using System;
using System.Collections;
using System.Globalization;

namespace ConsoleApp1
{
    internal class Program
    {
        #region "Enum"
        enum CategoriaTipo
        {
            EXPIRED,
            HIGHRISK,
            MEDIUMRISK
        }

        enum ClienteTipo
        {
            Private,
            Public
        }
        #endregion

        #region "Interface"

        interface ITrade
        {
            double Value { get; }
            string ClientSector { get; }
            DateTime NextPaymentDate { get; }
        }


        interface ICategoria
        {
            string ObterCategoria(string dataReferencia, string dados);
        }

        interface ICategoriaFactory
        {
            ICategoria CreateCategoria(CategoriaTipo tipo);
        }

        #endregion


        class CategoriaExpired : ICategoria
        {
            string ICategoria.ObterCategoria(string dataReferencia, string dados)
            {
               
                string[] transacao = dados.Split(" ");
                string valor = (transacao[0]), tipoCliente = transacao[1], datapagto = transacao[2];

                Trade trade = new Trade(Convert.ToDouble(valor), tipoCliente, DateTime.ParseExact(datapagto, "M/d/yyyy", CultureInfo.InvariantCulture));

                TimeSpan date = (TimeSpan)(trade.NextPaymentDate - DateTime.ParseExact(dataReferencia, "M/d/yyyy", CultureInfo.InvariantCulture));
                if (date.Days >= 30)
                {
                    return (nameof(CategoriaTipo.EXPIRED));
                }
                return String.Empty;
            }
            
        }

        class CategoriaHIGHRISK : ICategoria
        {
            string ICategoria.ObterCategoria(string dataReferencia, string dados)
            {
                string[] transacao = dados.Split(" ");
                string valor = (transacao[0]), tipoCliente = transacao[1], datapagto = transacao[2];
                Trade trade = new Trade(Convert.ToDouble(valor), tipoCliente, DateTime.ParseExact(datapagto, "M/d/yyyy", CultureInfo.InvariantCulture));

                if(trade.Value > 1000000 && trade.ClientSector == nameof(ClienteTipo.Private))
                {
                    return (nameof(CategoriaTipo.HIGHRISK));
                }

                return String.Empty;
            }
        }

        class CategoriaMEDIUMRISK : ICategoria
        {
            string ICategoria.ObterCategoria(string dataReferencia, string dados)
            {
                string[] transacao = dados.Split(" ");
                string valor = (transacao[0]), tipoCliente = transacao[1], datapagto = transacao[2];
                Trade trade = new Trade(Convert.ToDouble(valor), tipoCliente, DateTime.ParseExact(datapagto, "M/d/yyyy", CultureInfo.InvariantCulture));

                if (trade.Value > 1000000 && trade.ClientSector == nameof(ClienteTipo.Public))
                {
                    return (nameof(CategoriaTipo.MEDIUMRISK));
                }

                return String.Empty;
            }
        }

        class CategoriaNone : ICategoria
        {
            string ICategoria.ObterCategoria(string dataReferencia, string dados)
            {
                throw new NotImplementedException();
            }
        }



        class CategoriaFactory: ICategoriaFactory
        {
            public ICategoria CreateCategoria(CategoriaTipo tipo)
            {
                switch (tipo)
                {
                    case CategoriaTipo.EXPIRED:
                        return new CategoriaExpired();
                    case CategoriaTipo.HIGHRISK:
                        return new CategoriaHIGHRISK();
                    case CategoriaTipo.MEDIUMRISK:
                        return new CategoriaMEDIUMRISK();
                    default:
                        return new CategoriaNone();
                }
            }
        }

        class Trade:ITrade
        {
            public double Value { get; }
            public string ClientSector { get; }
            public DateTime NextPaymentDate { get; }

            public Trade(double _valor, string _tipocliente, DateTime _dateProximoPagto)
            {
                Value = _valor;
                ClientSector = _tipocliente;
                NextPaymentDate = _dateProximoPagto;
            }
        }
               
      
        static void Main(string[] args)
        {
            
            var arrListRetorno = new ArrayList();
            Console.WriteLine("Entre com a Data de Referencia : ");
            string dataReferencia = Console.ReadLine();
            Console.WriteLine("Entre com o Número de Trades : ");
            int ntrades = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Transacao");

            for (int i =0; i < ntrades;i++)
            {
                string dados = Console.ReadLine();
                ICategoriaFactory factory = new CategoriaFactory();

                ICategoria categoriaExpired = factory.CreateCategoria(CategoriaTipo.EXPIRED);
                string retornoExpired = categoriaExpired.ObterCategoria(dataReferencia, dados);
                if(retornoExpired != String.Empty)
                {
                    arrListRetorno.Add(retornoExpired);
                }
                                
                ICategoria categoriaMediumRisk = factory.CreateCategoria(CategoriaTipo.MEDIUMRISK);
                string retornoMediumRisk = categoriaMediumRisk.ObterCategoria(dataReferencia, dados);
                if (retornoMediumRisk != String.Empty)
                {
                    arrListRetorno.Add(retornoMediumRisk);
                }
                

                ICategoria categoriaHighRisk = factory.CreateCategoria(CategoriaTipo.HIGHRISK);
                string retornocategoriaHighRisk = categoriaHighRisk.ObterCategoria(dataReferencia, dados);
                if (retornocategoriaHighRisk != String.Empty)
                {
                    arrListRetorno.Add(retornocategoriaHighRisk);
                }
                
            }
            
            for (int i=0; i < arrListRetorno.Count; i++)
            {
                Console.WriteLine(arrListRetorno[i]);
            }
        }
    }
}
