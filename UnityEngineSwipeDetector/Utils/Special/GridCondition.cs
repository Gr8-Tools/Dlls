using System;
using System.Collections.Generic;
using System.Linq;
using Swipe.Utils.Send;
using UnityEngine;

namespace Swipe.Utils.Special
{
    /// <summary>
    /// Условие проверки по сетке
    /// </summary>
    [Serializable]
    public class GridCondition
    {
        [Serializable]
        private struct CellDependenceData
        {
            public int startIndex;
            public int[] finishIndexes;
        }

        [Header("Usability")] 
        [SerializeField] private bool isUsed;
        [SerializeField] private bool initializedFromInspector;

        [Header("Settings")] 
        [SerializeField] private CellDependenceData[] cellDependenceData;

        public bool IsUsed => isUsed;
        
        private readonly Dictionary<uint, uint[]> _cellDependence = new Dictionary<uint, uint[]>();

        public GridCondition(in Dictionary<uint, uint[]> cellDependence)
        {
            foreach (var kvp in cellDependence)
                _cellDependence.Add(kvp.Key, kvp.Value);

            initializedFromInspector = false;
            isUsed = true;
        }

        /// <summary>
        /// Выполнены ли условия проверки совпаления свайпа по сетке 
        /// </summary>
        public bool Achieved(SwipeInfo swipeInfo)
        {
            if(initializedFromInspector)
                GenerateDictionary();

            if (!_cellDependence.Any())
                return false;

            if (!_cellDependence.ContainsKey(swipeInfo.StartInfo.CellId))
                return false;
            
            var finishInfos = _cellDependence[swipeInfo.StartInfo.CellId];
            return finishInfos.Any(id => id == swipeInfo.FinishInfo.CellId);
        }
        
        /// <summary>
        /// Генирует единожды словарь из массива структур CellDependenceData в том случае,
        /// если условие было создано из инспектора
        /// </summary>
        private void GenerateDictionary()
        {
            foreach (var cdd in cellDependenceData)
            {
                if(cdd.finishIndexes.Length == 0)
                    continue;

                _cellDependence.Add(
                    (uint) cdd.startIndex,
                    cdd.finishIndexes.Select(integer => (uint) integer).ToArray());
            }

            initializedFromInspector = false;
        }
    }
}