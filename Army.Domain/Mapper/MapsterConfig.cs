using Army.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Mapper
{
    public class MapsterConfig
    {
        public static void Initialization()
        {
            TypeAdapterConfig.GlobalSettings.ForType<DilidiliPCSource, MyCollection>()
                .Ignore(x => x.Id)
                .Map(dest => dest.SourceId, source => source.Id);


            TypeAdapterConfig.GlobalSettings.ForType<DilidiliPCSource, DilidiliPCSource>();


            TypeAdapterConfig.GlobalSettings.ForType<DilidiliPCSourceItem, DilidiliPCSourceItem>();


            TypeAdapterConfig.GlobalSettings.ForType<MyCollection, MyCollection>();
        }
    }
}
