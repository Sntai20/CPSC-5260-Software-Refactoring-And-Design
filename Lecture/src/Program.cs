using Lecture;

int range = 20;
int totalCombinations = 0;
int[] combination = Combinations.InitializeCombination(6);

do
{
    Combinations.PrintCombination(combination);
    totalCombinations++;
}
while (Combinations.NextCombination(combination, range));

Console.WriteLine("TotalCombinations: " + totalCombinations);
