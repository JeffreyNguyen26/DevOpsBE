using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Utilities
{
    public class StageUtils
    {
        public static StageEnum GetNextStage(StageEnum current_stage)
        {
            return current_stage switch
            {
                StageEnum.Planning => StageEnum.Coding,
                StageEnum.Coding => StageEnum.Build,
                StageEnum.Build => StageEnum.Test,
                StageEnum.Test => StageEnum.Release,
                StageEnum.Release => StageEnum.Deploy,
                StageEnum.Deploy => StageEnum.Operate,
                StageEnum.Operate => StageEnum.Monitor,
                _ => StageEnum.Planning,
            };
        }
    }
}
