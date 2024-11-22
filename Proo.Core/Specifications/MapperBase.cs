using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Specifications
{
    public abstract class MapperBase<TSource, TDestination>
    {
        /// <summary>
        /// Map Destination Entity List into Source Entity List.
        /// </summary>
        /// <param name="destinationEntities"></param>
        /// <returns></returns>
        public List<TSource> MapFromDestinationToSource(List<TDestination> destinationEntities)
        {
            if (destinationEntities != null)
            {
                List<TSource> sourceList = new List<TSource>();

                foreach (TDestination destinationEntity in destinationEntities)
                {
                    sourceList.Add(MapFromDestinationToSource(destinationEntity));
                }

                return sourceList;
            }

            return null;
        }

        /// <summary>
        /// Map Source Entity List into Destination Entity List.
        /// </summary>
        /// <param name="sourceEntities"></param>
        /// <returns></returns>
        public List<TDestination> MapFromSourceToDestination(List<TSource> sourceEntities)
        {
            if (sourceEntities != null)
            {
                List<TDestination> destinationList = new List<TDestination>();

                foreach (TSource sourceEntity in sourceEntities)
                {
                    destinationList.Add(MapFromSourceToDestination(sourceEntity));
                }

                return destinationList;
            }

            return null;
        }

        /// <summary>
        /// Map Destination Entity into Source Entity.
        /// </summary>
        /// <param name="destinationEntity"></param>
        /// <returns></returns>
        public abstract TSource MapFromDestinationToSource(TDestination destinationEntity);

        /// <summary>
        /// Map Source Entity into Destination Entity.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns></returns>
        public abstract TDestination MapFromSourceToDestination(TSource sourceEntity);
    }
}
