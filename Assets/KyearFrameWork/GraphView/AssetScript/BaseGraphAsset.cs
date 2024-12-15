using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kyear.Graph
{
    [CreateAssetMenu(menuName = "测试脚本/Kyear", fileName = "新文件", order = 81)]
    public class BaseGraphAsset : ScriptableObject
    {
        public BaseGraphNodeAsset startNode = null;
    }


}
