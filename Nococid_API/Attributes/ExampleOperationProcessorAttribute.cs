using Newtonsoft.Json;
using NSwag;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Attributes
{
    public class ExampleOperationProcessorAttribute : OpenApiOperationProcessorAttribute
    {
        public ExampleOperationProcessorAttribute(Type exampleType, Type ortherExamples = null)
            : base(typeof(ExampleOperationProcessor), exampleType, ortherExamples) { }
    }

    public class ExampleOperationProcessor : IOperationProcessor
    {
        private readonly Type exampleType;
        private readonly Type ortherExamples;

        public ExampleOperationProcessor(Type exampleType, Type ortherExamples)
        {
            this.exampleType = exampleType;
            this.ortherExamples = ortherExamples;
        }

        public bool Process(OperationProcessorContext context)
        {
            try
            {
                if (context.OperationDescription.Operation.RequestBody.Content.TryGetValue("application/json", out OpenApiMediaType value))
                {
                    object exampleObject;
                    if (ortherExamples != null)
                    {
                        exampleObject = ortherExamples.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        exampleObject = ortherExamples.GetMethod("GetExamples").Invoke(exampleObject, null);
                        value.Example = exampleObject;
                    }

                    exampleObject = exampleType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                    value.Schema.ActualTypeSchema.Example = exampleObject;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
