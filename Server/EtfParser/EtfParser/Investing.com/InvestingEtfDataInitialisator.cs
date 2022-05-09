﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class InvestingEtfDataInitialisator : IEtfDataInitialisator
    {
        private IEtfCompositionDataProvider _etfCompositionDataProvider;
        private IEtfCostAndIsinProvider _etfCostProvider;
        public InvestingEtfDataInitialisator(IEtfCompositionDataProvider etfCompositionDataProvider, IEtfCostAndIsinProvider etfCostProvider)
        {
            _etfCompositionDataProvider = etfCompositionDataProvider;
            _etfCostProvider = etfCostProvider;
        }
        public void InitialiseEtfData(IEtfData finexEtfData)
        {
            //if (finexEtfData is not FinexEtfData)
            //    throw new ArgumentException("finexEtfData should be FinexEtfData");

            //string? isin = 
            var tuple =  _etfCostProvider.GetCostAndIsinByTicker(finexEtfData.Ticker);
            double? cost = tuple.Item1;
            string? isin = tuple.Item2;
            List<IShareData>? shareDatas
                = _etfCompositionDataProvider.GetComposition(finexEtfData.Ticker)?.ToList();
            if (shareDatas == null)
                throw new NullReferenceException("shareDatas was null. Probably something wrong with holdings file parsing.");
            Dictionary<string, double> isinToPartInEtf = new Dictionary<string, double>();
            foreach (var shareData in shareDatas)
            {
                if (shareData.Isin == null || shareData.PartInEtf == null)
                    throw new NullReferenceException("Isin or PartInEtf was null");
                isinToPartInEtf[shareData.Isin] = (double)shareData.PartInEtf;
            }

            finexEtfData.Initialise(cost, shareDatas, isinToPartInEtf, isin);
        }
    }
}
