using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr5_10
{
    class SimplexMethod
    {
        static void Main()
        {
            double[] objectiveCoefficients = { 3, 5, 4 };
            double[,] constraintCoefficients = {
                { 2, 3, 1 },
                { 0, 4, 6 },
                { 3, 1, 0 },
                { 1, 0, 1 }
            };
            double[] constraints = { 240, 180, 200, 160 };

            var result = SolveLinearProgrammingProblem(objectiveCoefficients, constraintCoefficients, constraints);

            Console.WriteLine("Севастьянов, Вариант18");
            Console.WriteLine("Оптимальное значение целевой функции: " + result.Item1);
            Console.Write("Оптимальный набор переменных: [ ");
            Console.Write(string.Join(", ", result.Item2));
            Console.WriteLine(" ]");
            Console.ReadLine();
        }

        static Tuple<double, double[]> SolveLinearProgrammingProblem(double[] objectiveCoefficients, double[,] constraintCoefficients, double[] constraints)
        {
            int numberOfRows = constraintCoefficients.GetLength(0);
            int numberOfVariables = constraintCoefficients.GetLength(1);

            double[,] tableau = InitializeTableau(objectiveCoefficients, constraintCoefficients, constraints, numberOfRows, numberOfVariables);

            int[] basis = DetermineInitialBasis(numberOfVariables, numberOfRows);

            RunSimplexAlgorithm(tableau, numberOfRows, numberOfVariables, basis);

            return ExtractOptimalSolution(tableau, basis, numberOfVariables);
        }

        static double[,] InitializeTableau(double[] objectiveCoefficients, double[,] constraintCoefficients, double[] constraints, int numberOfRows, int numberOfVariables)
        {
            double[,] tableau = new double[numberOfRows + 1, numberOfVariables + numberOfRows + 1];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfVariables; j++)
                {
                    tableau[i, j] = constraintCoefficients[i, j];
                }

                tableau[i, numberOfVariables + i] = 1;
                tableau[i, numberOfVariables + numberOfRows] = constraints[i];
            }

            for (int j = 0; j < numberOfVariables; j++)
            {
                tableau[numberOfRows, j] = -objectiveCoefficients[j];
            }

            return tableau;
        }

        static int[] DetermineInitialBasis(int numberOfVariables, int numberOfRows)
        {
            int[] basis = new int[numberOfRows];
            for (int i = 0; i < numberOfRows; i++)
            {
                basis[i] = numberOfVariables + i;
            }
            return basis;
        }

        static void RunSimplexAlgorithm(double[,] tableau, int numberOfRows, int numberOfVariables, int[] basis)
        {
            while (true)
            {
                int pivotColumn = 0;
                for (int j = 1; j < numberOfVariables + numberOfRows; j++)
                {
                    if (tableau[numberOfRows, j] < tableau[numberOfRows, pivotColumn])
                    {
                        pivotColumn = j;
                    }
                }

                if (tableau[numberOfRows, pivotColumn] >= 0)
                {
                    break;
                }

                int pivotRow = -1;
                for (int i = 0; i < numberOfRows; i++)
                {
                    if (tableau[i, pivotColumn] > 0)
                    {
                        if (pivotRow == -1)
                        {
                            pivotRow = i;
                        }
                        else
                        {
                            double ratio1 = tableau[i, numberOfVariables + numberOfRows] / tableau[i, pivotColumn];
                            double ratio2 = tableau[pivotRow, numberOfVariables + numberOfRows] / tableau[pivotRow, pivotColumn];
                            if (ratio1 < ratio2)
                            {
                                pivotRow = i;
                            }
                        }
                    }
                }

                double pivotElement = tableau[pivotRow, pivotColumn];
                for (int j = 0; j < numberOfVariables + numberOfRows + 1; j++)
                {
                    tableau[pivotRow, j] /= pivotElement;
                }

                for (int i = 0; i < numberOfRows + 1; i++)
                {
                    if (i != pivotRow)
                    {
                        double multiplier = tableau[i, pivotColumn];
                        for (int j = 0; j < numberOfVariables + numberOfRows + 1; j++)
                        {
                            tableau[i, j] -= multiplier * tableau[pivotRow, j];
                        }
                    }
                }

                basis[pivotRow] = pivotColumn;
            }
        }

        static Tuple<double, double[]> ExtractOptimalSolution(double[,] tableau, int[] basis, int numberOfVariables)
        {
            double optimalValue = tableau[tableau.GetLength(0) - 1, tableau.GetLength(1) - 1];
            double[] optimalSolution = new double[numberOfVariables];

            for (int i = 0; i < basis.Length; i++)
            {
                if (basis[i] < numberOfVariables)
                {
                    optimalSolution[basis[i]] = tableau[i, tableau.GetLength(1) - 1];
                }
            }

            return Tuple.Create(optimalValue, optimalSolution);
        }
    }
}
