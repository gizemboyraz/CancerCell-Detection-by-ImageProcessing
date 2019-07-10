using Accord.MachineLearning.Bayes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    class MachineLearning
    {

        private NaiveBayes bayes;

        private int[] outputs;
        private int[][] inputs;

        private Dictionary<string, int> className;
        private Dictionary<string, int> age;
        private Dictionary<string, int> menopause;
        private Dictionary<string, int> tumorSize;
        private Dictionary<string, int> breast;
        private Dictionary<string, int> irradiant;

        public MachineLearning()
        {
            createDictionary();
            readDataset();
            learn();
        }


        // datasetten okunan string değerlerinin int olan karşılıklarını oluşturur
        public void createDictionary()
        {
            age = new Dictionary<string, int>();

            age.Add("10-19", 0);
            age.Add("20-29", 1);
            age.Add("30-39", 2);
            age.Add("40-49", 3);
            age.Add("50-59", 4);
            age.Add("60-69", 5);
            age.Add("70-79", 6);
            age.Add("80-89", 7);
            age.Add("90-99", 8);

            menopause = new Dictionary<string, int>();

            menopause.Add("lt40", 0);
            menopause.Add("ge40", 1);
            menopause.Add("premeno", 2);


            tumorSize = new Dictionary<string, int>();

            tumorSize.Add("0-4", 0);
            tumorSize.Add("5-9", 1);
            tumorSize.Add("10-14", 2);
            tumorSize.Add("15-19", 3);
            tumorSize.Add("20-24", 4);
            tumorSize.Add("25-29", 5);
            tumorSize.Add("30-34", 6);
            tumorSize.Add("35-39", 7);
            tumorSize.Add("40-44", 8);
            tumorSize.Add("45-49", 9);
            tumorSize.Add("50-54", 10);
            tumorSize.Add("55-59", 11);

            breast = new Dictionary<string, int>();

            breast.Add("left", 0);
            breast.Add("right", 1);

            irradiant = new Dictionary<string, int>();

            irradiant.Add("yes", 0);
            irradiant.Add("no", 1);

            className = new Dictionary<string, int>();

            className.Add("no-recurrence-events", 0);
            className.Add("recurrence-events", 1);

        }


        //dosyadan verileri okuyup, bunları karşılıkları olan int değerlerine çevirdikten sonra inputs ve outputs dizilerine yerleştirir
        public void readDataset()
        {
            outputs = new int[286];
            inputs = new int[286][];
            int counter = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\GIZEM\source\repos\ImageProcessing\breast-cancer.data");
            while ((line = file.ReadLine()) != null)
            {

                string[] values = line.Split(',');

                className.TryGetValue(values[0], out outputs[counter]);

                int[] temp = new int[5];

                age.TryGetValue(values[1], out temp[0]);
                menopause.TryGetValue(values[2], out temp[1]);
                tumorSize.TryGetValue(values[3], out temp[2]);
                breast.TryGetValue(values[7], out temp[3]);
                irradiant.TryGetValue(values[9], out temp[4]);

                inputs[counter] = new[] { temp[0], temp[1], temp[2], temp[3], temp[4] };

                counter++;
            }

            file.Close();

        }

        //inputs ve outputs dizilerindeki verileri alıp Bayes classını kullanarak öğrenme işlemini yapar
        public void learn()
        {
            bayes = new NaiveBayes(classes: 2, symbols: new[] { 9, 3, 12, 2, 2 });

            var learning = new NaiveBayesLearning()
            {
                Model = bayes
            };
            learning.Options.InnerOption.UseLaplaceRule = true;


            learning.Learn(inputs, outputs);

        }

        //Arayüzden girilen değerleri parametre olarak alıp karar verme işlemi uygulanır
        public int process(string ageStr,string menopauseStr,string tumorSizeStr,string breastStr,string irradiatStr)
        {
            
            int ageInt, menopauseInt, tumorSizeInt, breastInt, IrradiatInt;

            age.TryGetValue(ageStr,out ageInt);
            menopause.TryGetValue(menopauseStr,out menopauseInt);
            tumorSize.TryGetValue(tumorSizeStr, out tumorSizeInt);
            breast.TryGetValue(breastStr, out breastInt);
            irradiant.TryGetValue(irradiatStr, out IrradiatInt);

            int answer = bayes.Decide(new int[] { ageInt, menopauseInt, tumorSizeInt, breastInt, IrradiatInt });

            return answer;

        }
    }
}
