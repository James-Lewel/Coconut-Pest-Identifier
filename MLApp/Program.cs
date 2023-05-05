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
                ImageSource = @"D:\Projects_C#\Cocunot-Pest-Identifier\Coconut Leaf Images\CCI_Caterpillars\CCI_10_0_jpg.rf.041b1c4c936db7346c0e1cc66e687757.jpg",
            };


            var result = MLModel.Predict(sampleData);


            Console.Clear();
            Console.WriteLine("Prediction is: ");
            Console.WriteLine(result.Prediction);
        }
    }
}
