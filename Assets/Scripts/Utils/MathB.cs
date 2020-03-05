using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Boxsun.Math
{
    public struct MathB
    {
        private static readonly Random RANDOM = new Random();

        #region WeightedRandom

        #region IntegerWeights

        public static T WeightedRandom<T>(T[] choices, int[] weights)
        {
            List<T> weightedList = new List<T>(); //create the weighted list

            for (int i = 0; i < choices.Length; i++)
            {
                for (int j = 0; j < weights[i]; j++)
                {
                    weightedList.Add(choices[i]); //add all the choices multiple times in the list
                }
            }

            int rnd = RANDOM.Next(0, weightedList.Count);

            return weightedList[rnd]; //return the weighted random value
        }

        public static T WeightedRandom<T>(List<T> choices, int[] weights) => WeightedRandom(choices.ToArray(), weights);
        public static T WeightedRandom<T>(List<T> choices, List<int> weights) => WeightedRandom(choices.ToArray(), weights.ToArray());
        public static T WeightedRandom<T>(T[] choices, List<int> weights) => WeightedRandom(choices, weights.ToArray());

        #endregion

        #region FloatWeights

        public static int[] FloatArrayToIntArrayConverter(float[] inputArray)
        {
            List<int> outputList = new List<int>();
            for (int i = 0; i < inputArray.Length; i++)
            {
                float value = inputArray[i] * 100;
                int input = (int)value;
                outputList.Add(input);
            }

            int[] outputArray = outputList.ToArray();
            
            return outputArray;
        }

        public static T WeightedRandom<T>(T[] choices, float[] weights) => WeightedRandom(choices, FloatArrayToIntArrayConverter(weights));
        public static T WeightedRandom<T>(List<T> choices, float[] weights) => WeightedRandom(choices.ToArray(), FloatArrayToIntArrayConverter(weights));
        public static T WeightedRandom<T>(List<T> choices, List<float> weights) => WeightedRandom(choices.ToArray(), FloatArrayToIntArrayConverter(weights.ToArray()));
        public static T WeightedRandom<T>(T[] choices, List<float> weights) => WeightedRandom(choices, FloatArrayToIntArrayConverter(weights.ToArray()));
        
        #endregion

        #endregion

        #region RandomGeneric

        public static T RandomGeneric<T>(T[] values)
        {
            List<T> list = values.ToList(); //convert the array to a list
            list = ShuffleList(list); //shuffle the list
            return list[0]; //return the first values, this is always random because it is shuffled
        }

        public static T RandomGeneric<T>(List<T> values) => RandomGeneric(values.ToArray());

        #endregion

        #region Clamp

        public static int IntClamp(int value, int min, int max)
        {
            return (value <= min) ? min : (value >= max) ? max : value;
        }

        public static float FloatClamp(float value, float min, float max)
        {
            return (value <= min) ? min : (value >= max) ? max : value;
        }

        #endregion

        #region ShuffleList

        public static List<T> ShuffleList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>();

            while (inputList.Count > 0)
            {
                int rnd = RANDOM.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[rnd]); //add it to the new, random list
                inputList.RemoveAt(rnd); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        public static T[] ShuffleArray<T>(T[] inputArray)
        {
            List<T> list = inputArray.ToList(); //convert the array to a list
            list = ShuffleList(list); //shuffle the list
            return list.ToArray(); //convert the shuffled list back to a array
        }

        #endregion

        #region ConvertToSystemVector2

        public static Vector2 Vector2Conversion(float x, float y)
        {
            return new Vector2(x, y);
        }

        #endregion
    }
}