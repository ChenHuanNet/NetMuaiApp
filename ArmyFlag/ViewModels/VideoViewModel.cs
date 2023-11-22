using Army.Domain.Dto;
using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyFlag.ViewModels
{

    public class VideoViewModel
    {


        public static List<VideoSourceDto> VideoSourceList = new List<VideoSourceDto>();
        public VideoViewModel()
        {
            VideoSourceList = new List<VideoSourceDto>()
        {
            new VideoSourceDto(){ Content="最大",Value="zd" },
            new VideoSourceDto(){ Content="永久",Value="yj" },
            new VideoSourceDto(){ Content="牛牛",Value="hn" },
            new VideoSourceDto(){ Content="光波",Value="gs" },
            new VideoSourceDto(){ Content="新朗",Value="sn" },
            new VideoSourceDto(){ Content="涡轮",Value="wl" },
            new VideoSourceDto(){ Content="良子",Value="lz" },
            new VideoSourceDto(){ Content="F速",Value="fs" },
            new VideoSourceDto(){ Content="飞飞",Value="ff" },
            new VideoSourceDto(){ Content="百度",Value="bd" },
            new VideoSourceDto(){ Content="酷U",Value="uk" },
            new VideoSourceDto(){ Content="无天",Value="wj" },
            new VideoSourceDto(){ Content="八戒",Value="bj" },
            new VideoSourceDto(){ Content="天空",Value="tk" },
            new VideoSourceDto(){ Content="速速",Value="ss" },
            new VideoSourceDto(){ Content="酷播",Value="kb" },
            new VideoSourceDto(){ Content="闪电",Value="sd" },
            new VideoSourceDto(){ Content="看看",Value="xk" },
            new VideoSourceDto(){ Content="淘淘",Value="tp" },
            new VideoSourceDto(){ Content="精英",Value="jy" }
        };
        }

    }
}
