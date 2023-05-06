using System;

namespace MLApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Load sample data
            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = @"D:\Projects_C#\Coconut-Pest-Identifier\Coconut Leaf Images\WCLWD_Yellowing\DSC_0071.JPG",
            };


            var result = MLModel.Predict(sampleData);


            Console.Clear();
            Console.WriteLine("Prediction is: ");
            Console.WriteLine(result.Prediction);
        }
    }
}
