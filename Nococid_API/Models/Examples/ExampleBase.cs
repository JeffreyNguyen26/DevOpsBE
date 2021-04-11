using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples
{
    public interface IExampleBase
    {
        string ConvertExample<TExamples>(TExamples examples) where TExamples : ExampleBase;
    }
    public class ExampleBase : IExampleBase
    {
        public virtual string ConvertExample<TExamples>(TExamples examples) where TExamples : ExampleBase
        {
            string example = "";
            FieldInfo[] fieldInfos = examples.GetType().GetFields();
            if (fieldInfos.Length != 0)
            {
                foreach (var fieldInfo in fieldInfos)
                {
                    example += "\n - " + fieldInfo.Name + " : "
                        + JsonConvert.SerializeObject(fieldInfo.GetValue(examples));
                }
            }
            return example;
        }
    }
}
